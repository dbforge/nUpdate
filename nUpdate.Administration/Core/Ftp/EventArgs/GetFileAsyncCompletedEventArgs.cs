// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Provides data for the GetFileAsyncCompleted event.
    /// </summary>
    [ComVisible(false)]
    public class GetFileAsyncCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the GetFileAsyncCompletedEventArgs class.
        /// </summary>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="canceled">A value indicating whether the asynchronous operation was canceled.</param>
        public GetFileAsyncCompletedEventArgs(Exception error, bool canceled)
            : base(error, canceled, null)
        {
        }
    }
}