using nUpdate.Installer;

namespace nUpdate.UpdateInstaller;

/// <summary>What an installer window gets from <see cref="InstallerHost" />: the options and where the log is written.</summary>
public sealed class InstallerSession(InstallerOptions options, string? logFilePath)
{
    public InstallerOptions Options { get; } = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>The <c>install.log</c> of this run, for error messages; <c>null</c> when none is written.</summary>
    public string? LogFilePath { get; } = logFilePath;
}
