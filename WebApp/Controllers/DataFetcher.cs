using System;
using System.Threading.Tasks;
using InterfacesLibrary;

namespace WebApp.Controllers
{
    public class DataFetcher: IDataFetcher
    {
        public Task<string> GetData(string test1, int test2)
        {
            return Task.FromResult("ok 1");
        }

        public Task<string> GetBaseData(string test1, int test2)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSuperBaseData(string test1, int test2)
        {
            throw new NotImplementedException();
        }
    }

    public class Data2Fetcher: IData2Fetcher
    {
        public Task<string> GetDataAsync(string test1, int test2)
        {
            return Task.FromResult("ok 2");
        }
    }

    public class Data7Fetcher: IData7Fetcher
    {
        public Task<string> GetData(string test1, int test2)
        {
            return Task.FromResult("ok");
        }

        public Task<OutData> GetData(InData test1, int test2)
        {
            throw new NotImplementedException();
        }

        public Task<OutData> GetData2(string test1, int test2)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetDat5(InData test1, int test2)
        {
            throw new NotImplementedException();
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
