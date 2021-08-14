using System.Threading.Tasks;

namespace InterfacesLibrary
{
    public interface IDataFetcher
    {
        Task<string> GetData(string test1, int test2);
    }
    public interface IData2Fetcher
    {
        Task<string> GetData(string test1, int test2);
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
        Task<string> GetData2(string test1, int test2);
        Task<string> GetDat5(string test1, int test2);
        Task<string> GetDat8(string test1, int test2);
    }
}
