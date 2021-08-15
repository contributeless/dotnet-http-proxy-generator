using System.Threading.Tasks;

namespace InterfacesLibrary
{
    public interface IBaseDataFetcher
    {
        Task<string> GetBaseData(string test1, int test2);
    }

    public interface IBaseGenericDataFetcher<TData>
    {
        Task<TData> GetGenericData();

        Task<string> SendGenericData(TData data);
    }

    public interface IDataFetcher: IBaseDataFetcher, IBaseGenericDataFetcher<SampleData>
    {
        Task<string> GetSimpleDataAsync(string test1, int test2);

        Task<string> GetDataWithoutParametersAsync();

        Task SendWithoutResultAsync(string data);

        Task<string> GetOverloadedAsync(string data);

        Task<string> GetOverloadedAsync(int data);
    }
}
