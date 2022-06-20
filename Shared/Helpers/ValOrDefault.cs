using System.Globalization;

namespace AccReporting.Shared.Helpers
{
    internal static class ValOrDefault
    {
        public static string str(string val) => string.IsNullOrEmpty(val) ? " " : val;

        public static int? ToInt(string val) => int.TryParse(val, out int outval) ? outval : null;

        public static double? ToDouble(string val) => double.TryParse(val, out double outval) ? outval : null;

        public static decimal? ToDecimal(string val) => decimal.TryParse(val, out decimal outval) ? outval : null;

        public static DateTime? ToDateTime(string val) => string.IsNullOrWhiteSpace(val) ? DateTime.MinValue : DateTime.TryParseExact(val, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime outval) ? outval : DateTime.MinValue;
    }
}