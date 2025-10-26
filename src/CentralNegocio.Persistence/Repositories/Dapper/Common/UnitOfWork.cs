using CentralNegocio.Domain.Common.Dapper;
using CentralNegocio.Domain.Interfaces.Dapper;
using System.Collections;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class UnitOfWork : IUnitOfWork
  {
    private Hashtable _repositories;
    private readonly DbContextDapperCommon _contextDapper;

    public UnitOfWork(DbContextDapperCommon contextDapper)
    {
      _contextDapper = contextDapper;
    }
    public IRepositoryCommand<TDomainEntity> RepositoryCommand<TDomainEntity>() where TDomainEntity : Entity
    {
      if (_contextDapper == null)
      {
        throw new ArgumentNullException("Context dapper is null");
      }
      if (_repositories == null)
      {
        _repositories = new Hashtable();
      }

      string name = typeof(TDomainEntity).Name;
      if (!_repositories.ContainsKey(name))
      {
        Type typeFromHandle = typeof(RepositoryCommand<>);
        object value = Activator.CreateInstance(typeFromHandle.MakeGenericType(typeof(TDomainEntity)), _contextDapper);
        _repositories.Add(name, value);
      }

      return (IRepositoryCommand<TDomainEntity>)_repositories[name];
    }

    public IRepositoryProcedures<TDomainEntity> RepositoryProcedure<TDomainEntity>() where TDomainEntity : EntitySp
    {
      if (_contextDapper == null)
      {
        throw new ArgumentNullException("Context dapper is null");
      }
      if (_repositories == null)
      {
        _repositories = new Hashtable();
      }

      string name = typeof(TDomainEntity).Name;
      if (!_repositories.ContainsKey(name))
      {
        Type typeFromHandle = typeof(RepositoryProcedures<>);
        object value = Activator.CreateInstance(typeFromHandle.MakeGenericType(typeof(TDomainEntity)), _contextDapper);
        _repositories.Add(name, value);
      }

      return (IRepositoryProcedures<TDomainEntity>)_repositories[name];
    }

    public void CommitTransaction()
    {
      _contextDapper.CommitTransaccion();
    }

    public void RollBackTransaction()
    {
      _contextDapper.RollBackTransaction();
    }

    public void BeginTransaccion()
    {
      _contextDapper.BeginTransaccion();
    }
  }

}
