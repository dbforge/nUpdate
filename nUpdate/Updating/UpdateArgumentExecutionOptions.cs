// Copyright © Dominic Beger 2017

namespace nUpdate.Updating
{
    /// <summary>
    ///     Sets the behaviour of an <see cref="UpdateArgument" /> when an update is installed.
    /// </summary>
    public enum UpdateArgumentExecutionOptions
    {
        /// <summary>
        ///     Sets that the parameter should only be executed if the installation of an update succeeded.
        /// </summary>
        OnlyOnSucceeded,

        /// <summary>
        ///     Sets that the parameter should only be executed if the installation of an update failed.
        /// </summary>
        OnlyOnFaulted
    }
}