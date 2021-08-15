using System;
using System.Reflection;

namespace HttpProxyGenerator.Consumer.Abstractions
{
    public class DefaultProxyNamingConventionProvider: Common.Abstractions.ProxyNamingConventionProviderBase, IProxyNamingConventionProvider
    {
        public virtual string GetControllerNamespace(Type interfaceType)
        {
            return $"{interfaceType.Namespace}.CodeGen";
        }

        public virtual string GetGeneratedControllerName(Type interfaceType)
        {
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            var name = GetContractBaseName(interfaceType);
            return $"{name}Controller";
        }

        public virtual string GetServiceFieldName(Type interfaceType)
        {
            return "_service";
        }

        public virtual string GetServiceConstructorParameterName(Type interfaceType)
        {
            return "service";
        }

        public virtual string GetApiMethodModelParameterName(MethodInfo method)
        {
            return "model";
        }

        public virtual string GetParameterModelTypeName(Type interfaceType, MethodInfo method, string uniqueEndpointContractName)
        {
            return $"{GetContractBaseName(interfaceType)}{GetApiEndpointWithoutAsyncPostfix(uniqueEndpointContractName)}ParameterModel";
        }
    }
}
