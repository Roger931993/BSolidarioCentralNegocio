using CentralNegocio.Domain.Common.Dapper;
using CentralNegocio.Domain.Interfaces.Dapper;
using System.Data;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class ResponseDataSetSpExecution<TDomainResponse> : IResponseDataSetSpExecution<TDomainResponse> where TDomainResponse : EntitySp
  {
    public TDomainResponse EntityDomainResponse { get; private set; }
    public DataSet DataSetResult { get; private set; }
    public void SetDataSet(DataSet dataset)
    {
      DataSetResult = dataset;
    }
    public void SetEntity(TDomainResponse entity)
    {
      EntityDomainResponse = entity;
    }
  }
}
