using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxyGenerator
{
    public class ControllerGeneratorOptions
    {
        internal IList<Type> InterfacesToExpose { get; }

        public ControllerGeneratorOptions()
        {
            InterfacesToExpose = new List<Type>();
        }

        public void RegisterInterfaceToExpose<TInterface>()
        {
            var type = typeof(TInterface);

            ValidateType(type);
            ValidateInterfaceMethods(type);

            InterfacesToExpose.Add(type);
        }

        private void ValidateType(Type type)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException($"{type.Name} is not interface");
            }
        }

        private void ValidateInterfaceMethods(Type type)
        {
            //TODO
        }
    }
}
