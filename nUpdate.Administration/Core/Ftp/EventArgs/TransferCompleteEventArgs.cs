// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Event arguments to facilitate the transfer complete event.
    /// </summary>
    public class TransferCompleteEventArgs : System.EventArgs
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytesTransferred">The total number of bytes transferred.</param>
        /// <param name="bytesPerSecond">The data transfer speed in bytes per second.</param>
        /// <param name="elapsedTime">The time that has elapsed since the data transfer started.</param>
        public TransferCompleteEventArgs(long bytesTransferred, int bytesPerSecond, TimeSpan elapsedTime)
        {
            BytesTransferred = bytesTransferred;
            BytesPerSecond = bytesPerSecond;
            ElapsedTime = elapsedTime;
        }

        /// <summary>
        ///     The total number of bytes transferred.
        /// </summary>
        public long BytesTransferred { get; }

        /// <summary>
        ///     Gets the data transfer speed in bytes per second.
        /// </summary>
        public int BytesPerSecond { get; }

        /// <summary>
        ///     Gets the data transfer speed in kilobytes per second.
        /// </summary>
        public int KilobytesPerSecond => BytesPerSecond/1024;

        /// <summary>
        ///     Gets the time that has elapsed since the data transfer started.
        /// </summary>
        public TimeSpan ElapsedTime { get; }
    }
}