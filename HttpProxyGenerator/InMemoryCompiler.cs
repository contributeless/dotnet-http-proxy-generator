using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace HttpProxyGenerator
{
    public class InMemoryCompiler
    {
        public Assembly CompileCSharpCode(string code, List<Assembly> assemblies)
        {
            var sw = new Stopwatch();
            sw.Start();

            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            Console.WriteLine($"--Code parsed in {sw.ElapsedMilliseconds}");

            string assemblyName = Guid.NewGuid().ToString();
            var references = GetAssemblyReferences(assemblies);

            Console.WriteLine($"--References found in {sw.ElapsedMilliseconds}");

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            Console.WriteLine($"--Compilation validated in {sw.ElapsedMilliseconds}");

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            ThrowExceptionIfCompilationFailure(result);
            ms.Seek(0, SeekOrigin.Begin);

            Console.WriteLine($"--Compiled in {sw.ElapsedMilliseconds}");
            var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(ms);

            Console.WriteLine($"--Loaded in {sw.ElapsedMilliseconds}");

            sw.Stop();
            return assembly;
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
                    throw new Exception($"Compilation failed, first error is: {firstErrorMessage}");
                }
            }
        }

        private static IEnumerable<MetadataReference> GetAssemblyReferences(List<Assembly> assemblies)
        {
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            return Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                .Select(x => Assembly.Load(x))
                .Concat(new List<Assembly>(){ typeof(System.Object).Assembly, Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly() })
                .Concat(assemblies)
                .Select(x => MetadataReference.CreateFromFile(x.Location))
                .Concat(new List<PortableExecutableReference>(){ MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")) })
                ;
        }
    }
}
