using System;
using System.Threading.Tasks;

namespace InterfacesLibrary
{
    public interface IDataFetcher: IBaseDataFetcher
    {
        Task<string> GetSimpleDataAsync(string test1, int test2);
    }

    public interface IBaseDataFetcher
    {
        Task<string> GetBaseData(string test1, int test2);
    }
}
