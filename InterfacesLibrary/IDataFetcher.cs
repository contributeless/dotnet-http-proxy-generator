using System;
using System.Threading.Tasks;

namespace InterfacesLibrary
{
    public interface IDataFetcher: IBaseDataFetcher
    {
        Task<string> GetData(string test1, int test2);
    }

    public interface IBaseDataFetcher: ISuperBaseDataFetcher
    {
        Task<string> GetBaseData(string test1, int test2);
    }
    public interface ISuperBaseDataFetcher
    {
        Task<string> GetSuperBaseData(string test1, int test2);
    }

    public interface IData2Fetcher
    {
        Task<string> GetDataAsync(string test1, int test2);
    }
    public interface IData3Fetcher
    {
        Task<string> GetData(string test1, int test2);
    }
    public interface IData5Fetcher
    {
        Task<string> GetData(string test1, int test2);
    }
    public interface IData7Fetcher
    {
        Task<string> GetData(string test1, int test2);
        Task<OutData> GetData2(string test1, int test2);
        Task<string> GetDat5(InData test1, int test2);
        Task<string> GetDat8(string test1, int test2);
    }

    public class OutData
    {
        public string Pup { get; set; }
        public DateTime Trup { get; set; }
    }
    public class InData
    {
        public string Pup { get; set; }
        public DateTime Trup { get; set; }
    }

}
