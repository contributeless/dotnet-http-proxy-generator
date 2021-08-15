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

        public Task<string> SendComplexGenericData(SampleGenericData<SampleData> data)
        {
            return Task.FromResult($"Complex info received {data.SampleComplexInfo.Info1}");
        }

        public Task<string> SendComplexGenericData(SampleGenericData<SampleGenericData<SampleData>> data)
        {
            return Task.FromResult($"Complex info received {data.SampleInfo}");
        }

        public Task<SampleGenericData<SampleData>> GetComplexGenericData()
        {
            return Task.FromResult(new SampleGenericData<SampleData>()
            {
                SampleComplexInfo = new SampleData()
                {
                    Info1 = "test",
                    Info2 = DateTime.UtcNow
                },
                SampleInfo = "test2"
            });
        }
    }
}
