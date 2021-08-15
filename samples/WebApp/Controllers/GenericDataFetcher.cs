using System;
using System.Threading.Tasks;
using InterfacesLibrary;

namespace WebApp.Controllers
{
    public class GenericDataFetcher : IGenericDataFetcher<SampleData>
    {
        public Task<SampleData> GetCustomDataAsync()
        {
            return Task.FromResult(new SampleData()
            {
                Info1 = "Sample data",
                Info2 = DateTime.UtcNow
            });
        }
    }
}