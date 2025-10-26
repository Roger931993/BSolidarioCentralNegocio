using CentralNegocio.Domain.Interfaces.Dapper;
using System.Data;

namespace CentralNegocio.Persistence.Repositories.Dapper.Common
{
    public class AtributosPropiedad : IAtributosPropiedad
    {
        public string NombreCampo { get; private set; } = string.Empty;
        public string TipoDato { get; private set; }
        public bool PrimaryKey { get; private set; } = false;
        public bool ForeingKey { get; private set; } = false;
        public string NombreCampoClase { get; private set; } = string.Empty;

        public DbType? DbValueType { get; private set; }
        public ParameterDirection? Direction { get; private set; }
        public object ValueCampo { get; set; }

        public string CustomDbType { get; set; }

        public void SetNombreCampo(string nombreCampo)
        {
            NombreCampo = nombreCampo;
        }
        public void SetTipoDato(string tipoDato)
        {
            TipoDato = tipoDato;
        }

        public void SetPrimaryKey(bool isPrimaryKey)
        {
            PrimaryKey = isPrimaryKey;
        }

        public void SetForeingKey(bool isForeingKey)
        {
            ForeingKey = isForeingKey;
        }

        public void SetNombreCampoClase(string nombreCampoClase)
        {
            NombreCampoClase = nombreCampoClase;
        }

        public void SetValueDbType(DbType type)
        {
            DbValueType = type;
        }
        public void SetDirecctionParame(ParameterDirection parameter)
        {
            Direction = parameter;
        }
    }

}