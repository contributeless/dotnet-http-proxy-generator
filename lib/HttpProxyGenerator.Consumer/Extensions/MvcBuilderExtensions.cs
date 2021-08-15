using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HttpProxyGenerator.Consumer.Extensions
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder RegisterHttpProxyEndpoints(this IMvcBuilder mvcBuilder, Action<ControllerGeneratorOptions> configure)
        {
            var assembly = mvcBuilder.Services.RegisterHttpProxyEndpoints(configure);

            return mvcBuilder.AddApplicationPart(assembly);
        }

        private static Assembly RegisterHttpProxyEndpoints(this IServiceCollection serviceCollection, Action<ControllerGeneratorOptions> configure)
        {
            var options = new ControllerGeneratorOptions();

            configure(options);

            options.PostConfigureValidate();

            var controllerGenerator = new ControllerGenerator(options);

            var syntaxTree = controllerGenerator.Generate();

            var compiler = new InMemoryCompiler();

            var referencedAssemblies = options.InterfacesToExpose.Select(x => x.Assembly)
                .Concat(new [] { Assembly.GetCallingAssembly(), Assembly.GetEntryAssembly() })
                .ToList();

            var resultAssembly = compiler.CompileCSharpCode(syntaxTree, referencedAssemblies);

            var controllers = resultAssembly.GetExportedTypes().Where(x => typeof(ControllerBase).IsAssignableFrom(x));

            foreach (var type in controllers)
            {
                serviceCollection.AddTransient(type);
            }

            return resultAssembly;
        }
    }
}
