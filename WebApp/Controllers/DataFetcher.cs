using System;
using System.Threading.Tasks;
using InterfacesLibrary;

namespace WebApp.Controllers
{
    public class DataFetcher: IDataFetcher
    {
        public Task<string> GetBaseData(string test1, int test2)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSimpleDataAsync(string test1, int test2)
        {
            throw new NotImplementedException();
        }
    }
}
