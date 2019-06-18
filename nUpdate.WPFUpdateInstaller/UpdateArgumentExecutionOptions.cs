// UpdateArgumentExecutionOptions.cs, 12.06.2019
// Copyright (C) Dominic Beger 17.06.2019

namespace nUpdate.WPFUpdateInstaller
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