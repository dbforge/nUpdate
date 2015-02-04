using System;

namespace nUpdate.Internal.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the PHP-script for the communication with the statistics server returns an error message.
    /// </summary>
    [Serializable]
    public class StatisticsScriptException : Exception
    {
        public StatisticsScriptException(string message)
            : base(message)
        {
        }
    }
}
