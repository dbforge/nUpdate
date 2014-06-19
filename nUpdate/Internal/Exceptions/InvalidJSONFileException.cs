using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Internal.Exceptions
{
    internal class InvalidJSONFileException : Exception
    {
        public InvalidJSONFileException(string message) :
            base(message)
        { }
    }
}
