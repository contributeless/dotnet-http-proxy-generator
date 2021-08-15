using System;
using System.Collections.Generic;
using HttpProxyGenerator.Abstractions;

namespace HttpProxyGenerator
{
    public class ControllerGeneratorOptions
    {
        internal IList<Type> InterfacesToExpose { get; }

        internal IProxyNamingConventionProvider NamingConventionProvider { get; set; }

        internal IProxyContractProvider ProxyContractProvider { get; set; }

        public ControllerGeneratorOptions()
        {
            InterfacesToExpose = new List<Type>();
            NamingConventionProvider = new DefaultProxyNamingConventionProvider();
            ProxyContractProvider = new DefaultProxyContractProvider();
        }
    }
}
