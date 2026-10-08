namespace nUpdate.Installer;

/// <summary>How the engine proceeds with a file that is in use.</summary>
public enum LockedFileDecision
{
    /// <summary>Try to replace the file again.</summary>
    Retry,

    /// <summary>Leave the existing file untouched and continue.</summary>
    Skip,

    /// <summary>Fail the update.</summary>
    Abort,
}
