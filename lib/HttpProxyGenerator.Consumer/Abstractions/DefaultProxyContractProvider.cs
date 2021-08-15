using System;
using System.Collections.Generic;
using System.Reflection;
using HttpProxyGenerator.Consumer.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HttpProxyGenerator.Consumer.Abstractions
{
    public class DefaultProxyContractProvider : IProxyContractProvider
    {
        public IEnumerable<MethodInfo> GetMethodsToExpose(Type interfaceType)
        {
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            return interfaceType.GetInterfaceMethods();
        }

        public Type GetBaseControllerType(Type interfaceType)
        {
            return typeof(ControllerBase);
        }
    }
}
