using System.Threading.Tasks;

namespace InterfacesLibrary.AnotherNamespace
{
    public interface IGenericDataFetcher<TCustom>
    {
        Task<TCustom> GetCustomDataAsync();
    }
}