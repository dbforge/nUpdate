using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.ServiceProcess;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller.Windows;

/// <summary>Service control through <see cref="ServiceController" />.</summary>
[SupportedOSPlatform("windows")]
[ExcludeFromCodeCoverage] // Controls real Windows services; verified by the Windows-only test run.
internal sealed class WindowsServiceController : IServiceController
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    public void StartService(string serviceName, string[] arguments)
    {
        using var controller = new ServiceController(serviceName);
        if (controller.Status == ServiceControllerStatus.Running)
        {
            controller.Stop();
            controller.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);
        }

        if (arguments is { Length: > 0 })
            controller.Start(arguments);
        else
            controller.Start();
        controller.WaitForStatus(ServiceControllerStatus.Running, Timeout);
    }

    public void StopService(string serviceName)
    {
        using var controller = new ServiceController(serviceName);
        if (controller.Status == ServiceControllerStatus.Stopped)
            return;
        controller.Stop();
        controller.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);
    }
}
