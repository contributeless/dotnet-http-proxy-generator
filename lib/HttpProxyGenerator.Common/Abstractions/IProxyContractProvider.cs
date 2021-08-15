using System;
using System.Collections.Generic;
using System.Reflection;

namespace HttpProxyGenerator.Common.Abstractions
{
    public interface IProxyContractProvider
    {
        IEnumerable<MethodInfo> GetMethodsToExpose(Type interfaceType);
    }
}
