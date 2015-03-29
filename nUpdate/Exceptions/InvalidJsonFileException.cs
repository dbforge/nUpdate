// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the JSON-file containing the configuration is invalid.
    /// </summary>
    [Serializable]
    public class InvalidJsonFileException : Exception
    {
        public InvalidJsonFileException(string message)
            : base(message)
        {
        }
    }
}