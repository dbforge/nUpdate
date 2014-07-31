using System;

namespace nUpdate.Internal.Exceptions
{
    internal class NetworkException : Exception
    {
        public NetworkException(string message)
            : base(message)
        {
        }
    }
}