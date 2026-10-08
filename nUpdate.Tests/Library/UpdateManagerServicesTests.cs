using nUpdate.Platform;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class UpdateManagerServicesTests
{
    [Fact]
    public void Constructor_DefaultsToTheProductionImplementations()
    {
        var services = new UpdateManagerServices();
        services.HttpClient.ShouldBeNull();
        services.FileSystem.ShouldBeOfType<System.IO.Abstractions.FileSystem>();
        services.SystemInformation.ShouldBeOfType<SystemInformation>().RuntimeIdentifier.ShouldBe(PackagePlatform.Current);
        services.ApplicationInfo.ShouldBeOfType<EntryAssemblyApplicationInfo>();
        services.ProcessLauncher.ShouldBeOfType<ProcessLauncher>();
        services.FilePermissions.ShouldBeOfType<FilePermissions>();
        services.ApplicationTerminator.ShouldBeOfType<EnvironmentExitTerminator>();
        services.Logger.ShouldBe(Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
    }
}
