// Copyright © Dominic Beger 2018

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