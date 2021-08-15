using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace HttpProxyGenerator.Client
{
    public class HttpClientProxyGenerator
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        public TInterface CreateProxy<TInterface>(HttpClient client)
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType.Name} should be interface");
            }

            return (TInterface)Generator.CreateInterfaceProxyWithoutTarget(interfaceType, new HttpServiceCallInterceptor(client, interfaceType));
        }

        private class HttpServiceCallInterceptor : IInterceptor
        {
            private readonly HttpClient _client;
            private readonly Type _interfaceType;

            public HttpServiceCallInterceptor(HttpClient client, Type interfaceType)
            {
                _client = client;
                _interfaceType = interfaceType;
            }

            public void Intercept(IInvocation invocation)
            {
                var methodName = invocation.Method.Name;

                var arguments = new Dictionary<string, object>();

                var methodParameters = invocation.Method.GetParameters();
                for (var i = 0; i < methodParameters.Length; i++)
                {
                    var parameter = methodParameters[i];
                    var parameterValue = i < invocation.Arguments.Length ? invocation.Arguments[i] : null;

                    arguments.Add(parameter.Name, parameterValue);
                }

                var content = new StringContent(JsonConvert.SerializeObject(arguments), Encoding.UTF8, "application/json");

                var responseType = invocation.Method.ReturnType.GetGenericArguments().Single();

                invocation.ReturnValue = typeof(HttpServiceCallInterceptor).GetMethod(nameof(PostData),
                        BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(responseType)
                    .Invoke(this,
                        new object[]
                            {$"/{_interfaceType.Name}Controller/{methodName}", content, responseType});
            }

            private async Task<TResponse> PostData<TResponse>(string url, StringContent content, Type returnType) where TResponse: class
            {
               var result = await _client.PostAsync(url, content);

               var stringResponse = await result.Content.ReadAsStringAsync();

               if (returnType == typeof(string))
               {
                   return stringResponse as TResponse;
               }

               return (TResponse)JsonConvert.DeserializeObject(stringResponse, returnType);
            }
        }
    }
}
