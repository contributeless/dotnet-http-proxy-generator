using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HttpProxyGenerator.Common.Abstractions
{
    public abstract class ProxyContractProviderBase : IProxyContractProvider
    {
        public virtual IEnumerable<MethodInfo> GetMethodsToExpose(Type interfaceType)
        {
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            var baseInterfaces = interfaceType.GetInterfaces()

                //exclude system interfaces like IDisposable
                .Where(x => !(x.FullName?.StartsWith("System") ?? false));

            return interfaceType.GetMethods()
                .Concat(baseInterfaces.SelectMany(x => x.GetMethods()))
                .ToArray();
        }
    }
}
