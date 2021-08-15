using System;
using System.Threading.Tasks;
using InterfacesLibrary;

namespace WebApp.Controllers
{
    public class DataFetcher: IDataFetcher
    {
        public Task<string> GetBaseData(string test1, int test2)
        {
            return Task.FromResult("base data");
        }

        public Task<string> GetSimpleDataAsync(string test1, int test2)
        {
            return Task.FromResult("simple data");
        }

        public Task<string> GetDataWithoutParametersAsync()
        {
            return Task.FromResult("without parameters data");
        }

        public Task SendWithoutResultAsync(string data)
        {
            return Task.CompletedTask;
        }

        public Task<SampleData> GetGenericData()
        {
            return Task.FromResult(new SampleData()
            {
                Info1 = "Generic data",
                Info2 = DateTime.UtcNow
            });
        }
        
        public Task<string> SendGenericData(SampleData data)
        {
            return Task.FromResult($"Passed {data.Info1} {data.Info2}");
        }

        public Task<string> GetOverloadedAsync(string data)
        {
            return Task.FromResult($"Overloaded method 1; data: {data}");
        }

        public Task<string> GetOverloadedAsync(int data)
        {
            return Task.FromResult($"Overloaded method 2; data: {data}");
        }
    }
}
