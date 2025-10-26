using CentralNegocio.Application.Common;
using CentralNegocio.Application.Interfaces;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CentralNegocio.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(ApplicationMappingProfile));

            services.AddScoped<IErrorCatalogException, ErrorCatalogException>();

            services.AddMediatR(gfc => gfc.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            services.AddScoped<IPermissionService, PermissionService>();

            return services;
        }
    }
}
