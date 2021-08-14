using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HttpProxyGenerator.Tests
{
    [TestFixture]
    public class ControllerGeneratorTests
    {
        [Test]
        public void Generate()
        {
            var options = new ControllerGeneratorOptions();
            options.RegisterInterfaceToExpose<IInterfaceToGenerate>();
            var generator = new ControllerGenerator(options);

            var (result, assemblies) = generator.Generate();
            Console.WriteLine(result);

            var compiler = new InMemoryCompiler();

            var assembly = compiler.CompileCSharpCode(result, assemblies);
            var types = assembly.GetExportedTypes().Select(x => x.FullName);
            Console.WriteLine(string.Join(',', types));
        }
    }
}
