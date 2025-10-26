using CentralNegocio.Domain.Interfaces.Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class DatabaseConnectPostgres : IDatabaseConnect, IDisposable
    {
        private readonly Dictionary<string, IDbConnection> _connection = new Dictionary<string, IDbConnection>();
        private readonly IConfiguration _configuration;
        private bool _disposed = false;

        public DatabaseConnectPostgres(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection(string coonectionName)
        {
            if (_connection.TryGetValue(coonectionName, out IDbConnection value))
            {
                if (value.State == ConnectionState.Closed)
                {
                    value.Open();
                }

                return value;
            }

            string connectionString = _configuration.GetConnectionString(coonectionName);
            value = new NpgsqlConnection(connectionString);
            value.Open();
            _connection.Add(coonectionName, value);
            return value;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                foreach (IDbConnection item in _connection.Values)
                {
                    if (item.State != 0)
                    {
                        item.Close();
                    }
                    item.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
