using System;

namespace nUpdate.Internal.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the JSON-file containing the configuration is invalid.
    /// </summary>
    public class InvalidJsonFileException : Exception
    {
        public InvalidJsonFileException(string message)
            : base(message)
        {
        }
    }
}