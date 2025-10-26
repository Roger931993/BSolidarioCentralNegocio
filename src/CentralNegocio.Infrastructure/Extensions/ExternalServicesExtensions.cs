using CentralNegocio.Infrastructure.ExternalServices.Common.Api;
using CentralNegocio.Infrastructure.ExternalServices.Common.Wcf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralNegocio.Infrastructure.Extensions
{
    public static class ExternalServicesExtensions
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            IConfiguration configuration2 = configuration;

            #region Cadenas de conexion WCF
            IEnumerable<IConfigurationSection> children = configuration2.GetSection("ConexionHttp").GetSection("WcfServices").GetChildren();
            if (children.Count() > 0)
            {
                Dictionary<string, IWcfUrl> values = children.AsEnumerable().ToDictionary((IConfigurationSection a) => a.Key, (IConfigurationSection a) => CrearWcfObject(configuration2, a).Result);
                services.AddSingleton(new WcfConnectionDto(values));
            }
            #endregion

            #region Cadenas de conexion API/Rest
            IEnumerable<IConfigurationSection> children2 = configuration2.GetSection("ConexionHttp").GetSection("ApiServices").GetChildren();
            if (children2.Count() > 0)
            {
                Dictionary<string, IApiUrl> values2 = children2.AsEnumerable().ToDictionary((IConfigurationSection a) => a.Key, (IConfigurationSection a) => CrearApiObject(configuration2, a).Result);
                services.AddSingleton(new ApiConnectionDto(values2));
            }
            #endregion

            services.AddScoped(typeof(IWcfConnect), typeof(ConnectWcf));
            services.AddHttpClient();         

            return services;
        }

        private static async Task<IWcfUrl> CrearWcfObject(IConfiguration configuration, IConfigurationSection configurationSection)
        {
            string url = configuration.GetSection(configurationSection.Path + ":Url").Value!;
            TimeSpan timeout = TimeSpan.Parse(configuration.GetSection(configurationSection.Path + ":Timeout").Value!);
            string protocol = configuration.GetSection(configurationSection.Path + ":Protocol").Value!;
            WcfUrl result = new WcfUrl()
            {
                Url = url,
                TimeOut = timeout,
                Protocol = protocol
            };
            return Task.Run(() => result).Result;
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

    public class EmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }  // Nueva propiedad
    }

    public class AzureConfiguration
    {
        public string ClientId { get; set; }
        public string Tenant { get; set; }
        public string Instance { get; set; }
        public string SecretId { get; set; }
        public string FromEmail { get; set; }  // Nueva propiedad
    }
}
