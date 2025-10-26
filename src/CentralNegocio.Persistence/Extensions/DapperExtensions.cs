using CentralNegocio.Application.Interfaces.Persistence;
using CentralNegocio.Domain.Interfaces.Dapper;
using CentralNegocio.Persistence.Contexts;
using CentralNegocio.Persistence.Repositories.Dapper;
using CentralNegocio.Persistence.Repositories.Dapper.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralNegocio.Persistence.Extensions
{
    public static class DapperExtensions
    {
        public static IServiceCollection AddDapperService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDatabaseConnect, DatabaseConnectPostgres>();
            services.AddScoped(typeof(IRepositoryCommand<>), typeof(RepositoryCommand<>));
            services.AddScoped(typeof(IRepositoryProcedures<>), typeof(RepositoryProcedures<>));
            services.AddScoped(typeof(IRepositoryExecute<>), typeof(RepositoryExecute<>));

            services.AddDbContextDapper<ClientesContextCommand>(options => options.ConnectionString = configuration.GetConnectionString("Clientes")!);
            services.AddScoped(typeof(IUnitOfWorkStamp), typeof(UnitOfWorkStamp));           


            return services;
        }

        public static IServiceCollection AddDbContextDapper<TContext>(this IServiceCollection services, Action<DbContextDapperOptions> optionAction, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped, ServiceLifetime optionLifeTime = ServiceLifetime.Scoped) where TContext : class, IDbContextDapperCommon
        {
            if (optionAction == null)
            {
                throw new ArgumentNullException("optionsAction");
            }
            DbContextDapperOptions options = new DbContextDapperOptions();
            optionAction(options);
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                throw new ArgumentException("Connectionstring is empty");
            }

            services.Add(new ServiceDescriptor(typeof(TContext), typeof(TContext), contextLifeTime));
            services.Add(new ServiceDescriptor(typeof(DbContextDapperOptions), (IServiceProvider provider) => options, optionLifeTime));
            return services;
        }
    }
}
