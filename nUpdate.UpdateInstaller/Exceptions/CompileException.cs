// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace nUpdate.UpdateInstaller.Exceptions
{
    public class CompileException : Exception
    {
        public CompileException(string message)
            : base(message)
        {
        }
    }
}