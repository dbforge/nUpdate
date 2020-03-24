// TransferProtocolException.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Administration.BusinessLogic.Exceptions
{
    public class TransferProtocolException : Exception
    {
        public TransferProtocolException(string message) : base(message)
        {
        }
    }
}