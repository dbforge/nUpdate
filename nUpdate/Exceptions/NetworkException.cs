// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if no network connection is available.
    /// </summary>
    [Serializable]
    public class NetworkException : Exception
    {
        public NetworkException(string message)
            : base(message)
        {
        }
    }
}