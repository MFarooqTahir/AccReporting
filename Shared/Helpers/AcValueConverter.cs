namespace AccReporting.Shared.Helpers
{
    public class AcValueConverter
    {
        public enum SizeUnits
        {
            Byte, Kb, Mb, Gb, Tb, Pb, Eb, Zb, Yb,
        }

        public static string ToSize(long value, SizeUnits unit)
        {
            return (value / Math.Pow(x: 1024, y: (int)unit)).ToString(format: "0.00");
        }
    }
}