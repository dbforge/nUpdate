using nUpdate.Tests.Support;
using nUpdate.UpdateInstaller.Windows;

namespace nUpdate.Tests.Installer;

/// <summary>
///     Exercises an adapter that touches the real machine. It is excluded from coverage and runs on the Windows CI job;
///     the engine and the operation handlers are tested against substitutes everywhere.
/// </summary>
public class WindowsServiceControllerTests
{
    [WindowsFact]
    public void WindowsServiceController_ReportsUnknownServices()
    {
        var controller = new WindowsServiceController();
        Should.Throw<InvalidOperationException>(() => controller.StopService("nUpdate-no-such-service-" + Guid.NewGuid().ToString("N")));
        Should.Throw<InvalidOperationException>(() => controller.StartService("nUpdate-no-such-service-" + Guid.NewGuid().ToString("N"), []));
    }
}
