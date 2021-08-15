using System;
using System.Linq;
using System.Net.Http;
using Castle.DynamicProxy;
using HttpProxyGenerator.Common.Abstractions;

namespace HttpProxyGenerator.Client
{
    public class HttpClientProxyGenerator
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        public TInterface CreateProxy<TInterface>(HttpClient client, IProxyContractProvider contractProvider,
            IProxyNamingConventionProvider namingConventionProvider)
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType.Name} should be interface");
            }

            var methodsToExpose = contractProvider.GetMethodsToExpose(interfaceType);
            var contractNames = namingConventionProvider.GetUniqueEndpointContractNames(methodsToExpose);

            var apiRoutes = contractNames.ToDictionary(x => x.Value, x => namingConventionProvider.GetEndpointRoute(x.Value, x.Key));
            var controllerRoute = namingConventionProvider.GetControllerRoute(interfaceType);

            return (TInterface) Generator.CreateInterfaceProxyWithoutTarget(
                interfaceType,
                new HttpServiceCallInterceptor(client, new HttpServiceInterceptorOptions()
                {
                    ApiRoutes = apiRoutes,
                    ControllerRoute = controllerRoute
                }));
        }
    }
}
