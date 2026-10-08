using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class InstallerServicesTests
{
    [Fact]
    public void Constructor_DefaultsToTheProductionImplementations()
    {
        var services = new InstallerServices();
        services.FileSystem.ShouldBeOfType<System.IO.Abstractions.FileSystem>();
        services.PackageExtractor.ShouldBeOfType<ZipPackageExtractor>();
        services.Registry.ShouldBeOfType<nUpdate.UpdateInstaller.Windows.WindowsRegistry>();
        services.ServiceController.ShouldBeOfType<nUpdate.UpdateInstaller.Windows.WindowsServiceController>();
        services.ProcessService.ShouldBeOfType<nUpdate.UpdateInstaller.Platform.SystemProcessService>();
        services.SpecialFolders.ShouldBeOfType<nUpdate.UpdateInstaller.Platform.SystemSpecialFolders>();
        services.EnvironmentInfo.ShouldBeOfType<nUpdate.UpdateInstaller.Platform.EnvironmentInfo>();
        services.DirectorySwap.ShouldBeOfType<nUpdate.UpdateInstaller.Platform.DirectorySwap>();
        services.EventLog.ShouldBeOfType<nUpdate.UpdateInstaller.Windows.WindowsEventLog>();
        services.ErrorOutput.ShouldBe(Console.Error);
        services.Clock().ShouldBeInRange(DateTimeOffset.Now.AddMinutes(-1), DateTimeOffset.Now.AddMinutes(1));
        services.OperationHandlers.Count.ShouldBe(4);
        services.MaxLockedFileAttempts.ShouldBe(5);
        services.HostExitTimeout.ShouldBe(TimeSpan.FromMinutes(2));
        var started = DateTime.UtcNow;
        services.Delay(TimeSpan.FromMilliseconds(20));
        (DateTime.UtcNow - started).ShouldBeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(15));
    }
}
