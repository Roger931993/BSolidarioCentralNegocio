using CentralNegocio.Domain.Interfaces.Dapper;
using Dapper;
using System.Data;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class DbContextDapperCommon : IDbContextDapperCommon
  {
    public readonly IDbConnection _connection;
    private IDbTransaction? _transaction = null;

    public DbContextDapperCommon(IDbConnection connection)
    {
      _connection = connection;
    }

    public async Task<int> ExecuteAddAsync(string query, DynamicParameters parameters)
    {
      return await ExecuteCrudAsync(query, parameters);
    }

    public async Task<int> ExecuteUpdateAsync(string query, DynamicParameters parameters)
    {
      return await ExecuteCrudAsync(query, parameters);
    }

    public async Task<int> ExecuteDeleteAsync(string query, DynamicParameters parameters)
    {
      return await ExecuteCrudAsync(query, parameters);
    }

    public async Task<IResultSpExecute> ExecuteSpReturnIntAsync(string spName, DynamicParameters parameters)
    {
      IResultSpExecute response = new ResultSpExecute();
      IResultSpExecute resultSpExcecute = response;
      IDbConnection connection = _connection;
      IDbTransaction transaction = _transaction;
      CommandType? commandType = CommandType.StoredProcedure;
      resultSpExcecute.SetResultSp(await connection.ExecuteAsync(spName, parameters, transaction, null, commandType));
      response.SetParameters(parameters);
      return response;
    }

    public async Task<int> ExcecuteQueryTextReturnIntAsync(string sqlText, int? commandTimeout = null, object param = null)
    {
      return await _connection.ExecuteAsync(sqlText, param, _transaction, commandTimeout, CommandType.Text);
    }

    public async Task<IResultSpExecuteDataSet> ExecuteSpReturnDataSetAsync(string spName, DynamicParameters parameters)
    {
      IResultSpExecuteDataSet response = new ResultSpExecuteDataSet();
      DataSet dataset = new DataSet();
      IDbConnection connection = _connection;
      IDbTransaction transaction = _transaction;
      CommandType? commandType = CommandType.StoredProcedure;
      SqlMapper.GridReader resultadoSp = await connection.QueryMultipleAsync(spName, parameters, transaction, null, commandType);
      while (!resultadoSp.IsConsumed)
      {
        List<object> tabla = resultadoSp.Read<object>().ToList();
        DataTable dataTableNew = new DataTable();
        foreach (dynamic item in tabla)
        {
          DataRow dataRow = dataTableNew.NewRow();
          foreach (object item2 in item)
          {
            KeyValuePair<string, object> campo = (KeyValuePair<string, object>)(dynamic)item2;
            if (!dataTableNew.Columns.Contains(campo.Key))
            {
              dataTableNew.Columns.Add(campo.Key, campo.Key.GetType());
            }
            dataRow[campo.Key] = campo.Value;
          }
          dataTableNew.Rows.Add(dataRow);
        }
        dataset.Tables.Add(dataTableNew);
      }
      response.SetResultSp(dataset);
      response.SetParameters(parameters);
      return response;
    }

    public async Task<IResultSpExecuteGridReader> ExecuteSpReturnGridReaderAsync(string spName, DynamicParameters parameters)
    {
      IResultSpExecuteGridReader response = new ResultSpExecuteGridReader();
      IDbConnection connection = _connection;
      IDbTransaction transaction = _transaction;
      CommandType? commandType = CommandType.StoredProcedure;
      SqlMapper.GridReader result = await connection.QueryMultipleAsync(spName, parameters, transaction, null, commandType);
      response.SetResultSp(result);
      response.SetParameters(parameters);
      return response;
    }


    public async Task<IResultSpExecuteGridReader> ExecuteFunctionReturnGridReaderAsync(string spName, DynamicParameters parameters)
    {
      List<string> param = parameters.ParameterNames.ToList().Where(x => !x.Equals("Retorno")).ToList();
      string strParam = string.Empty;
      foreach (string item in param)
      {
        strParam += $"{item},";
      }
      if (!string.IsNullOrEmpty(strParam))
        strParam = strParam.Remove(strParam.Length - 1);
      var sql = $"SELECT * FROM {spName}({strParam})";
      IResultSpExecuteGridReader response = new ResultSpExecuteGridReader();
      IDbConnection connection = _connection;      
      SqlMapper.GridReader result = await connection.QueryMultipleAsync(sql, parameters);
      response.SetResultSp(result);
      response.SetParameters(parameters);
      return response;
    }

    public async Task<IResultSpExecuteDataTable> ExecuteSpReturnDataTabeAsync(string spName, DynamicParameters parameters)
    {
      IResultSpExecuteDataTable response = new ResultSpExecuteDataTable();
      DataTable dataTable = new DataTable();
      IDbConnection connection = _connection;
      IDbTransaction transaction = _transaction;
      CommandType? commandType = CommandType.StoredProcedure;
      List<object> result = (await connection.QueryMultipleAsync(spName, parameters, transaction, null, commandType)).Read<object>().ToList();
      foreach (dynamic item in result)
      {
        DataRow dataRow = dataTable.NewRow();
        foreach (object item2 in item)
        {
          KeyValuePair<string, object> campo = (KeyValuePair<string, object>)(dynamic)item2;
          if (!dataTable.Columns.Contains(campo.Key))
          {
            dataTable.Columns.Add(campo.Key, campo.Key.GetType());
          }
          dataRow[campo.Key] = campo.Value;
        }
        dataTable.Rows.Add(dataRow);
      }

      response.SetResultSp(dataTable);
      response.SetParameters(parameters);
      return response;
    }

    public async Task<IEnumerable<TDomainEntity>> QueryAsync<TDomainEntity>(string sqlText, object param = null)
    {      
      return await _connection.QueryAsync<TDomainEntity>(sqlText, param);
    }

    public void BeginTransaccion()
    {
      _transaction = _connection.BeginTransaction();
    }

    public void CommitTransaccion()
    {
      try
      {
        _transaction?.Commit();
      }
      catch (Exception)
      {
        RollBackTransaction();
        throw;
      }
      finally
      {
        _transaction?.Dispose();
        _transaction = null;
      }
    }

    public void RollBackTransaction()
    {
      try
      {
        _transaction?.Rollback();
      }
      finally
      {
        _transaction?.Dispose();
        _transaction = null;
      }
    }

    public void Dispose()
    {
      _transaction?.Dispose();
      _transaction = null;
    }


    private async Task<int> ExecuteCrudAsync(string query, DynamicParameters parameters)
    {
      return await _connection.ExecuteAsync(query, parameters, _transaction);
    }
  }
}
