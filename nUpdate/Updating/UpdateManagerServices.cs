using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using nUpdate.Platform;

namespace nUpdate.Updating;

/// <summary>The dependencies of <see cref="UpdateManager" />; every one has a production default.</summary>
public sealed class UpdateManagerServices
{
    /// <summary>
    ///     The HTTP client to use. When <c>null</c>, the manager creates one from its proxy and credential settings on
    ///     first use and disposes it with the manager.
    /// </summary>
    public HttpClient? HttpClient { get; set; }

    public IFileSystem FileSystem { get; set; } = new FileSystem();

    public ISystemInformation SystemInformation { get; set; } = new SystemInformation();

    public IApplicationInfo ApplicationInfo { get; set; } = new EntryAssemblyApplicationInfo();

    public IProcessLauncher ProcessLauncher { get; set; } = new ProcessLauncher();

    public IFilePermissions FilePermissions { get; set; } = new FilePermissions();

    public IApplicationTerminator ApplicationTerminator { get; set; } = new EnvironmentExitTerminator();

    /// <summary>Receives what is not worth failing an update for: a statistics report that could not be delivered, a download that could not be cleaned up.</summary>
    public ILogger Logger { get; set; } = NullLogger.Instance;
}
