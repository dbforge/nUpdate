using System;

namespace nUpdate.Administration.Exceptions
{
    internal class TransferProtocolException : Exception
    {
        public TransferProtocolException(string message) : base(message)
        { }
    }
}