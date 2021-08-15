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

        string GetParameterModelTypeName(MethodInfo method);

        string GetControllerRoute(Type targetInterface);

        string GetEndpointRoute(MethodInfo method);
    }
}
