using System.Text.RegularExpressions;

namespace CentralNegocio.Shared.Helpers
{
    public static class Helper
    {
        public static string GetNameOfSpi(string core, string module)
        {
            string text = string.Empty;
            string[] array = Regex.Split(core, "(?<!^)(?=[A-Z])");
            string[] array2 = array;
            foreach (string item in array2)
            {
                text = text + item + "_";
            }
            text = ("log_" + text).ToLower();
            return ("spi_" + text + module).ToLower();
        }
    }
}
