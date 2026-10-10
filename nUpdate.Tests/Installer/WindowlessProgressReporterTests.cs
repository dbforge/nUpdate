using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.Reporting;

namespace nUpdate.Tests.Installer;

public class WindowlessProgressReporterTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void WindowlessProgressReporter_RetriesLockedFilesAndReportsFailures()
    {
        using var reporter = new WindowlessProgressReporter(_services.Services);
        reporter.ReportUnpackingProgress(1, "a");
        reporter.ReportOperationProgress(1, "b");
        reporter.ReportLockedFile("/f", 1).ShouldBe(LockedFileDecision.Retry);
        reporter.ReportLockedFile("/f", 2)
            .ShouldBe(LockedFileDecision.Retry); // the engine's attempt limit ends the retries
        _services.Delays.ShouldBe([TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)]);

        reporter.Fail(new IOException("disk full"));
        _services.ErrorOutput.ToString()
            .ShouldContain("nUpdate could not install the update: System.IO.IOException: disk full");
        _services.EventLog.Received(1).WriteError(Arg.Is<string>(m => m.Contains("disk full")));

        _services.EnvironmentInfo.IsWindows.Returns(false);
        reporter.Fail(new IOException("again"));
        _services.EventLog.ReceivedCalls().Count().ShouldBe(1);

        var broken = Substitute.For<TextWriter>();
        broken.When(w => w.WriteLine(Arg.Any<string>())).Do(_ => throw new IOException("closed"));
        _services.Services.ErrorOutput = broken;
        reporter.Fail(new IOException("unwritable"));

        Should.Throw<ArgumentNullException>(() => reporter.Fail(null!));
        Should.Throw<ArgumentNullException>(() => new WindowlessProgressReporter(null!));
    }

    [Fact]
    public async Task Initialize_BlocksUntilTerminate()
    {
        using var reporter = new WindowlessProgressReporter(_services.Services);
        var initialize = Task.Run(() => reporter.Initialize());
        await Task.Delay(50);
        initialize.IsCompleted.ShouldBeFalse();
        reporter.Terminate();
        await initialize.WaitAsync(TimeSpan.FromSeconds(10));
    }
}
