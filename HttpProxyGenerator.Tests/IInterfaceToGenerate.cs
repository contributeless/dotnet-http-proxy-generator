using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpProxyGenerator.Tests
{
    public interface IInterfaceToGenerate
    {
        Task<string> CreateModel(string test, int aza);

        Task CreateAnotherModelWithoutResult(string test, int aza);
    }
}
