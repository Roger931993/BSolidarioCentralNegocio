using CentralNegocio.Infrastructure.Extensions;
using CentralNegocio.Infrastructure.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralNegocio.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(typeof(InfrastructureMappingProfile).Assembly);
            services.AddMemoryCache(config);
            services.AddRedis(config);
            services.AddExternalServices(config);
            services.AddInternalServices(config);

            return services;
        }
    }
}
