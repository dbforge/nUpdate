﻿using System;

namespace nUpdate.Internal.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the package size calculating fails.
    /// </summary>
    [Serializable]
    public class SizeCalculationException : Exception
    {
        public SizeCalculationException(string message)
            : base(message)
        {
        }
    }
}