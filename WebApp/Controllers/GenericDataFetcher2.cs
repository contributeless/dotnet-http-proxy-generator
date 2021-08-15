using System;
using System.Threading.Tasks;
using InterfacesLibrary;

namespace WebApp.Controllers
{
    public class GenericDataFetcher2 : IGenericDataFetcher<SampleData2>
    {
        public Task<SampleData2> GetCustomDataAsync()
        {
            return Task.FromResult(new SampleData2()
            {
                Info3 = "Sample data",
                Info4 = DateTime.UtcNow
            });
        }
    }
}