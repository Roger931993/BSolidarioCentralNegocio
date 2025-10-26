using CentralNegocio.Domain.Common.Dapper;
using CentralNegocio.Domain.Interfaces.Dapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class ServiceGetDataAnnotationValue<TDomainEntity> : IGetDataAnnotationValues<TDomainEntity>, IAtributoEntidad where TDomainEntity : class
  {
    private object[] _propsEntity = typeof(TDomainEntity).GetProperties();
    public IList<IAtributosPropiedad> ListaDetallesAtributos { get; private set; } = new List<IAtributosPropiedad>();

    public string Esquema { get; private set; }
    public string NombreTable { get; private set; }
    public bool HasDataEntity { get; private set; } = true;
    public string ErrorMessageRecoverDataValues { get; private set; } = string.Empty;

    private void SetNombreTabla()
    {
      IEnumerable<CustomAttributeData> customAttribute = typeof(TDomainEntity).CustomAttributes;
      try
      {
        NombreTable = (from p in customAttribute
                       where p.AttributeType.Name.Equals("TableAttribute", StringComparison.Ordinal)
                       select p.ConstructorArguments[0].Value.ToString()).FirstOrDefault() ?? string.Empty;
      }
      catch (Exception)
      {
        ErrorMessageRecoverDataValues = "No DataAnnotation - Table name";
      }
      try
      {
        Esquema = (from p in customAttribute
                   where p.AttributeType.Name.Equals("TableAttribute", StringComparison.Ordinal)
                   select p.NamedArguments[0].TypedValue.ToString()).FirstOrDefault() ?? string.Empty;
      }
      catch (Exception)
      {
        if (string.IsNullOrEmpty(NombreTable))
        {
          ErrorMessageRecoverDataValues = string.Empty;
          HasDataEntity = false;
          return;
        }
        ErrorMessageRecoverDataValues = "No DataAnnotation - Esquema Data Base";
        HasDataEntity = true;
      }

      if (Esquema != null)
      {
        NombreTable = Esquema.Trim() + "." + NombreTable.Trim();
      }
    }

    private void SetTableNameDataset()
    {
      IEnumerable<CustomAttributeData> customAttributes = typeof(TDomainEntity).CustomAttributes;
      try
      {
        NombreTable = (from p in customAttributes
                       where p.AttributeType.Name.Equals("DataAnnotationSpNameAttribute", StringComparison.Ordinal)
                       select p.ConstructorArguments[0].Value.ToString()).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(NombreTable))
          throw new Exception("Name SP empty");
      }
      catch (Exception)
      {
        throw new Exception("No DataAnnotationSpName - Is Empty");
      }
    }

    public void GetDataAnnotationValues()
    {
      SetNombreTabla();
      if (!HasDataEntity)
      {
        return;
      }

      object[] propsEntity = _propsEntity;
      for (int i = 0; i < propsEntity.Length; i++)
      {
        PropertyInfo propertyInfo = (PropertyInfo)propsEntity[i];
        bool flag = false;
        AtributosPropiedad atributosPropiedad = new AtributosPropiedad();
        object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
        if (customAttributes != null || customAttributes.Length > 1)
        {
          object[] array = customAttributes;
          for (int j = 0; j < array.Length; j++)
          {
            Attribute attribute = (Attribute)array[j];
            if (attribute is NotMappedAttribute)
            {
              flag = true;
              break;
            }
            if (attribute is ColumnAttribute)
            {
              atributosPropiedad.SetNombreCampo((attribute as ColumnAttribute).Name.ToString() ?? string.Empty);
            }

            if (attribute is KeyAttribute)
            {
              atributosPropiedad.SetPrimaryKey(isPrimaryKey: true);
            }
            if (attribute is ForeignKeyAttribute)
            {
              atributosPropiedad.SetForeingKey(isForeingKey: true);
            }
          }
          if (!flag)
          {
            if (string.IsNullOrEmpty(atributosPropiedad.NombreCampo))
            {
              atributosPropiedad.SetNombreCampo(propertyInfo.Name.ToString());
            }
            atributosPropiedad.SetNombreCampoClase(propertyInfo.Name.ToString());
            atributosPropiedad.SetTipoDato(propertyInfo.PropertyType.Name.ToString());
            ListaDetallesAtributos.Add(atributosPropiedad);
          }
          continue;
        }
        throw new Exception("SP Annotation no implement");
      }
    }

    public void GetSpDataAnnotationValues()
    {
      SetTableNameDataset();
      if (!HasDataEntity)
      {
        return;
      }
      object[] propsEntity = _propsEntity;
      for (int i = 0; i < propsEntity.Length; i++)
      {
        PropertyInfo propertyInfo = (PropertyInfo)propsEntity[i];
        AtributosPropiedad atributosPropiedad = new AtributosPropiedad();
        object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
        if (propertyInfo.DeclaringType.FullName.ToString().Equals("Heifer.Model.Common", StringComparison.Ordinal))
        {
          continue;
        }
        if (customAttributes.Where((x) => x is SpParametersCommonAttribute).Count() == 0)
        {
          throw new Exception("No implement SP entity");
        }

        object[] array = customAttributes;
        for (int j = 0; j < array.Length; j++)
        {
          Attribute attribute = (Attribute)array[j];
          if (attribute is NotMappedAttribute)
          {
            break;
          }
          if (attribute is SpParametersCommonAttribute)
          {
            atributosPropiedad.SetNombreCampo((attribute as SpParametersCommonAttribute).ParamName ?? string.Empty);
            atributosPropiedad.SetDirecctionParame((attribute as SpParametersCommonAttribute).Direction);
            atributosPropiedad.SetValueDbType((attribute as SpParametersCommonAttribute).DbType);
          }
        }
        atributosPropiedad.SetNombreCampoClase(propertyInfo.Name.ToString());
        ListaDetallesAtributos.Add(atributosPropiedad);
      }
    }
  }
}
