using System;

namespace nUpdate.Internal.Exceptions
{
    public class NetworkException : Exception
    {
        public NetworkException(string message)
            : base(message)
        {
        }
    }
}