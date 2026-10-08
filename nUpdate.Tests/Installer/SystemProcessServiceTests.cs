using System.Diagnostics;
using nUpdate.Tests.Support;
using nUpdate.UpdateInstaller.Platform;

namespace nUpdate.Tests.Installer;

/// <summary>
///     Exercises an adapter that touches the real machine. It is excluded from coverage and runs on the Windows CI job;
///     the engine and the operation handlers are tested against substitutes everywhere.
/// </summary>
public class SystemProcessServiceTests
{
    [WindowsFact]
    public void SystemProcessService_StartsWaitsForAndKillsProcesses()
    {
        var service = new SystemProcessService();
        service.WaitForExit(int.MaxValue - 1, TimeSpan.FromSeconds(1)).ShouldBeTrue();

        // A private copy of ping.exe gives the process a unique name, so Kill cannot hit anything else on the machine.
        var target = Path.Combine(Path.GetTempPath(), $"nUpdateKillTarget-{Guid.NewGuid():N}.exe");
        File.Copy(Path.Combine(Environment.SystemDirectory, "ping.exe"), target);
        try
        {
            using var process = Process.Start(new ProcessStartInfo(target, "-n 60 127.0.0.1") { CreateNoWindow = true, UseShellExecute = false })!;
            service.WaitForExit(process.Id, TimeSpan.FromMilliseconds(200)).ShouldBeFalse();
            service.Kill(Path.GetFileNameWithoutExtension(target));
            service.WaitForExit(process.Id, TimeSpan.FromSeconds(10)).ShouldBeTrue();
            process.HasExited.ShouldBeTrue();
        }
        finally
        {
            try
            {
                File.Delete(target);
            }
            catch (IOException)
            {
                // Still being torn down by the OS; the temp folder is cleaned up eventually.
            }
        }

        service.Start("cmd.exe", "/c exit 0").ShouldBeTrue();
    }
}
