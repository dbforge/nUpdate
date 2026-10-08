using nUpdate.Platform;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Library;

/// <summary>Verifies the real process adapter on Windows; the unit suite substitutes it everywhere else.</summary>
public class ProcessLauncherTests
{
    [WindowsFact]
    public void Start_StartsAProcessWithoutElevation()
    {
        new ProcessLauncher().Start("cmd.exe", "/c exit 0", elevated: false).ShouldBeTrue();
    }
}
