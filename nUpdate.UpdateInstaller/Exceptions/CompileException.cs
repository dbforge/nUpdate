// Copyright © Dominic Beger 2018

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