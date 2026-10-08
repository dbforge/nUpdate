namespace nUpdate.Ui;

/// <summary>How a run of the <see cref="UpdateFlow" /> ended.</summary>
public enum UpdateFlowResult
{
    /// <summary>Another run was still in progress; nothing happened.</summary>
    AlreadyRunning,

    /// <summary>The application is up to date.</summary>
    NoUpdates,

    /// <summary>The user cancelled the search or the download.</summary>
    Cancelled,

    /// <summary>The user did not want to install the updates.</summary>
    Declined,

    /// <summary>The temp drive cannot hold the packages; the user was told how much to free.</summary>
    InsufficientDiskSpace,

    /// <summary>A step failed; the error was shown to the user.</summary>
    Failed,

    /// <summary>A downloaded package did not carry a valid signature and was deleted.</summary>
    InvalidSignature,

    /// <summary>The user declined the elevation prompt of the installer.</summary>
    ElevationDeclined,

    /// <summary>The installer was started; the host application is being closed when configured to.</summary>
    InstallerStarted,
}
