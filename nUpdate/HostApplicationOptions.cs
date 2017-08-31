// Copyright © Dominic Beger 2017

namespace nUpdate
{
    /// <summary>
    ///     Represents different behaviours of the host application when the update installation is started.
    /// </summary>
    public enum HostApplicationOptions
    {
        /// <summary>
        ///     Leaves the host application open and does not restart it after the update installation.
        /// </summary>
        /// <remarks>This option is only recommended, if no program files are changed.</remarks>
        LeaveOpen,

        /// <summary>
        ///     Closes the host application and does not restart it after the update installation.
        /// </summary>
        Close,

        /// <summary>
        ///     Closes the host application and restarts it after the update installation.
        /// </summary>
        CloseAndRestart
    }
}