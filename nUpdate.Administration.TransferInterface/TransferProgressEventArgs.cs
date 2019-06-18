// TransferProgressEventArgs.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate.Administration.TransferInterface
{
    public class TransferProgressEventArgs : EventArgs
    {
        public TransferProgressEventArgs(int bytesTransferred, long totalBytesTransferred, int bytesPerSecond,
            int kilobytesPerSecond, int megabytesPerSecond, int gigabytesPerSecond, TimeSpan elapsedTime,
            int percentComplete, long transferSize, long bytesRemaining, TimeSpan timeRemaining)
        {
            BytesTransferred = bytesTransferred;
            TotalBytesTransferred = totalBytesTransferred;
            BytesPerSecond = bytesPerSecond;
            KilobytesPerSecond = kilobytesPerSecond;
            MegabytesPerSecond = megabytesPerSecond;
            GigabytesPerSecond = gigabytesPerSecond;
            ElapsedTime = elapsedTime;
            PercentComplete = percentComplete;
            TransferSize = transferSize;
            BytesRemaining = bytesRemaining;
            TimeRemaining = timeRemaining;
        }

        /// <summary>
        ///     Gets or sets the number of bytes transferred in the last transfer block.
        /// </summary>
        public int BytesTransferred { get; set; }

        /// <summary>
        ///     Gets or sets the total number of bytes transferred for a particular transfer event.
        /// </summary>
        public long TotalBytesTransferred { get; set; }

        /// <summary>
        ///     Gets or sets the data transfer speed in bytes per second.
        /// </summary>
        public int BytesPerSecond { get; set; }

        /// <summary>
        ///     Gets or sets the data transfer speed in Kilobytes per second.
        /// </summary>
        public int KilobytesPerSecond { get; set; }

        /// <summary>
        ///     Gets or sets the data transfer speed in Megabytes per second.
        /// </summary>
        public int MegabytesPerSecond { get; set; }

        /// <summary>
        ///     Gets or sets the data transfer speed in Gigabytes per second.
        /// </summary>
        public int GigabytesPerSecond { get; set; }

        /// <summary>
        ///     Gets or sets the time that has elapsed since the data transfer started.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        /// <summary>
        ///     Gets or sets the percentage of the transfer completion, if data is available.
        /// </summary>
        public int PercentComplete { get; set; }

        /// <summary>
        ///     Gets or sets the size of the data transfer´, if data is available.
        /// </summary>
        public long TransferSize { get; set; }

        /// <summary>
        ///     Gets or sets the number of bytes remaining in the transfer.
        /// </summary>
        public long BytesRemaining { get; set; }

        /// <summary>
        ///     Gets or sets the estimated time that remains for the data transfer.
        /// </summary>
        public TimeSpan TimeRemaining { get; set; }
    }
}