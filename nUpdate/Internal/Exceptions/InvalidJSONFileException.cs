using System;

namespace nUpdate.Internal.Exceptions
{
    internal class InvalidJSONFileException : Exception
    {
        public InvalidJSONFileException(string message)
            : base(message)
        {
        }
    }
}