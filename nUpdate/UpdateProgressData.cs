// Copyright © Dominic Beger 2017

namespace nUpdate
{
    /// <summary>
    ///     Provides data for progress updates.
    /// </summary>
    public struct UpdateProgressData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateProgressData" /> class.
        /// </summary>
        /// <param name="bytesReceived">The amount of bytes received.</param>
        /// <param name="totalBytesToReceive">The total bytes to receive.</param>
        /// <param name="percentage">The progress percentage.</param>
        internal UpdateProgressData(long bytesReceived, long totalBytesToReceive, float percentage)
        {
            TotalBytesToReceive = totalBytesToReceive;
            BytesReceived = bytesReceived;
            Percentage = percentage;
        }

        /// <summary>
        ///     Gets the total bytes to receive.
        /// </summary>
        public long TotalBytesToReceive { get; }

        /// <summary>
        ///     Gets the amount of received bytes.
        /// </summary>
        public long BytesReceived { get; }

        /// <summary>
        ///     Gets the progress percentage.
        /// </summary>
        public float Percentage { get; }
    }
}