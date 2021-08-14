using System.Threading.Tasks;
using InterfacesLibrary;

namespace WebApp.Controllers
{
    public class DataFetcher: IDataFetcher
    {
        public Task<string> GetData(string test1, int test2)
        {
            return Task.FromResult("ok");
        }
    }
    public class Data2Fetcher: IData2Fetcher
    {
        public Task<string> GetData(string test1, int test2)
        {
            return Task.FromResult("ok");
        }
    }
    public class Data3Fetcher: IData3Fetcher
    {
        public Task<string> GetData(string test1, int test2)
        {
            return Task.FromResult("ok");
        }
    }
    public class Data5Fetcher: IData5Fetcher
    {
        public Task<string> GetData(string test1, int test2)
        {
            return Task.FromResult("ok");
        }
    }
    public class Data7Fetcher: IData7Fetcher
    {
        public Task<string> GetData(string test1, int test2)
        {
            return Task.FromResult("ok");
        }

        public Task<string> GetData2(string test1, int test2)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetDat5(string test1, int test2)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetDat8(string test1, int test2)
        {
            throw new System.NotImplementedException();
        }
    }
}
