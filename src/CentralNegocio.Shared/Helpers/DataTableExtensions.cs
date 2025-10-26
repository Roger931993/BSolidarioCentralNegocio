using System.Data;
using System.Reflection;

namespace CentralNegocio.Shared.Helpers
{
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(IList<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] array = properties;
            foreach (PropertyInfo item in array)
            {
                dataTable.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
            }

            foreach (T item in items)
            {
                object[] array2 = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    array2[i] = properties[i].GetValue(item, null);
                }

                dataTable.Rows.Add(array2);
            }
            return dataTable;
        }
    }
}
