using System;
using System.Net.Http;
using System.Threading.Tasks;
using HttpProxyGenerator.Client;
using HttpProxyGenerator.Client.Abstractions;
using InterfacesLibrary;

namespace ConsoleApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new HttpClient {BaseAddress = new Uri("http://localhost:5000")};

            var generator = new HttpClientProxyGenerator();

            var proxy = generator.CreateProxy<IDataFetcher>(client, new DefaultProxyContractProvider(), new DefaultProxyNamingConventionProvider());

            var result = await proxy.GetBaseData("test", 1);
            Console.WriteLine(result);

            var genericData = await proxy.GetGenericData();
            Console.WriteLine(genericData.Info1);

            var resultSendGenericData = await proxy.SendGenericData(new SampleData());
            Console.WriteLine(resultSendGenericData);

            var simpleData = await proxy.GetSimpleDataAsync("test", 1);
            Console.WriteLine(simpleData);

            var withoutParameters = await proxy.GetDataWithoutParametersAsync();
            Console.WriteLine(withoutParameters);

             await proxy.SendWithoutResultAsync("test");

            var overloaded1 = await proxy.GetOverloadedAsync("test");
            Console.WriteLine(overloaded1);

            var overloaded2 = await proxy.GetOverloadedAsync(1);
            Console.WriteLine(overloaded2);

            var complexGenericResult = await proxy.SendComplexGenericData(new SampleGenericData<SampleData>()
            {
                SampleComplexInfo = new SampleData() { Info1 = "test"}
            });
            Console.WriteLine(complexGenericResult);

            var veryComplexGenericResult = await proxy.SendComplexGenericData(new SampleGenericData<SampleGenericData<SampleData>>()
            {
                SampleComplexInfo = new SampleGenericData<SampleData>()
                {
                    SampleComplexInfo = new SampleData()
                }
            });
            Console.WriteLine(veryComplexGenericResult);

            var complexGenericData = await proxy.GetComplexGenericData();
            Console.WriteLine(complexGenericData.SampleInfo);
        }
    }
}
