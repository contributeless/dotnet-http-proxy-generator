using System;
using Microsoft.AspNetCore.Mvc;

namespace HttpProxyGenerator.Consumer.Abstractions
{
    public class DefaultProxyContractProvider : Common.Abstractions.ProxyContractProviderBase, IProxyContractProvider
    {
        public virtual Type GetBaseControllerType(Type interfaceType)
        {
            return typeof(ControllerBase);
        }
    }
}
