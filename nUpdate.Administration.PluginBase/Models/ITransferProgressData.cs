// ITransferProgressData.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Administration.PluginBase.Models
{
    public interface ITransferProgressData
    {
        /// <summary>
        ///     Gets the data transfer speed in bytes per second.
        /// </summary>
        double BytesPerSecond { get; }

        /// <summary>
        ///     Gets the estimated time span for finishing the data transfer.
        /// </summary>
        TimeSpan FinishTime { get; }

        /// <summary>
        ///     Gets the data transfer speed in Gigabytes per second.
        /// </summary>
        double GigabytesPerSecond { get; }

        /// <summary>
        ///     Gets the data transfer speed in Kilobytes per second.
        /// </summary>
        double KilobytesPerSecond { get; }

        /// <summary>
        ///     Gets the data transfer speed in Megabytes per second.
        /// </summary>
        double MegabytesPerSecond { get; }

        /// <summary>
        ///     Gets the progress of the transfer completion.
        /// </summary>
        double Progress { get; }
    }
}