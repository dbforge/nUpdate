using System;

namespace nUpdate.Administration.BusinessLogic.Exceptions
{
    public class TransferProtocolException : Exception
    {
        public TransferProtocolException(string message) : base(message)
        { }
    }
}