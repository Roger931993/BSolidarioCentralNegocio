namespace CentralNegocio.Shared.Constants
{
  public static class Generals
  {
    public struct General
    {
      public const string schema_db_logs = "logs";
      public const string schema_db_cliente = "cliente";
      public const string nemonico_table = "cat";
    }
    public struct MesaggeError
    {

      public const string Ok = "Ok";
      public const string ErrorNoData = "No existe Data";
      public const string ErrorNoParent = "No existe padre";
      public const string ErrorNoResultFramework = "No existe campos relacionados (Result Framework, Strategy Framework)";
      public const string ErrorNoIndicate = "No existe campos relacionados (Indicator)";
      public const string ErrorIndicatorType = "No existe campos relacionados (indicator_type_id)";
      public const string ErrorCamposRelacionados = "No existe campos relacionados";
      public const string ErrorFrameworkId = "No existe campos relacionados (framework_id)";
      public const string Finalizado = "Finalizado";
      public const string ErrorColumnaVacio = "Error -> El valor de la columna es vacio: ";
      public const string ErrorFormatoPeriodo = "Error -> Formato de periodo incorrecto (yyyyMM): ";

    }

    public struct NameSpacesObject
    {     
    }

    public struct Claims
    {
      public const string idSession = "idSession";
      public const string isAdmin = "isAdmin";
      public const string IdUser = "userId";
    }
  }
}
