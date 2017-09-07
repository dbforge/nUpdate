// Copyright © Dominic Beger 2017

using System;
using nUpdate.Updating;

namespace nUpdate.UpdateEventArgs
{
    /// <summary>
    ///     Provides data for the <see cref="UpdateManager.UpdateSearchFinished" />-event.
    /// </summary>
    public class UpdateSearchFinishedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateSearchFinishedEventArgs" />-class.
        /// </summary>
        /// <param name="updatesAvailable">A value which indicates whether a new update is available or not.</param>
        internal UpdateSearchFinishedEventArgs(bool updatesAvailable)
        {
            UpdatesAvailable = updatesAvailable;
        }

        /// <summary>
        ///     Gets a value indicating whether new updates are available, or not.
        /// </summary>
        public bool UpdatesAvailable { get; }
    }
}