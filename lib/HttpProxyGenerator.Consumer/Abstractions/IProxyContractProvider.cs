using System;
using System.Collections.Generic;
using System.Reflection;

namespace HttpProxyGenerator.Consumer.Abstractions
{
    public interface IProxyContractProvider
    {
        IEnumerable<MethodInfo> GetMethodsToExpose(Type interfaceType);

        Type GetBaseControllerType(Type interfaceType);
    }
}
