using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.Reporting;

namespace nUpdate.Tests.Installer;

public class FallbackProgressReporterTests
{
    private readonly TestInstallLogs _logs = new();

    [Fact]
    public void FallbackProgressReporter_UsesTheWindowWhileItWorks()
    {
        var window = new RecordingReporter { LockedFileDecision = (_, _) => LockedFileDecision.Retry };
        var fallback = new RecordingReporter();
        var reporter = new FallbackProgressReporter(window, fallback, _logs.Log(null));

        reporter.Initialize();
        reporter.ReportUnpackingProgress(1, "a");
        reporter.ReportOperationProgress(2, "b");
        reporter.ReportLockedFile("/f", 1).ShouldBe(LockedFileDecision.Retry);
        reporter.Fail(new IOException("x"));
        reporter.Terminate();

        window.Initialized.ShouldBeTrue();
        window.Unpacking.Count.ShouldBe(1);
        window.Operations.Count.ShouldBe(1);
        window.Failures.Count.ShouldBe(1);
        window.Terminated.ShouldBe(1);
        fallback.Initialized.ShouldBeFalse();
        fallback.Unpacking.ShouldBeEmpty();
        fallback.Terminated.ShouldBe(1);

        Should.Throw<ArgumentNullException>(() => new FallbackProgressReporter(null!, fallback, _logs.Log(null)));
        Should.Throw<ArgumentNullException>(() => new FallbackProgressReporter(window, null!, _logs.Log(null)));
        Should.Throw<ArgumentNullException>(() => new FallbackProgressReporter(window, fallback, null!));
    }

    [Fact]
    public void Initialize_SwitchesForGoodWhenTheWindowCannotOpen()
    {
        _logs.FileSystem.AddDirectory("/tmp");
        var window = Substitute.For<IProgressReporter>();
        window.When(w => w.Initialize()).Do(_ => throw new InvalidOperationException("XOpenDisplay failed"));
        window.When(w => w.Terminate()).Do(_ => throw new InvalidOperationException("no window"));
        var fallback = new RecordingReporter { LockedFileDecision = (_, _) => LockedFileDecision.Skip };
        var reporter = new FallbackProgressReporter(window, fallback, _logs.Log());

        reporter.Initialize();
        fallback.Initialized.ShouldBeTrue();
        reporter.ReportUnpackingProgress(1, "a");
        reporter.ReportLockedFile("/f", 1).ShouldBe(LockedFileDecision.Skip);
        reporter.Terminate();

        window.DidNotReceive().ReportUnpackingProgress(Arg.Any<float>(), Arg.Any<string>());
        fallback.Unpacking.Single().Text.ShouldBe("a");
        fallback.Terminated.ShouldBe(1);
        var log = _logs.FileSystem.GetFile("/tmp/install.log").TextContents;
        log.ShouldContain(
            "The installer window failed; continuing without it: System.InvalidOperationException: XOpenDisplay failed");
        log.ShouldContain("The installer window could not be closed");
    }

    [Fact]
    public void FallbackProgressReporter_SwitchesWhenAWindowCallThrows()
    {
        var window = Substitute.For<IProgressReporter>();
        window.When(w => w.ReportOperationProgress(Arg.Any<float>(), Arg.Any<string>()))
            .Do(_ => throw new InvalidOperationException("window gone"));
        window.ReportLockedFile(Arg.Any<string>(), Arg.Any<int>()).Returns(_ => throw new InvalidOperationException());
        var fallback = new RecordingReporter();
        var reporter = new FallbackProgressReporter(window, fallback, _logs.Log(null));

        reporter.ReportUnpackingProgress(1, "a");
        reporter.ReportLockedFile("/f", 1).ShouldBe(LockedFileDecision.Abort); // the recording fallback aborts
        reporter.ReportOperationProgress(2, "b");
        reporter.Fail(new IOException("x"));
        reporter.Initialize();

        fallback.LockedFiles.Single().ShouldBe(("/f", 1));
        fallback.Operations.Single().Text.ShouldBe("b");
        fallback.Failures.Count.ShouldBe(1);
        fallback.Initialized.ShouldBeTrue();
        window.DidNotReceive().ReportOperationProgress(Arg.Any<float>(), Arg.Any<string>());
        window.DidNotReceive().Fail(Arg.Any<Exception>());
        window.DidNotReceive().Initialize(); // already switched: straight to the fallback
    }

    [Fact]
    public void Initialize_ContinuesInTheFallbackWhenTheWindowClosesAfterFailing()
    {
        var window = Substitute.For<IProgressReporter>();
        var fallback = new RecordingReporter();
        var reporter = new FallbackProgressReporter(window, fallback, _logs.Log(null));
        window.When(w => w.ReportOperationProgress(Arg.Any<float>(), Arg.Any<string>()))
            .Do(_ => throw new InvalidOperationException("window gone"));
        window.When(w => w.Initialize()).Do(_ => reporter.ReportOperationProgress(5, "while the window was up"));

        reporter.Initialize();

        window.Received(1).Initialize();
        fallback.Operations.Single().Text.ShouldBe("while the window was up");
        fallback.Initialized.ShouldBeTrue();
    }
}
