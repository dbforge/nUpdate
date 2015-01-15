// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Internal.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if creating a entry for the statistics fails.
    /// </summary>
    [Serializable]
    public class StatisticsException : Exception
    {
        public StatisticsException(string message)
            : base(message)
        {
        }
    }
}