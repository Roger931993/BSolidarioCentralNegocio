using CentralNegocio.Domain.Common;

namespace CentralNegocio.Application.Interfaces.Infraestructure
{
    public interface IMemoryCacheLocalService
    {
        Task<DataCacheLocal> GetCachedData(string key);
        Task SetCachedData(string key, DataCacheLocal value);
        Task DeleteCacheData(string key);
        Task<T> GetCachedData<T>(string key) where T : class;
        Task SetCachedData<T>(string key, T value) where T : class;

        Task<object> GetCachedDataObject(string key);
        Task SetCacheObject(string key, object value);
    }
}
