// StatisticsException.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown, if the statistics entry fails.
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