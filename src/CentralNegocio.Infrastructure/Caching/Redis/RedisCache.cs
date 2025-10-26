using CentralNegocio.Application.Interfaces.Infraestructure;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace CentralNegocio.Infrastructure.Caching.Redis
{
    public class RedisCache : IRedisCache
    {
        private readonly IDistributedCache _cache;

        public RedisCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetAsync<T>(string key, T value, int TimeMinutes = 60)
        {

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                  .SetAbsoluteExpiration(TimeSpan.FromHours(TimeMinutes));

            var jsonString = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            await _cache.SetAsync(key, bytes, options);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes == null)
            {
                return default;
            }

            var jsonString = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(jsonString)!;
        }
    }
}
