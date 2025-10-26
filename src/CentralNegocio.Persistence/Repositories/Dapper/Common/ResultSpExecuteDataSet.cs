using CentralNegocio.Domain.Interfaces.Dapper;
using Dapper;
using System.Data;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class ResultSpExecuteDataSet : IResultSpExecuteDataSet
  {
    public DataSet ResultSp { get; private set; }
    public DynamicParameters Parameters { get; private set; }

    public void SetResultSp(DataSet value)
    {
      ResultSp = value;
    }

    public void SetParameters(DynamicParameters value)
    {
      Parameters = value;
    }
  }
}
