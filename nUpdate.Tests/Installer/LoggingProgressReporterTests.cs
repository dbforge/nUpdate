using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.Reporting;

namespace nUpdate.Tests.Installer;

public class LoggingProgressReporterTests
{
    private readonly TestInstallLogs _logs = new();

    [Fact]
    public void LoggingProgressReporter_WritesEverythingAndPassesItOn()
    {
        _logs.FileSystem.AddDirectory("/tmp");
        var inner = new RecordingReporter { LockedFileDecision = (_, _) => LockedFileDecision.Skip };
        var reporter = new LoggingProgressReporter(inner, _logs.Log());

        reporter.Initialize();
        reporter.ReportUnpackingProgress(12.34f, "app.dll");
        reporter.ReportOperationProgress(50f, "Deleting file \"x\"...");
        reporter.ReportLockedFile("/app/app.dll", 2).ShouldBe(LockedFileDecision.Skip);
        reporter.Fail(new InvalidOperationException("broken"));
        reporter.Terminate();

        inner.Initialized.ShouldBeTrue();
        inner.Unpacking.Single().ShouldBe((12.34f, "app.dll"));
        inner.Operations.Single().Text.ShouldBe("Deleting file \"x\"...");
        inner.LockedFiles.Single().ShouldBe(("/app/app.dll", 2));
        inner.Failures.Single().Message.ShouldBe("broken");
        inner.Terminated.ShouldBe(1);
        var lines = _logs.ReadLog("/tmp/install.log").Select(l => l.Substring(32)).ToList();
        lines[0].ShouldBe("[ 12.3%] Copied app.dll");
        lines[1].ShouldBe("[ 50.0%] Deleting file \"x\"...");
        lines[2].ShouldBe("\"/app/app.dll\" is in use (attempt 2): Skip");
        lines[3].ShouldStartWith("Failed: System.InvalidOperationException: broken");
        lines[^1].ShouldBe("Finished.");

        Should.Throw<ArgumentNullException>(() => reporter.Fail(null!));
        Should.Throw<ArgumentNullException>(() => new LoggingProgressReporter(null!, _logs.Log()));
        Should.Throw<ArgumentNullException>(() => new LoggingProgressReporter(inner, null!));
    }
}
