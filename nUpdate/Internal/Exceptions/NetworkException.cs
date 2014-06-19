using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Internal.Exceptions
{
    internal class NetworkException : Exception
    {
        public NetworkException(string message) :
            base(message)
        { }
    }
}
