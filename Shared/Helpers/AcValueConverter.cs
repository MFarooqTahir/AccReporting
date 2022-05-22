namespace AccReporting.Shared.Helpers
{
    public class AcValueConverter
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB,
        }

        public static string ToSize(long value, SizeUnits unit)
        {
            return (value / Math.Pow(1024, (int)unit)).ToString("0.00");
        }
    }
}