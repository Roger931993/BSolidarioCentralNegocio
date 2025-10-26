using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Infrastructure.Services;
using CentralNegocio.Infrastructure.Services.Common.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralNegocio.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            IConfiguration configuration2 = configuration;
            
            #region Cadenas de conexion API/Rest
            IEnumerable<IConfigurationSection> children2 = configuration2.GetSection("ConexionHttp").GetSection("ApiServices").GetChildren();
            if (children2.Count() > 0)
            {
                Dictionary<string, IApiUrl> values2 = children2.AsEnumerable().ToDictionary((IConfigurationSection a) => a.Key, (IConfigurationSection a) => CrearApiObject(configuration2, a).Result);
                services.AddSingleton(new ApiConnectionDto(values2));
            }
            #endregion
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            
            services.AddScoped(typeof(ICatalogService), typeof(CatalogService));
            services.AddScoped(typeof(IClientesService), typeof(ClientesService));
            services.AddScoped(typeof(ICuentasService), typeof(CuentasService));
            services.AddTransient(typeof(IAuthService), typeof(AuthService));

            return services;
        }
        private static async Task<IApiUrl> CrearApiObject(IConfiguration configuration, IConfigurationSection configurationSection)
        {
            string url = configuration.GetSection(configurationSection.Path + ":Url").Value!;
            TimeSpan timeout = TimeSpan.Parse(configuration.GetSection(configurationSection.Path + ":Timeout").Value!);
            string protocol = configuration.GetSection(configurationSection.Path + ":Protocol").Value!;
            ApiUrl result = new ApiUrl()
            {
                Url = url,
                TimeOut = timeout,
                Protocol = protocol
            };
            return Task.Run(() => result).Result;
        }

    }
}
