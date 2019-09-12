using System;

namespace nUpdate.Administration.Common.Exceptions
{
    public class TransferProtocolException : Exception
    {
        public TransferProtocolException(string message) : base(message)
        { }
    }
}