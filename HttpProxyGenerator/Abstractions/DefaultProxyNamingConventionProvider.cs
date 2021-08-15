using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttpProxyGenerator.Extensions;

namespace HttpProxyGenerator.Abstractions
{
    public class DefaultProxyNamingConventionProvider: IProxyNamingConventionProvider
    {
        private const string AsyncPostfix = "Async";

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

        public string GetParameterModelTypeName(Type interfaceType, MethodInfo method, string uniqueEndpointContractName)
        {
            return $"{GetContractBaseName(interfaceType)}{GetApiEndpointWithoutAsyncPostfix(uniqueEndpointContractName)}ParameterModel";
        }

        public string GetControllerRoute(Type targetInterface)
        {
            var name = GetContractBaseName(targetInterface);
            return $"api/{name.ToKebabCase()}";
        }

        public string GetEndpointRoute(MethodInfo method, string uniqueEndpointContractName)
        {
            return GetApiEndpointWithoutAsyncPostfix(uniqueEndpointContractName).ToKebabCase();
        }

        public IDictionary<string, MethodInfo> GetUniqueEndpointContractNames(IEnumerable<MethodInfo> methods)
        {
            var methodsGroupedByName = methods.GroupBy(x => x.Name).ToArray();

            var singularMethods = methodsGroupedByName.Where(x => x.Count() == 1).SelectMany(x => x);

            var result = singularMethods.ToDictionary(x => x.Name, x => x);

            var overloadedMethodGroups = methodsGroupedByName.Where(x => x.Count() > 1)
                .ToArray();

            foreach (var overloadedMethodGroup in overloadedMethodGroups)
            {
                var overloadedMethods = overloadedMethodGroup.ToArray();
                for (var i = 0; i < overloadedMethods.Length; i++)
                {
                    var method = overloadedMethods[i];

                    var withoutPostfix = GetApiEndpointWithoutAsyncPostfix(method.Name);

                    result.Add(
                        withoutPostfix.Length != method.Name.Length
                            ? $"{withoutPostfix}{i}{AsyncPostfix}"
                            : $"{method.Name}{i}", method);
                }
            }

            return result;
        }

        private string GetApiEndpointWithoutAsyncPostfix(string methodName)
        {
            if (methodName.EndsWith(AsyncPostfix))
            {
                return methodName[..^AsyncPostfix.Length];
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
