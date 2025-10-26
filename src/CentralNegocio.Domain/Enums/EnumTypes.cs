using System.ComponentModel;

namespace CentralNegocio.Model.Entity
{
    public class EnumTypes
    {
        public enum TypeStatus
        {
            Disable,
            Active
        }

        public enum TypeError
        {
            Ninguno = 0,
            Ok = 10000,
            ErrorGenerico = 9999,
            NoInformation = 1,
            NoData = 2,
            NoDeterminate = 3,
            DataIncorrect = 4,
            ErrorTimeout = 5,
            InternalError = 6,
            SaldoNoDisponible = 7
        }   

        public enum TypeService
        {
            None = 0,
            Frontend = 1,
            BackEnd = 2,
        }

        // Método para buscar un enum a partir de su descripción
        public static bool TryGetEnumFromDescription<TEnum>(string description, out TEnum result) where TEnum : struct
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                     .Cast<DescriptionAttribute>()
                                     .FirstOrDefault();

                if (attribute?.Description == description)
                {
                    result = (TEnum)field.GetValue(null);
                    return true;
                }
            }

            result = default;
            return false;
        }

    }
}
