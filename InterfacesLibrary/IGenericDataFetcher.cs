using System.Threading.Tasks;

namespace InterfacesLibrary
{
    public interface IGenericDataFetcher<TCustom>
    {
        Task<TCustom> GetCustomDataAsync();
    }
}