using System.IO.Abstractions;
using nUpdate.Platform;
using nUpdate.UpdateInstaller.Abstractions;
using nUpdate.UpdateInstaller.Operations;
using nUpdate.UpdateInstaller.Platform;
using nUpdate.UpdateInstaller.Windows;

namespace nUpdate.UpdateInstaller;

/// <summary>The external dependencies of <see cref="InstallEngine" /> and <see cref="InstallerHost" />, with production defaults.</summary>
public sealed class InstallerServices
{
    public InstallerServices()
    {
        FileSystem = new FileSystem();
        PackageExtractor = new ZipPackageExtractor(FileSystem, new FilePermissions());
        EnvironmentInfo = new EnvironmentInfo();
        DirectorySwap = new DirectorySwap(FileSystem, EnvironmentInfo.IsMacOS);
    }

    public IFileSystem FileSystem { get; set; }

    public IPackageExtractor PackageExtractor { get; set; }

    public IRegistry Registry { get; set; } = new WindowsRegistry();

    public IServiceController ServiceController { get; set; } = new WindowsServiceController();

    public IProcessService ProcessService { get; set; } = new SystemProcessService();

    public ISpecialFolders SpecialFolders { get; set; } = new SystemSpecialFolders();

    public IEnvironmentInfo EnvironmentInfo { get; set; }

    /// <summary>Replaces a macOS application bundle as a whole.</summary>
    public IDirectorySwap DirectorySwap { get; set; }

    /// <summary>Receives failures of a windowless run on Windows.</summary>
    public IEventLog EventLog { get; set; } = new WindowsEventLog();

    /// <summary>Where a windowless run reports failures besides the log file.</summary>
    public TextWriter ErrorOutput { get; set; } = Console.Error;

    /// <summary>The time written into <c>install.log</c>.</summary>
    public Func<DateTimeOffset> Clock { get; set; } = () => DateTimeOffset.Now;

    /// <summary>Pauses a windowless run before it retries a locked file.</summary>
    public Action<TimeSpan> Delay { get; set; } = Thread.Sleep;

    /// <summary>How long to wait for the host application to exit before continuing anyway.</summary>
    public TimeSpan HostExitTimeout { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>How often a locked file is retried before the update is aborted.</summary>
    public int MaxLockedFileAttempts { get; set; } = 5;

    /// <summary>The handlers for the operation areas. One handler per area.</summary>
    public List<IOperationHandler> OperationHandlers { get; set; } =
    [
        new FileOperationHandler(),
        new RegistryOperationHandler(),
        new ProcessOperationHandler(),
        new ServiceOperationHandler(),
    ];
}
