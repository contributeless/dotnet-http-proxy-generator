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
    internal class HttpServiceCallInterceptor : IInterceptor
    {
        private readonly HttpClient _client;
        private readonly HttpServiceInterceptorOptions _options;

        public HttpServiceCallInterceptor(
            HttpClient client, HttpServiceInterceptorOptions options)
        {
            _client = client;
            _options = options;
        }

        public void Intercept(IInvocation invocation)
        {
            var content = GetRequestContent(invocation);

            var methodRoute = _options.ApiRoutes[invocation.Method];

            var uri = $"{_options.ControllerRoute}/{methodRoute}";

            if (invocation.Method.ReturnType.IsGenericType)
            {
                var responseType = invocation.Method.ReturnType.GetGenericArguments().Single();

                invocation.ReturnValue = typeof(HttpServiceCallInterceptor).GetMethod(nameof(PostData),
                        BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(responseType)
                    .Invoke(this, new object[] {uri, content});
            }
            else
            {
                invocation.ReturnValue = PostDataWithoutResponse(uri, content);
            }
        }

        private async Task PostDataWithoutResponse(string url, HttpContent content)
        {
            var result = await _client.PostAsync(url, content);

            result.EnsureSuccessStatusCode();
        }

        private async Task<TResponse> PostData<TResponse>(string url, HttpContent content) where TResponse: class
        {
            var responseType = typeof(TResponse);

            var result = await _client.PostAsync(url, content);
            
            result.EnsureSuccessStatusCode();

            var stringResponse = await result.Content.ReadAsStringAsync();

            if (responseType == typeof(string))
            {
                return stringResponse as TResponse;
            }

            return (TResponse)JsonConvert.DeserializeObject(stringResponse, responseType);
        }

        private static StringContent GetRequestContent(IInvocation invocation)
        {
            var arguments = new Dictionary<string, object>();

            var methodParameters = invocation.Method.GetParameters();
            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameter = methodParameters[i];
                var parameterValue = i < invocation.Arguments.Length ? invocation.Arguments[i] : null;

                arguments.Add(parameter.Name, parameterValue);
            }

            var content = new StringContent(JsonConvert.SerializeObject(arguments), Encoding.UTF8, "application/json");

            return content;
        }
    }
}