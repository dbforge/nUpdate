// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Provides data for the GetDirListAsyncCompleted event.
    /// </summary>
    [ComVisible(false)]
    public class GetDirListAsyncCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the PutFileAsyncCompletedEventArgs class.
        /// </summary>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="canceled">A value indicating whether the asynchronous operation was canceled.</param>
        /// <param name="directoryListing">A FtpItemCollection containing the directory listing.</param>
        public GetDirListAsyncCompletedEventArgs(Exception error, bool canceled, FtpItemCollection directoryListing)
            : base(error, canceled, null)
        {
            DirectoryListingResult = directoryListing;
        }

        /// <summary>
        ///     Directory listing collection.
        /// </summary>
        public FtpItemCollection DirectoryListingResult { get; }
    }
}