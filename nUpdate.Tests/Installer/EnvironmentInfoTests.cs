using nUpdate.UpdateInstaller.Platform;

namespace nUpdate.Tests.Installer;

public class EnvironmentInfoTests
{
    [Theory]
    [InlineData(true, false, false, null, null, true)]
    [InlineData(true, false, true, ":0", null, false)]
    [InlineData(false, true, false, null, null, true)]
    [InlineData(false, false, false, ":0", null, true)]
    [InlineData(false, false, false, null, "wayland-0", true)]
    [InlineData(false, false, false, "", "", false)]
    [InlineData(false, false, false, null, null, false)]
    public void DetectDisplay_FollowsTheSystem(bool windows, bool macOS, bool serviceContext, string? display, string? wayland, bool expected)
    {
        var variables = new Dictionary<string, string?> { ["DISPLAY"] = display, ["WAYLAND_DISPLAY"] = wayland };
        EnvironmentInfo.DetectDisplay(windows, macOS, serviceContext, name => variables[name]).ShouldBe(expected);
    }

    [Fact]
    public void EnvironmentInfo_ReadsTheEnvironment()
    {
        var environment = new EnvironmentInfo();
        environment.IsServiceContext.ShouldBe(!Environment.UserInteractive);
        environment.IsWindows.ShouldBe(OperatingSystem.IsWindows());
        environment.IsMacOS.ShouldBe(OperatingSystem.IsMacOS());
        environment.HasDisplay.ShouldBe(EnvironmentInfo.DetectDisplay(OperatingSystem.IsWindows(), OperatingSystem.IsMacOS(), !Environment.UserInteractive, Environment.GetEnvironmentVariable));
    }
}
