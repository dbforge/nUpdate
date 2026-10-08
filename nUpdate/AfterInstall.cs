namespace nUpdate;

/// <summary>What happens to the host application once the installer has been started.</summary>
public enum AfterInstall
{
    /// <summary>The host application is closed and started again after the update.</summary>
    Restart,

    /// <summary>The host application is closed and stays closed.</summary>
    Close,

    /// <summary>The host application keeps running; the installer waits for nothing.</summary>
    KeepRunning,
}
