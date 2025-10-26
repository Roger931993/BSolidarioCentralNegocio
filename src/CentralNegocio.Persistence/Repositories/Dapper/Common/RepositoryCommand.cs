using CentralNegocio.Domain.Common.Dapper;
using CentralNegocio.Domain.Interfaces.Dapper;
using Dapper;
using System.Reflection;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class RepositoryCommand<TDomaindEntity> : IRepositoryCommand<TDomaindEntity> where TDomaindEntity : Entity
  {
    public readonly DbContextDapperCommon _context;
    private readonly IGetDataAnnotationValues<TDomaindEntity> _getValuesDataAnnotation;
    private readonly string _nombreEntidad;

    public RepositoryCommand(DbContextDapperCommon context)
    {
      _context = context;
      _getValuesDataAnnotation = new ServiceGetDataAnnotationValue<TDomaindEntity>();
      _nombreEntidad = typeof(TDomaindEntity).Name;
    }

    public virtual async Task<int> AddEntityAsync(TDomaindEntity entity)
    {
      TDomaindEntity entity2 = entity;
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      _ = string.Empty;
      _ = string.Empty;
      DynamicParameters parameters = new DynamicParameters();
      try
      {
        _getValuesDataAnnotation.GetDataAnnotationValues();
        string tableName = string.IsNullOrEmpty(_getValuesDataAnnotation.NombreTable) ? _nombreEntidad : _getValuesDataAnnotation.NombreTable;
        foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
        {
          item.ValueCampo = (from v in properties
                             where v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal)
                             select v.GetValue(entity2)).First();

          parameters.Add("@" + item.NombreCampo, item.ValueCampo);
        }

        string query = $"INSERT INTO {tableName} ({string.Join(',', from p in _getValuesDataAnnotation.ListaDetallesAtributos
                                                                    where p.ValueCampo != null
                                                                    select p.NombreCampo)}) VALUES ({string.Join(',', from p in _getValuesDataAnnotation.ListaDetallesAtributos
                                                                                                                      where p.ValueCampo != null
                                                                                                                      select p.TipoDato.Equals("String", StringComparison.Ordinal) || p.TipoDato.Equals("DateTime", StringComparison.Ordinal) ? $"'{p.ValueCampo}'" : p.ValueCampo)})";
        return await _context.ExecuteAddAsync(query, parameters);
      }
      catch (Exception ex2)
      {
        Exception ex = ex2;
        throw;
      }
    }

    public virtual async Task<IEnumerable<TDomaindEntity>> AddEntitiesAsync(IEnumerable<TDomaindEntity> entities)
    {
      try
      {
        // Obtén el nombre de la tabla
        _getValuesDataAnnotation.GetDataAnnotationValues();
        string tableName = string.IsNullOrEmpty(_getValuesDataAnnotation.NombreTable) ? _nombreEntidad : _getValuesDataAnnotation.NombreTable;

        // Construye la lista de columnas para la consulta de inserción
        var columnNames = _getValuesDataAnnotation.ListaDetallesAtributos
            .Where(x => !x.PrimaryKey)
            .Select(p => p.NombreCampo)
            .ToList();

        // Construye la consulta de inserción base
        string insertQuery = $"INSERT INTO {tableName} ({string.Join(',', columnNames)}) VALUES ";

        // Lista para almacenar los valores de las filas
        var rowsValues = new List<string>();

        // Lista para almacenar los parámetros
        var allParameters = new DynamicParameters();

        int counter = 0;
        foreach (var entity in entities)
        {
          var properties = entity.GetType().GetProperties();

          // Asigna los valores de la entidad a los parámetros
          foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
          {
            item.ValueCampo = (from v in properties
                               where v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal)
                               select v.GetValue(entity)).First();

            allParameters.Add($"@{item.NombreCampo}_{counter}", item.ValueCampo);
          }

          // Construye la fila de valores para la consulta
          var rowValues = _getValuesDataAnnotation.ListaDetallesAtributos
              .Where(x => !x.PrimaryKey)
              .Select(p => $"@{p.NombreCampo}_{counter}")
              .ToList();

          rowsValues.Add($"({string.Join(',', rowValues)})");
          counter++;
        }

        // Une todas las filas a la consulta base
        insertQuery += string.Join(",", rowsValues);

        string columnKeyNames = _getValuesDataAnnotation.ListaDetallesAtributos.FirstOrDefault(x => x.PrimaryKey)!.NombreCampo;
        string charComa = !string.IsNullOrEmpty(columnKeyNames) ? "," : "";
        // Añade la cláusula RETURNING para devolver los registros insertados
        insertQuery += $" RETURNING {columnKeyNames} {charComa} *";

        // Ejecuta la consulta de inserción y obtiene los registros insertados
        IEnumerable<TDomaindEntity> insertedEntities = await _context.QueryAsync<TDomaindEntity>(insertQuery, allParameters);

        return insertedEntities;
      }
      catch (Exception ex2)
      {
        Exception ex = ex2;
        throw;
      }
    }
   

    public virtual async Task<IEnumerable<TDomaindEntity>> AddEntitiesWithKeyAsync(IEnumerable<TDomaindEntity> entities)
    {
      try
      {
        // Obtén el nombre de la tabla
        _getValuesDataAnnotation.GetDataAnnotationValues();
        string tableName = string.IsNullOrEmpty(_getValuesDataAnnotation.NombreTable) ? _nombreEntidad : _getValuesDataAnnotation.NombreTable;

        // Construye la lista de columnas para la consulta de inserción
        var columnNames = _getValuesDataAnnotation.ListaDetallesAtributos
            .Select(p => p.NombreCampo)
            .ToList();

        // Construye la consulta de inserción base
        string insertQuery = $"INSERT INTO {tableName} ({string.Join(',', columnNames)}) VALUES ";

        // Lista para almacenar los valores de las filas
        var rowsValues = new List<string>();

        // Lista para almacenar los parámetros
        var allParameters = new DynamicParameters();

        int counter = 0;
        foreach (var entity in entities)
        {
          var properties = entity.GetType().GetProperties();

          // Asigna los valores de la entidad a los parámetros
          foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
          {
            item.ValueCampo = (from v in properties
                               where v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal)
                               select v.GetValue(entity)).First();

            allParameters.Add($"@{item.NombreCampo}_{counter}", item.ValueCampo);
          }

          // Construye la fila de valores para la consulta
          var rowValues = _getValuesDataAnnotation.ListaDetallesAtributos
              .Select(p => $"@{p.NombreCampo}_{counter}")
              .ToList();

          rowsValues.Add($"({string.Join(',', rowValues)})");
          counter++;
        }

        // Une todas las filas a la consulta base
        insertQuery += string.Join(",", rowsValues);

        // Añade la cláusula RETURNING para devolver los registros insertados
        insertQuery += $" RETURNING *";

        // Ejecuta la consulta de inserción y obtiene los registros insertados
        IEnumerable<TDomaindEntity> insertedEntities = await _context.QueryAsync<TDomaindEntity>(insertQuery, allParameters);

        return insertedEntities;
      }
      catch (Exception ex2)
      {
        Exception ex = ex2;
        throw;
      }
    }

    public virtual async Task DeleteEntityAsync<TIdIdentity>(TIdIdentity tIdEntity)
    {
      try
      {
        _getValuesDataAnnotation.GetDataAnnotationValues();
        string tableName = string.IsNullOrEmpty(_getValuesDataAnnotation.NombreTable) ? _nombreEntidad : _getValuesDataAnnotation.NombreTable;
        string nameIdField = (from p in _getValuesDataAnnotation.ListaDetallesAtributos
                              where p.PrimaryKey
                              select p.NombreCampo).First().ToString();


        string query = $"DELETE FROM {tableName} WHERE {nameIdField} = {tIdEntity}";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add(nameIdField, tIdEntity);
        await _context.ExecuteDeleteAsync(query, parameters);
      }
      catch (Exception ex2)
      {
        Exception ex = ex2;
        throw;
      }
    }

    public virtual async Task UpdateEntityAsync<TIdEntity>(TIdEntity tIdEntity, TDomaindEntity entity)
    {
      TDomaindEntity entity2 = entity;
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      DynamicParameters parameters = new DynamicParameters();
      try
      {
        _getValuesDataAnnotation.GetDataAnnotationValues();
        string tableName = string.IsNullOrEmpty(_getValuesDataAnnotation.NombreTable) ? _nombreEntidad : _getValuesDataAnnotation.NombreTable;
        List<string> fieldsToUpdate = new List<string>();

        foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
        {
          var property = properties.FirstOrDefault(v => v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal));
          if (property != null)
          {
            var value = property.GetValue(entity2);
            if (value != null && !IsDefaultValue(value))
            {
              item.ValueCampo = value;
              parameters.Add("@" + item.NombreCampo, item.ValueCampo);
              if (!item.PrimaryKey)
              {
                fieldsToUpdate.Add($"{item.NombreCampo} = {(item.TipoDato.Equals("String", StringComparison.Ordinal) || item.TipoDato.Equals("DateTime", StringComparison.Ordinal) ? $"'{item.ValueCampo}'" : item.ValueCampo.ToString())}");
              }
            }
          }
        }

        if (fieldsToUpdate.Count > 0)
        {
          string query2 = $"UPDATE {tableName} SET {string.Join(',', fieldsToUpdate)} WHERE {(from p in _getValuesDataAnnotation.ListaDetallesAtributos where p.PrimaryKey select p.NombreCampo).First()}= {tIdEntity}";
          await _context.ExecuteUpdateAsync(query2, parameters);
        }
      }
      catch (Exception ex2)
      {
        Exception ex = ex2;
        throw;
      }
    }

    public virtual async Task<IEnumerable<TDomaindEntity>> SelectEntitiesAsync<TDomaindEntity>(TDomaindEntity filterEntity)
    {
      TDomaindEntity entity2 = filterEntity;
      PropertyInfo[] properties = entity2.GetType().GetProperties();
      DynamicParameters parameters = new DynamicParameters();
      List<string> filters = new List<string>();
      try
      {
        _getValuesDataAnnotation.GetDataAnnotationValues();
        string tableName = string.IsNullOrEmpty(_getValuesDataAnnotation.NombreTable) ? _nombreEntidad : _getValuesDataAnnotation.NombreTable;

        foreach (IAtributosPropiedad item in _getValuesDataAnnotation.ListaDetallesAtributos)
        {
          var property = properties.FirstOrDefault(v => v.Name.Equals(item.NombreCampoClase, StringComparison.Ordinal));
          if (property != null)
          {
            var value = property.GetValue(entity2);
            if (value != null && !IsDefaultValue(value))
            {
              item.ValueCampo = value;
              parameters.Add("@" + item.NombreCampo, item.ValueCampo);
              filters.Add($"{item.NombreCampo} = {(item.TipoDato.Equals("String", StringComparison.Ordinal) || item.TipoDato.Equals("DateTime", StringComparison.Ordinal) ? $"'{item.ValueCampo}'" : item.ValueCampo.ToString())}");
            }
          }
        }

        string query = $"SELECT * FROM {tableName}";

        if (filters.Count > 0)
        {
          query += $" WHERE {string.Join(" AND ", filters)}";
        }

        return await _context.QueryAsync<TDomaindEntity>(query, parameters);
      }
      catch (Exception ex)
      {
        throw new Exception("Error executing select query", ex);
      }
    }

    public virtual async Task<IEnumerable<TDomaindEntity>> SelectEntitieByIdAsync<TDomaindEntity, TIdEntity>(TIdEntity tIdEntity)
    {    
      try
      {
        _getValuesDataAnnotation.GetDataAnnotationValues();
        string tableName = string.IsNullOrEmpty(_getValuesDataAnnotation.NombreTable) ? _nombreEntidad : _getValuesDataAnnotation.NombreTable;       
        string nameIdField = (from p in _getValuesDataAnnotation.ListaDetallesAtributos
                              where p.PrimaryKey
                              select p.NombreCampo).First().ToString();
       

        string query = $"SELECT * FROM {tableName}  WHERE {nameIdField} = {tIdEntity}";

       
        return await _context.QueryAsync<TDomaindEntity>(query);
      }
      catch (Exception ex)
      {
        throw new Exception("Error executing select query", ex);
      }
    }

    private bool IsDefaultValue(object value)
    {
      if (value == null) return true;
      Type type = value.GetType();
      if (type == typeof(string))
      {
        return string.IsNullOrEmpty((string)value);
      }
      if (type.IsValueType)
      {
        return value.Equals(Activator.CreateInstance(type));
      }
      return false;
    }
  }
}
