// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace nUpdate.Administration.TransferInterface
{
    public interface ITransferProgressData
    {
        /// <summary>
        ///     Gets the number of bytes transferred in the last transfer block.
        /// </summary>
        int BytesTransferred { get; }

        /// <summary>
        ///     Gets the total number of bytes transferred for a particular transfer event.
        /// </summary>
        long TotalBytesTransferred { get; }

        /// <summary>
        ///     Gets the data transfer speed in bytes per second.
        /// </summary>
        int BytesPerSecond { get; }

        /// <summary>
        ///     Gets the data transfer speed in Kilobytes per second.
        /// </summary>
        int KilobytesPerSecond { get; }

        /// <summary>
        ///     Gets the data transfer speed in Megabytes per second.
        /// </summary>
        int MegabytesPerSecond { get; }

        /// <summary>
        ///     Gets the data transfer speed in Gigabytes per second.
        /// </summary>
        int GigabytesPerSecond { get; }

        /// <summary>
        ///     Gets the time that has elapsed since the data transfer started.
        /// </summary>
        TimeSpan ElapsedTime { get; }

        /// <summary>
        ///     Gets the percentage of the transfer completion, if data is available.
        /// </summary>
        int PercentComplete { get; }

        /// <summary>
        ///     Gets the size of the data transfer´, if data is available.
        /// </summary>
        long TransferSize { get; }

        /// <summary>
        ///     Gets the number of bytes remaining in the transfer.
        /// </summary>
        long BytesRemaining { get; }

        /// <summary>
        ///     Gets the estimated time that remains for the data transfer.
        /// </summary>
        TimeSpan TimeRemaining { get; }
    }
}