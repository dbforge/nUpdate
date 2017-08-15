// Copyright © Dominic Beger 2017

using System;

namespace nUpdate.UpdateEventArgs
{
    /// <summary>
    ///     Provides data for any event that represents a failure of an operation.
    /// </summary>
    public class FailedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FailedEventArgs" />-class.
        /// </summary>
        /// <param name="exception"></param>
        internal FailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     Gets or sets a value representing the exception that occured.
        /// </summary>
        public Exception Exception { get; set; }
    }
}