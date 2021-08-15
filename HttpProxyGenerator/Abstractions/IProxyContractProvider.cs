using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HttpProxyGenerator.Abstractions
{
    public interface IProxyContractProvider
    {
        IEnumerable<MethodInfo> GetMethodsToExpose(Type interfaceType);

        Type GetBaseControllerType(Type interfaceType);
    }
}
