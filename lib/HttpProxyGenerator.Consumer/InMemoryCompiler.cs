using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace HttpProxyGenerator.Consumer
{
    internal class InMemoryCompiler
    {
        public Assembly CompileCSharpCode(SyntaxTree syntaxTree, IEnumerable<Assembly> assemblies)
        {
            string assemblyName = Guid.NewGuid().ToString();
            var references = GetAssemblyReferences(assemblies);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            
            ThrowExceptionIfCompilationFailure(result);
            
            ms.Seek(0, SeekOrigin.Begin);
            
            return Assembly.Load(ms.ToArray());
        }

        private void ThrowExceptionIfCompilationFailure(EmitResult result)
        {
            if (!result.Success)
            {
                var compilationErrors = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error)
                    .ToList();
                if (compilationErrors.Any())
                {
                    var firstError = compilationErrors.First();
                    var errorNumber = firstError.Id;
                    var errorDescription = firstError.GetMessage();
                    var firstErrorMessage = $"{errorNumber}: {errorDescription};";
                    throw new InvalidOperationException($"Compilation failed, first error is: {firstErrorMessage}");
                }
            }
        }

        private static IEnumerable<MetadataReference> GetAssemblyReferences(IEnumerable<Assembly> assemblies)
        {
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            return Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Concat(new List<Assembly>(){ typeof(object).Assembly })
                .Concat(assemblies)
                .Select(x => MetadataReference.CreateFromFile(x.Location))
                .Concat(new List<PortableExecutableReference>(){ MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")) })
                ;
        }
    }
}
