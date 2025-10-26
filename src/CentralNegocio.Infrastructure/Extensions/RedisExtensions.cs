using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Infrastructure.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralNegocio.Infrastructure.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("Redis").GetValue<string>("ConnectionString");
            });
            services.AddTransient<IRedisCache, RedisCache>();

            return services;
        }
    }
}
