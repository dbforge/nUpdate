using System;

namespace nUpdate.Internal.Exceptions
{
    public class InvalidJsonFileException : Exception
    {
        public InvalidJsonFileException(string message)
            : base(message)
        {
        }
    }
}