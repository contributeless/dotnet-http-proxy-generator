using System;
using System.Threading.Tasks;
using HttpProxyGenerator.Abstractions;

namespace HttpProxyGenerator.Extensions
{
    public static class ControllerGeneratorOptionsExtensions
    {
        public static void RegisterInterfaceToExpose<TInterface>(this ControllerGeneratorOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var type = typeof(TInterface);

            if(!type.IsInterface)
            {
                throw new ArgumentException($"{type.Name} should be interface type");
            }

            options.InterfacesToExpose.Add(type);
        }

        public static void RegisterNamingConventionProvider(this ControllerGeneratorOptions options, IProxyNamingConventionProvider namingConventionProvider)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (namingConventionProvider is null)
            {
                throw new ArgumentNullException(nameof(namingConventionProvider));
            }

            options.NamingConventionProvider = namingConventionProvider;
        }

        public static void RegisterProxyContractProvider(this ControllerGeneratorOptions options, IProxyContractProvider contractProvider)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (contractProvider is null)
            {
                throw new ArgumentNullException(nameof(contractProvider));
            }

            options.ProxyContractProvider = contractProvider;
        }

        internal static void PostConfigureValidate(this ControllerGeneratorOptions options)
        {
            foreach (var type in options.InterfacesToExpose)
            {
                EnsureAllEndpointsAsync(options, type);
            }
        }

        private static void EnsureAllEndpointsAsync(ControllerGeneratorOptions options, Type type)
        {
            var methods = options.ProxyContractProvider.GetMethodsToExpose(type);
            foreach (var methodInfo in methods)
            {
                if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
                {
                    throw new ArgumentException(
                        $"Method '{methodInfo.Name}' in '{type.Name}' interface should return '{nameof(Task)}' or '{nameof(Task)}<TData>'");
                }
            }
        }
    }
}
