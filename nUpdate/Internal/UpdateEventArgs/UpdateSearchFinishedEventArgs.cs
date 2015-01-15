// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Internal.UpdateEventArgs
{
    /// <summary>
    ///     Provides data for the <see cref="UpdateManager.UpdateSearchFinished" />-event.
    /// </summary>
    public class UpdateSearchFinishedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateSearchFinishedEventArgs" />-class.
        /// </summary>
        /// <param name="updateAvailable">A value which indicates whether a new update is available or not.</param>
        public UpdateSearchFinishedEventArgs(bool updateAvailable)
        {
            UpdateAvailable = updateAvailable;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether a new update is available or not.
        /// </summary>
        public bool UpdateAvailable { get; private set; }
    }
}