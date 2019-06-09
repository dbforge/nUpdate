using System;

namespace nUpdate.Administration.Common.Exceptions
{
    internal class TransferProtocolException : Exception
    {
        public TransferProtocolException(string message) : base(message)
        { }
    }
}