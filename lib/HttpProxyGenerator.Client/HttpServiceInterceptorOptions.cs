using System.Collections.Generic;
using System.Reflection;

namespace HttpProxyGenerator.Client
{
    public class HttpServiceInterceptorOptions
    {
        public string ControllerRoute { get; set; }

        public Dictionary<MethodInfo, string> ApiRoutes { get; set; }
    }
}
