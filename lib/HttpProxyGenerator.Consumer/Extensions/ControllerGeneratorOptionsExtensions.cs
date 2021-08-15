using System;
using System.Reflection;
using System.Threading.Tasks;
using HttpProxyGenerator.Consumer.Abstractions;

namespace HttpProxyGenerator.Consumer.Extensions
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
                var methods = options.ProxyContractProvider.GetMethodsToExpose(type);

                foreach (var methodInfo in methods)
                {
                    EnsureAllEndpointsAsync(methodInfo, type);
                    EnsureAllEndpointsNonGeneric(methodInfo, type);
                    EnsureAllEndpointsIsNotSpecial(methodInfo, type);
                }
            }
        }

        private static void EnsureAllEndpointsAsync(MethodInfo method, Type type)
        {
            if (!typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                throw new ArgumentException(
                    $"Method '{method.Name}' in '{type.Name}' interface should return '{nameof(Task)}' or '{nameof(Task)}<TData>'");
            }
        }

        private static void EnsureAllEndpointsNonGeneric(MethodInfo method, Type type)
        {
            if (method.IsGenericMethod)
            {
                throw new ArgumentException(
                    $"Method '{method.Name}' in '{type.Name}' interface should not be generic");
            }
        }
        private static void EnsureAllEndpointsIsNotSpecial(MethodInfo method, Type type)
        {
            if (method.IsSpecialName)
            {
                throw new ArgumentException(
                    $"Method '{method.Name}' in '{type.Name}' interface should not be special");
            }
        }
    }
}
