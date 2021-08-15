using System;

namespace HttpProxyGenerator.Consumer.Abstractions
{
    public interface IProxyContractProvider : Common.Abstractions.IProxyContractProvider
    {
        Type GetBaseControllerType(Type interfaceType);
    }
}
