using System;

namespace nUpdate.Internal.Exceptions
{
    public class StatisticsException : Exception
    {
        public StatisticsException(string message)
            : base(message)
        {
        }
    }
}