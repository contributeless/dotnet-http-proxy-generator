using System;
using System.Linq;
using System.Reflection;
using HttpProxyGenerator.Extensions;

namespace HttpProxyGenerator.Abstractions
{
    public class DefaultProxyNamingConventionProvider: IProxyNamingConventionProvider
    {
        public string GetControllerNamespace(Type interfaceType)
        {
            return "Test.Controllers.CodeGen";
        }

        public string GetGeneratedControllerName(Type interfaceType)
        {
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            var name = GetContractBaseName(interfaceType);
            return $"{name}Controller";
        }

        public string GetServiceFieldName(Type interfaceType)
        {
            return "_service";
        }

        public string GetServiceConstructorParameterName(Type interfaceType)
        {
            return "service";
        }

        public string GetApiMethodModelParameterName(MethodInfo method)
        {
            return "model";
        }

        public string GetParameterModelTypeName(Type interfaceType, MethodInfo method)
        {
            return $"{GetContractBaseName(interfaceType)}{GetApiEndpointWithoutAsyncPostfix(method.Name)}ParameterModel";
        }

        public string GetControllerRoute(Type targetInterface)
        {
            var name = GetContractBaseName(targetInterface);
            return $"api/{name.ToKebabCase()}";
        }

        public string GetEndpointRoute(MethodInfo method)
        {
            return GetApiEndpointWithoutAsyncPostfix(method.Name).ToKebabCase();
        }

        private string GetApiEndpointWithoutAsyncPostfix(string methodName)
        {
            const string asyncPostfix = "Async";
            if (methodName.EndsWith(asyncPostfix))
            {
                return methodName[..^asyncPostfix.Length];
            }

            return methodName;
        }

        private string GetContractBaseName(Type interfaceType)
        {
            var urlFriendly = interfaceType.GetUrlFriendlyName();
            var trimmedInterfaceName = GetInterfaceNameWithoutLeadingI(urlFriendly);

            return trimmedInterfaceName;
        }

        private string GetInterfaceNameWithoutLeadingI(string interfaceName)
        {
            // do not transform names IE, EG, etc. names
            if (interfaceName.Length < 2)
            {
                return interfaceName;
            }

            // second letter should be upper-cased
            if (!char.IsUpper(interfaceName[1]))
            {
                return interfaceName;
            }

            if (interfaceName.First().Equals('I'))
            {
                return interfaceName[1..interfaceName.Length];
            }

            return interfaceName;
        }
    }
}
