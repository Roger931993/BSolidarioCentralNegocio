using CentralNegocio.Domain.Common.Dapper;
using CentralNegocio.Domain.Interfaces.Dapper;
using Dapper;
using System.Data;
using System.Reflection;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class RepositoryProcedures<TDomainEntity> : IRepositoryProcedures<TDomainEntity> where TDomainEntity : EntitySp
  {
    public readonly DbContextDapperCommon _context;
    private readonly IGetDataAnnotationValues<TDomainEntity> _getValuesDataAnnotation;

    public RepositoryProcedures(DbContextDapperCommon context)
    {
      _context = context;
      _getValuesDataAnnotation = new ServiceGetDataAnnotationValue<TDomainEntity>();
    }

    public async Task<TDomainEntity> ExecuteSpAsyncIntReturn(TDomainEntity entity, string? spName = null)
    {
      PropertyInfo[] properties = entity.GetType().GetProperties();
      DynamicParameters dynamicParameters = BuidlParametersFromEntity(entity, properties);
      string nombreSp = (string.IsNullOrEmpty(spName) ? _getValuesDataAnnotation.NombreTable : spName);
      UpdateEntityWithOutputParameters(entity, (await _context.ExecuteSpReturnIntAsync(nombreSp, dynamicParameters)).Parameters, properties);
      return entity;
    }

    //public async Task<TDomainEntity> ExecuteSpCustomTypeAsynIntReturn(TDomainEntity entity, object objectParameters, string? spName = null)
    //{
    //  PropertyInfo[] properties = entity.GetType().GetProperties();
    //  DynamicParameters dynamicParameters = BuidlParametersFromEntity(entity, properties);
    //  string nombreSp = (string.IsNullOrEmpty(spName) ? _getValuesDataAnnotation.NombreTable : spName);
    //  UpdateEntityWithOutputParameters(entity, (await _context.ExecuteSpCustomTypeAsynIntReturn(nombreSp, objectParameters, dynamicParameters)).Parameters, properties);
    //  return entity;
    //}

    public async Task<TDomainEntity> ExecuteSpAsync(TDomainEntity entity)
    {
      TDomainEntity entity2 = entity;
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      _getValuesDataAnnotation.GetSpDataAnnotationValues();
      DynamicParameters parameters = new DynamicParameters();
      foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
      {
        item.ValueCampo = (from v in properties
                           where v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal)
                           select v.GetValue(entity2)).First();

        parameters.Add("@" + item.NombreCampo, item.ValueCampo, item.DbValueType, item.Direction);
      }
      IResultSpExecute resp = await _context.ExecuteSpReturnIntAsync(_getValuesDataAnnotation.NombreTable, parameters);
      List<IAtributosPropiedad> outputParams = _getValuesDataAnnotation.ListaDetallesAtributos.Where((p) => p.Direction.Equals(ParameterDirection.Output) || p.Direction.Equals(ParameterDirection.ReturnValue)).ToList();
      foreach (IAtributosPropiedad item in outputParams)
      {
        item.ValueCampo = resp.Parameters.Get<object>("@" + item.NombreCampo);
      }

      PropertyInfo[] prop2 = (from p in entity2.GetType().GetProperties()
                              from op in outputParams
                              where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                              select p).ToArray();

      PropertyInfo[] array = prop2;
      foreach (PropertyInfo prop in array)
      {
        object value = (from p in outputParams
                        where prop.Name == p.NombreCampoClase
                        select p.ValueCampo).First();
        prop.SetValue(entity2, value);
      }

      return entity2;
    }

    public async Task<IResponseDataSetSpExecution<TDomainEntity>> ExecuteSpAsyncDataSetReturn(TDomainEntity entity)
    {
      TDomainEntity entity2 = entity;
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      IResponseDataSetSpExecution<TDomainEntity> response = new ResponseDataSetSpExecution<TDomainEntity>();
      _getValuesDataAnnotation.GetDataAnnotationValues();
      DynamicParameters parameters = new DynamicParameters();
      foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
      {
        item.ValueCampo = (from v in properties
                           where v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal)
                           select v.GetValue(entity2)).First();

        parameters.Add("@" + item.NombreCampo, item.ValueCampo, item.DbValueType, item.Direction);
      }
      IResultSpExecuteDataSet resp = await _context.ExecuteSpReturnDataSetAsync(_getValuesDataAnnotation.NombreTable, parameters);
      List<IAtributosPropiedad> outputParams = _getValuesDataAnnotation.ListaDetallesAtributos.Where((p) => p.Direction.Equals(ParameterDirection.Output) || p.Direction.Equals(ParameterDirection.ReturnValue)).ToList();
      if (outputParams.Count() == 1)
      {
        foreach (IAtributosPropiedad item in outputParams)
        {
          item.ValueCampo = resp.Parameters.Get<object>("@" + item.NombreCampo);
        }

        PropertyInfo prop = (from p in entity2.GetType().GetProperties()
                             from op in outputParams
                             where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                             select p).First();

        object value2 = (from p in outputParams
                         where prop.Name == p.NombreCampoClase
                         select p.ValueCampo).First();
        prop.SetValue(entity2, value2);
      }
      if (outputParams.Count > 1)
      {
        foreach (IAtributosPropiedad item in outputParams)
        {
          item.ValueCampo = resp.Parameters.Get<object>("@" + item.NombreCampo);
        }
        PropertyInfo[] prop2 = (from p in entity2.GetType().GetProperties()
                                from op in outputParams
                                where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                                select p).ToArray();
        PropertyInfo[] array = prop2;
        foreach (PropertyInfo prop in array)
        {
          object value = (from p in outputParams
                          where prop.Name == p.NombreCampoClase
                          select p.ValueCampo).First();
          prop.SetValue(entity2, value);
        }

      }

      response.SetEntity(entity2);
      response.SetDataSet(resp.ResultSp);
      return response;
    }

    public async Task<IResponseDataSetSpExecution<TDomainEntity>> ExecuteSpAsyncDataTableReturn(TDomainEntity entity)
    {
      TDomainEntity entity2 = entity;
      IResponseDataSetSpExecution<TDomainEntity> response = new ResponseDataSetSpExecution<TDomainEntity>();
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      _getValuesDataAnnotation.GetDataAnnotationValues();
      DynamicParameters parameters = new DynamicParameters();
      foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
      {
        item.ValueCampo = (from v in properties
                           where v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal)
                           select v.GetValue(entity2)).First();

        parameters.Add("@" + item.NombreCampo, item.ValueCampo, item.DbValueType, item.Direction);
      }
      IResultSpExecuteDataSet resp = await _context.ExecuteSpReturnDataSetAsync(_getValuesDataAnnotation.NombreTable, parameters);
      List<IAtributosPropiedad> outputParams = _getValuesDataAnnotation.ListaDetallesAtributos.Where((p) => p.Direction.Equals(ParameterDirection.Output) || p.Direction.Equals(ParameterDirection.ReturnValue)).ToList();
      if (outputParams.Count() == 1)
      {
        foreach (IAtributosPropiedad item in outputParams)
        {
          item.ValueCampo = resp.Parameters.Get<object>("@" + item.NombreCampo);
        }

        PropertyInfo prop = (from p in entity2.GetType().GetProperties()
                             from op in outputParams
                             where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                             select p).First();

        object value2 = (from p in outputParams
                         where prop.Name == p.NombreCampoClase
                         select p.ValueCampo).First();
        prop.SetValue(entity2, value2);
      }
      if (outputParams.Count > 1)
      {
        foreach (IAtributosPropiedad item in outputParams)
        {
          item.ValueCampo = resp.Parameters.Get<object>("@" + item.NombreCampo);
        }
        PropertyInfo[] prop2 = (from p in entity2.GetType().GetProperties()
                                from op in outputParams
                                where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                                select p).ToArray();
        PropertyInfo[] array = prop2;
        foreach (PropertyInfo prop in array)
        {
          object value = (from p in outputParams
                          where prop.Name == p.NombreCampoClase
                          select p.ValueCampo).First();
          prop.SetValue(entity2, value);
        }

      }

      response.SetEntity(entity2);
      response.SetDataSet(resp.ResultSp);
      return response;
    }

    public async Task<IResponseGridReaderSpExecution<TDomainEntity>> ExecuteSpAsyncGridReaderReturn(TDomainEntity entity)
    {
      TDomainEntity entity2 = entity;
      IResponseGridReaderSpExecution<TDomainEntity> response = new ResponseGridReaderSpExecution<TDomainEntity>();
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      _getValuesDataAnnotation.GetSpDataAnnotationValues();
      DynamicParameters parameters = new DynamicParameters();
      foreach (IAtributosPropiedad item2 in _getValuesDataAnnotation.ListaDetallesAtributos)
      {
        item2.ValueCampo = (from v in properties
                            where v.Name.Equals(item2.NombreCampoClase, StringComparison.Ordinal)
                            select v.GetValue(entity2)).First();

        parameters.Add("@" + item2.NombreCampo, item2.ValueCampo, item2.DbValueType, item2.Direction);
      }
      IResultSpExecuteGridReader resp = await _context.ExecuteSpReturnGridReaderAsync(_getValuesDataAnnotation.NombreTable, parameters);
      List<IAtributosPropiedad> outputParams = _getValuesDataAnnotation.ListaDetallesAtributos.Where((p) => p.Direction.Equals(ParameterDirection.Output) || p.Direction.Equals(ParameterDirection.ReturnValue)).ToList();
      if (outputParams.Count() == 1)
      {
        foreach (IAtributosPropiedad item4 in outputParams)
        {
          item4.ValueCampo = resp.Parameters.Get<object>("@" + item4.NombreCampo);
        }

        PropertyInfo prop2 = (from p in entity2.GetType().GetProperties()
                              from op in outputParams
                              where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                              select p).First();

        object value2 = (from p in outputParams
                         where prop2.Name == p.NombreCampoClase
                         select p.ValueCampo).First();
        prop2.SetValue(entity2, value2);
      }
      if (outputParams.Count > 1)
      {
        foreach (IAtributosPropiedad item3 in outputParams)
        {
          item3.ValueCampo = resp.Parameters.Get<object>("@" + item3.NombreCampo);
        }
        PropertyInfo[] prop3 = (from p in entity2.GetType().GetProperties()
                                from op in outputParams
                                where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                                select p).ToArray();
        PropertyInfo[] array = prop3;
        foreach (PropertyInfo item in array)
        {
          object value = (from p in outputParams
                          where item.Name == p.NombreCampoClase
                          select p.ValueCampo).First();
          item.SetValue(entity2, value);
        }

      }

      response.SetEntity(entity2);
      response.SetGridReader(resp.ResultSp);
      return response;
    }

    public async Task<IResponseGridReaderSpExecution<TDomainEntity>> ExecuteFunctionAsyncGridReaderReturn(TDomainEntity entity)
    {
      TDomainEntity entity2 = entity;
      IResponseGridReaderSpExecution<TDomainEntity> response = new ResponseGridReaderSpExecution<TDomainEntity>();
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      _getValuesDataAnnotation.GetSpDataAnnotationValues();
      DynamicParameters parameters = new DynamicParameters();
      foreach (IAtributosPropiedad item2 in _getValuesDataAnnotation.ListaDetallesAtributos)
      {
        item2.ValueCampo = (from v in properties
                            where v.Name.Equals(item2.NombreCampoClase, StringComparison.Ordinal)
                            select v.GetValue(entity2)).First();

        parameters.Add("@" + item2.NombreCampo, item2.ValueCampo, item2.DbValueType, item2.Direction);
      }
      IResultSpExecuteGridReader resp = await _context.ExecuteFunctionReturnGridReaderAsync(_getValuesDataAnnotation.NombreTable, parameters);
      List<IAtributosPropiedad> outputParams = _getValuesDataAnnotation.ListaDetallesAtributos.Where((p) => p.Direction.Equals(ParameterDirection.Output) || p.Direction.Equals(ParameterDirection.ReturnValue)).ToList();
      if (outputParams.Count() == 1)
      {
        foreach (IAtributosPropiedad item4 in outputParams)
        {
          item4.ValueCampo = resp.Parameters.Get<object>("@" + item4.NombreCampo);
        }

        PropertyInfo prop2 = (from p in entity2.GetType().GetProperties()
                              from op in outputParams
                              where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                              select p).First();

        object value2 = (from p in outputParams
                         where prop2.Name == p.NombreCampoClase
                         select p.ValueCampo).First();
        prop2.SetValue(entity2, value2);
      }
      if (outputParams.Count > 1)
      {
        foreach (IAtributosPropiedad item3 in outputParams)
        {
          item3.ValueCampo = resp.Parameters.Get<object>("@" + item3.NombreCampo);
        }
        PropertyInfo[] prop3 = (from p in entity2.GetType().GetProperties()
                                from op in outputParams
                                where p.Name.Equals(op.NombreCampoClase, StringComparison.Ordinal)
                                select p).ToArray();
        PropertyInfo[] array = prop3;
        foreach (PropertyInfo item in array)
        {
          object value = (from p in outputParams
                          where item.Name == p.NombreCampoClase
                          select p.ValueCampo).First();
          item.SetValue(entity2, value);
        }

      }

      response.SetEntity(entity2);
      response.SetGridReader(resp.ResultSp);
      return response;
    }

    private DynamicParameters BuidlParametersFromEntity(TDomainEntity entity, PropertyInfo[] properties)
    {
      _getValuesDataAnnotation.GetSpDataAnnotationValues();
      DynamicParameters dynamicParameters = new DynamicParameters();
      foreach (IAtributosPropiedad attribute in _getValuesDataAnnotation.ListaDetallesAtributos)
      {
        attribute.ValueCampo = properties.First((PropertyInfo p) => p.Name.Equals(attribute.NombreCampoClase, StringComparison.Ordinal)).GetValue(entity);
        if (string.IsNullOrEmpty(attribute.CustomDbType))
        {
          dynamicParameters.Add("@" + attribute.NombreCampo, attribute.ValueCampo, attribute.DbValueType, attribute.Direction);
        }
        else
        {
          dynamicParameters.Add("@" + attribute.NombreCampo, ((DataTable)attribute.ValueCampo).AsTableValuedParameter(attribute.CustomDbType), attribute.DbValueType, attribute.Direction);
        }
      }
      return dynamicParameters;
    }

    private void UpdateEntityWithOutputParameters(TDomainEntity entity, DynamicParameters outputParams, PropertyInfo[] properties)
    {
      foreach (PropertyInfo propertie in properties)
      {
        IAtributosPropiedad outputParam = _getValuesDataAnnotation.ListaDetallesAtributos.FirstOrDefault((IAtributosPropiedad p) => p.NombreCampoClase == propertie.Name && (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue));
        if (outputParam != null && outputParams.ParameterNames.Where((string x)=> x == outputParam.NombreCampo).ToList().Count >0)
        {
          object value = outputParams.Get<object>("@" + outputParam.NombreCampo);
          propertie.SetValue(entity, value);
        }
      }
    }
  }
}
