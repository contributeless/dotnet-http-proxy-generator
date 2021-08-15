using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttpProxyGenerator.Common.Extensions;

namespace HttpProxyGenerator.Common.Abstractions
{
    public abstract class ProxyNamingConventionProviderBase: IProxyNamingConventionProvider
    {
        private const string AsyncPostfix = "Async";
        
        public virtual string GetControllerRoute(Type targetInterface)
        {
            var name = GetContractBaseName(targetInterface);
            return $"api/{name.ToKebabCase()}";
        }

        public virtual string GetEndpointRoute(MethodInfo method, string uniqueEndpointContractName)
        {
            return GetApiEndpointWithoutAsyncPostfix(uniqueEndpointContractName).ToKebabCase();
        }

        public virtual IDictionary<string, MethodInfo> GetUniqueEndpointContractNames(IEnumerable<MethodInfo> methods)
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

        protected virtual string GetApiEndpointWithoutAsyncPostfix(string methodName)
        {
            if (methodName.EndsWith(AsyncPostfix))
            {
                return methodName[..^AsyncPostfix.Length];
            }

            return methodName;
        }

        protected virtual string GetContractBaseName(Type interfaceType)
        {
            var urlFriendly = interfaceType.GetUrlFriendlyName();
            var trimmedInterfaceName = GetInterfaceNameWithoutLeadingI(urlFriendly);

            return trimmedInterfaceName;
        }

        protected virtual string GetInterfaceNameWithoutLeadingI(string interfaceName)
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
