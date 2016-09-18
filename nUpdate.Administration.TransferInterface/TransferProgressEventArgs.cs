// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace nUpdate.Administration.TransferInterface
{
    /// <summary>
    ///     Provides the data for the <see cref="TransferProgressEventArgs" />-event.
    /// </summary>
    public class TransferProgressEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TransferProgressEventArgs" />-class.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred.</param>
        /// <param name="totalBytesTransferred">Total number of bytes transferred.</param>
        /// <param name="bytesPerSecond">The data transfer speed in bytes per second.</param>
        /// <param name="elapsedTime">The time that has elapsed since the data transfer started.</param>
        /// <param name="totalBytes">Total bytes of data.</param>
        public TransferProgressEventArgs(int bytesTransferred, long totalBytesTransferred, int bytesPerSecond,
            TimeSpan elapsedTime, long totalBytes)
        {
            BytesTransferred = bytesTransferred;
            TotalBytesTransferred = totalBytesTransferred;
            BytesPerSecond = bytesPerSecond;
            ElapsedTime = elapsedTime;
            TotalBytes = totalBytes;
        }

        /// <summary>
        ///     The number of bytes transferred.
        /// </summary>
        public int BytesTransferred { get; }

        /// <summary>
        ///     Total number of bytes transferred.
        /// </summary>
        public long TotalBytesTransferred { get; }

        /// <summary>
        ///     Total bytes of data.
        /// </summary>
        public long TotalBytes { get; }

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

        /// <summary>
        ///     Transferred data percentage.
        /// </summary>
        public float Percentage => (float) TotalBytesTransferred/TotalBytes*100;

        public TimeSpan EstimatedCompleteTime
        {
            get
            {
                double elapsed = ElapsedTime.TotalMilliseconds;
                double totalTime = (double) TotalBytes/TotalBytesTransferred*elapsed;
                return TimeSpan.FromMilliseconds(totalTime - elapsed);
            }
        }
    }
}