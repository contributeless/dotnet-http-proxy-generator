using System;
using System.Reflection;

namespace HttpProxyGenerator.Consumer.Abstractions
{
    public interface IProxyNamingConventionProvider: Common.Abstractions.IProxyNamingConventionProvider
    {
        string GetControllerNamespace(Type interfaceType);

        string GetGeneratedControllerName(Type interfaceType);

        string GetServiceFieldName(Type interfaceType);

        string GetServiceConstructorParameterName(Type interfaceType);

        string GetApiMethodModelParameterName(MethodInfo method);

        string GetParameterModelTypeName(Type interfaceType, MethodInfo method, string uniqueEndpointContractName);
    }
}
