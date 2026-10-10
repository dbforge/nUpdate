using System.IO.Abstractions;
using nUpdate.Platform;
using nUpdate.UpdateInstaller.Abstractions;
using nUpdate.UpdateInstaller.Operations;
using nUpdate.UpdateInstaller.Platform;
using nUpdate.UpdateInstaller.Windows;

namespace nUpdate.UpdateInstaller;

/// <summary>
///     The limits of an installer run, for <see cref="InstallerHost.Run" />. It also holds the system services the
///     installer works with, which nUpdate's tests replace.
/// </summary>
public sealed class InstallerServices
{
    public InstallerServices()
    {
        FileSystem = new FileSystem();
        PackageExtractor = new ZipPackageExtractor(FileSystem, new FilePermissions());
        EnvironmentInfo = new EnvironmentInfo();
        DirectorySwap = new DirectorySwap(FileSystem, EnvironmentInfo.IsMacOS);
    }

    internal IFileSystem FileSystem { get; set; }

    internal IPackageExtractor PackageExtractor { get; set; }

    internal IRegistry Registry { get; set; } = new WindowsRegistry();

    internal IServiceController ServiceController { get; set; } = new WindowsServiceController();

    internal IProcessService ProcessService { get; set; } = new SystemProcessService();

    internal ISpecialFolders SpecialFolders { get; set; } = new SystemSpecialFolders();

    internal IEnvironmentInfo EnvironmentInfo { get; set; }

    /// <summary>Replaces a macOS application bundle as a whole.</summary>
    internal IDirectorySwap DirectorySwap { get; set; }

    /// <summary>Sets the macOS code signature attributes the package manifest stores, which a zip cannot carry.</summary>
    internal ICodeSignatureAttributes CodeSignatures { get; set; } = new CodeSignatureAttributes();

    /// <summary>Receives failures of a windowless run on Windows.</summary>
    internal IEventLog EventLog { get; set; } = new WindowsEventLog();

    /// <summary>Where a windowless run reports failures besides the log file.</summary>
    internal TextWriter ErrorOutput { get; set; } = Console.Error;

    /// <summary>The time written into <c>install.log</c>.</summary>
    internal Func<DateTimeOffset> Clock { get; set; } = () => DateTimeOffset.Now;

    /// <summary>Pauses a windowless run before it retries a locked file.</summary>
    internal Action<TimeSpan> Delay { get; set; } = Thread.Sleep;

    /// <summary>How long to wait for the host application to exit before continuing anyway.</summary>
    public TimeSpan HostExitTimeout { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>How often a locked file is retried before the update is aborted.</summary>
    public int MaxLockedFileAttempts { get; set; } = 5;

    /// <summary>The handlers for the operation areas. One handler per area.</summary>
    internal List<IOperationHandler> OperationHandlers { get; set; } =
    [
        new FileOperationHandler(),
        new RegistryOperationHandler(),
        new ProcessOperationHandler(),
        new ServiceOperationHandler(),
    ];
}
