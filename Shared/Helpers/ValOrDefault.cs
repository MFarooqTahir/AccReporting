using System.Globalization;

namespace AccReporting.Shared.Helpers
{
    internal static class ValOrDefault
    {
        public static string Str(string val) => string.IsNullOrEmpty(value: val) ? " " : val;

        public static int? ToInt(string val) => int.TryParse(s: val, result: out var outval) ? outval : null;

        public static double? ToDouble(string val) => double.TryParse(s: val, result: out var outval) ? outval : null;

        public static decimal? ToDecimal(string val) => decimal.TryParse(s: val, result: out var outval) ? outval : null;

        public static DateTime? ToDateTime(string val) => string.IsNullOrWhiteSpace(value: val) ? DateTime.MinValue : DateTime.TryParseExact(s: val, format: "dd/MM/yyyy", provider: CultureInfo.InvariantCulture, style: DateTimeStyles.None, result: out var outval) ? outval : DateTime.MinValue;
    }
}