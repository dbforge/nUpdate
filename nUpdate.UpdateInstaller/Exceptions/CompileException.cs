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
