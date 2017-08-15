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
        public StatisticsException(string message)
            : base(message)
        {
        }
    }
}