// SizeCalculationException.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown, if the package size cannot be calculated.
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