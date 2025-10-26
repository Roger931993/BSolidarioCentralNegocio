namespace CentralNegocio.Shared.Helpers
{
    public static class DecimalExtensions
    {
        public static decimal ToNumDecimal(this decimal dec, int NumDecimals)
        {
            return Math.Round(dec, NumDecimals);
        }
        public static decimal? ToNumDecimal(this decimal? dec, int NumDecimals)
        {
            return Math.Round((decimal)dec!, NumDecimals);
        }
        public static int ToInt(this decimal? dec)
        {
            return Convert.ToInt32(dec);
        }
    }
}
