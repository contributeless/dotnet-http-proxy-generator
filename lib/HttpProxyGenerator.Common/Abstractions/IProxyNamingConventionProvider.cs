using System;
using System.Collections.Generic;
using System.Reflection;

namespace HttpProxyGenerator.Common.Abstractions
{
    public interface IProxyNamingConventionProvider
    {
        string GetControllerRoute(Type targetInterface);

        string GetEndpointRoute(MethodInfo method, string uniqueEndpointContractName);

        IDictionary<string, MethodInfo> GetUniqueEndpointContractNames(IEnumerable<MethodInfo> methods);
    }
}
