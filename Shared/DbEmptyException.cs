namespace AccReporting.Shared
{
    public class DbEmptyException : Exception
    {
        public DbEmptyException() : base()
        {
        }

        public DbEmptyException(string message) : base(message)
        {
        }

        public DbEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}