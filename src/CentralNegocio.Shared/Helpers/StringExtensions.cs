namespace CentralNegocio.Shared.Helpers
{
    public static class StringExtensions
    {
        public static decimal? ToDecimal(this string str)
        {


            if (decimal.TryParse(str, out decimal result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static decimal? ToDecimal(this string str, int NumDecimals)
        {
            if (decimal.TryParse(str, out decimal result))
            {
                return Math.Round(result, NumDecimals);
            }
            else
            {
                return null;
            }
        }

        public static int ToInteger(this string str)
        {
            if (int.TryParse(str, out int result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        public static int? ToIntegerNull(this string str)
        {
            if (int.TryParse(str, out int result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static DateTime? ToDateTime(this string str)
        {
            if (DateTime.TryParse(str, out DateTime result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static string ToStringParent(this string strChild)
        {
            List<string> strParent1 = strChild.Split('.').ToList();
            int total = strParent1.Count();
            string strResult = string.Empty;
            string strAux = "00";
            int intParent = 0;
            for (int i = 0; i < strParent1.Where(x => x != "00").Count() - 1; i++)
            {
                strResult += strParent1[i].ToString() + ".";
                intParent++;
            }
            if (intParent == 0)
            {
                for (int i = 0; i < total; i++)
                {
                    strResult += strAux + ".";
                }
                strResult = strResult.TrimEnd('.');
                return strResult;
            }

            for (int i = 0; i < total - intParent; i++)
            {
                strResult += strAux + ".";
            }
            strResult = strResult.TrimEnd('.');
            return strResult;
        }
    }
}
