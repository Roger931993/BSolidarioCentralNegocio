using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using Microsoft.Extensions.Caching.Memory;

namespace CentralNegocio.Infrastructure.Caching.Memory
{
    public class MemoryCacheLocalService : IMemoryCacheLocalService
    {

        private readonly IMemoryCache _memoryCache;

        public MemoryCacheLocalService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<DataCacheLocal> GetCachedData(string key)
        {
            DataCacheLocal objCache = _memoryCache.Get<DataCacheLocal>(key)!;
            return objCache;
        }

        public async Task<T> GetCachedData<T>(string key) where T : class
        {
            return _memoryCache.Get<T>(key);
        }

        public async Task<object> GetCachedDataObject(string key)
        {
            return _memoryCache.Get(key);
        }


        public async Task SetCachedData<T>(string key, T value) where T : class
        {
            _memoryCache.Set<T>(key, value);
        }



        public async Task SetCachedData(string key, DataCacheLocal value)
        {
            _memoryCache.Set(key, value);
        }

        public async Task SetCacheObject(string key, object value)
        {
            _memoryCache.Set<object>(key, value);
        }

        public async Task DeleteCacheData(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
