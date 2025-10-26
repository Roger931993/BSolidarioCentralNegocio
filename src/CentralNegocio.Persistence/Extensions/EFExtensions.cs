using CentralNegocio.Application.Interfaces.Persistence;
using CentralNegocio.Persistence.Contexts;
using CentralNegocio.Persistence.Repositories.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralNegocio.Persistence.Extensions
{
    public static class PersistenceServiceExtensions
    {
        public static IServiceCollection AddEFService(this IServiceCollection services, IConfiguration configuration)
        {
            #region SQL
            // Configura la conexión a la base de datos.
            //services.AddDbContext<ApplicationDbContext>(options =>
            //{
            //    options.UseSqlServer(configuration.GetConnectionString("Stamp"));
            //});            
            #endregion

            #region Postgres
            // Configura la conexión a la base de datos.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Clientes"), npgsqlOption =>
                {
                    npgsqlOption.CommandTimeout(60);
                });                
            });


            // Configura la conexión a la base de datos.
            services.AddDbContext<LoggDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Logs"), npgsqlOption =>
                {
                    npgsqlOption.CommandTimeout(60);
                });
                //.EnableSensitiveDataLogging();
            });

            //services.AddDbContextFactory<LoggDbContext>(delegate (DbContextOptionsBuilder options)
            //{
            //    options.UseNpgsql(configuration.GetConnectionString("Logs"), delegate (NpgsqlDbContextOptionsBuilder assembly)
            //    {
            //        assembly.MigrationsAssembly("CentralNegocio.Persistence");
            //    });
            //});
            #endregion

            // Configura la conexión a la base de datos.
            services.AddScoped<IClienteRepository, ClienteRespository>();
            services.AddTransient<ILoggRepository, LoggRepository>();


            return services;
        }
    }
}
