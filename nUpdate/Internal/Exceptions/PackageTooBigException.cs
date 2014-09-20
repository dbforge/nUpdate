using System;

namespace nUpdate.Internal.Exceptions
{
    public class PackageTooBigException : Exception
    {
        public PackageTooBigException(string message)
            : base(message)
        {
        }
    }
}