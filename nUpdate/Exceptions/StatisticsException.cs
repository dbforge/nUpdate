// Copyright © Dominic Beger 2017

using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the statistics entry fails.
    /// </summary>
    [Serializable]
    public class StatisticsException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StatisticsException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StatisticsException(string message)
            : base(message)
        {
        }
    }
}