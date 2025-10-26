using CentralNegocio.Domain.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Marken.DataAccess.Common;

public class DatabaseConnectSql : IDatabaseConnect, IDisposable
{
  private readonly Dictionary<string, IDbConnection> _connection = new Dictionary<string, IDbConnection>();
  private readonly IConfiguration _configuration;
  private bool _disposed = false;

  public DatabaseConnectSql(IConfiguration configuration)
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
    value = new SqlConnection(connectionString);
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
