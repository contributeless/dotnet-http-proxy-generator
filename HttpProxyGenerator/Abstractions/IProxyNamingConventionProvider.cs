using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxyGenerator.Abstractions
{
    public interface IProxyNamingConventionProvider
    {
        string GetControllerNamespace(Type interfaceType);

        string GetGeneratedControllerName(Type interfaceType);

        string GetServiceFieldName(Type interfaceType);

        string GetServiceConstructorParameterName(Type interfaceType);

        string GetApiMethodModelParameterName(MethodInfo method);

        string GetParameterModelTypeName(Type interfaceType, MethodInfo method, string uniqueEndpointContractName);

        string GetControllerRoute(Type targetInterface);

        string GetEndpointRoute(MethodInfo method, string uniqueEndpointContractName);

        IDictionary<string, MethodInfo> GetUniqueEndpointContractNames(IEnumerable<MethodInfo> methods);
    }
}
