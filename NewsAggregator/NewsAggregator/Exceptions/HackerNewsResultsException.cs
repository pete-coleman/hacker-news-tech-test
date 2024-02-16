using System.Globalization;

namespace NewsAggregator.Exceptions
{
    public class HackerNewsResultsException : Exception
    {
        public HackerNewsResultsException() : base() { }

        public HackerNewsResultsException(string message) : base(message) { }

        public HackerNewsResultsException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
