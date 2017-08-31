// Copyright © Dominic Beger 2017

using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the calculation of the package size fails.
    /// </summary>
    [Serializable]
    public class SizeCalculationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SizeCalculationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SizeCalculationException(string message)
            : base(message)
        {
        }
    }
}