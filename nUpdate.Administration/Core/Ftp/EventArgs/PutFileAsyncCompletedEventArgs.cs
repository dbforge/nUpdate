// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Provides data for the PutFileAsyncCompleted event.
    /// </summary>
    [ComVisible(false)]
    public class PutFileAsyncCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the PutFileAsyncCompletedEventArgs class.
        /// </summary>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="canceled">A value indicating whether the asynchronous operation was canceled.</param>
        public PutFileAsyncCompletedEventArgs(Exception error, bool canceled)
            : base(error, canceled, null)
        {
        }
    }
}