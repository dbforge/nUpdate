using System.Collections.Concurrent;
using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public sealed class WindowProgressReporterTests
{
    private static InstallerSession Session() => new(new InstallerOptions { Application = new ApplicationOptions { Name = "Demo" } }, "/tmp/install.log");

    [Fact]
    public async Task WindowProgressReporter_ShowsTheLatestProgressFromBeforeItOpenedAndEverythingAfterwards()
    {
        using var window = new TestWindow(Session());
        var run = Task.Run(window.Initialize);
        window.ReportOperationProgress(0, "Waiting for Demo to close...");
        window.ReportOperationProgress(0, "Still waiting");
        window.Posts.ShouldBe(0);
        Polling.WaitUntil(() => window.Open is not null);
        window.Open!();

        window.ReportUnpackingProgress(25, "app.dll");
        window.Terminate();
        await run.WaitAsync(TimeSpan.FromSeconds(10));
        window.Shown.ShouldBe(["0: Still waiting", "25: Copying app.dll...", "finished"]);
    }

    [Fact]
    public async Task WindowProgressReporter_WaitsForTheAnswerToALockedFileAndForTheErrorToBeRead()
    {
        using var window = new TestWindow(Session());
        var run = Task.Run(window.Initialize);
        Polling.WaitUntil(() => window.Open is not null);
        window.Open!();

        var decision = Task.Run(() => window.ReportLockedFile("/opt/demo/app.dll", 1));
        Polling.WaitUntil(() => window.Answer is not null);
        decision.IsCompleted.ShouldBeFalse();
        window.Answer!(LockedFileDecision.Skip);
        (await decision.WaitAsync(TimeSpan.FromSeconds(10))).ShouldBe(LockedFileDecision.Skip);

        var failure = Task.Run(() => window.Fail(new IOException("The disk is full.")));
        Polling.WaitUntil(() => window.CloseError is not null);
        failure.IsCompleted.ShouldBeFalse();
        window.CloseError!();
        await failure.WaitAsync(TimeSpan.FromSeconds(10));

        window.Terminate();
        await run.WaitAsync(TimeSpan.FromSeconds(10));
        window.Shown.ShouldBe(["locked /opt/demo/app.dll", "error The disk is full.", "finished"]);
    }

    [Fact]
    public async Task Initialize_ClosesTheWindowWhenItOpensAfterTheInstallerFinished()
    {
        using var window = new TestWindow(Session());
        window.Terminate();
        var run = Task.Run(window.Initialize);
        Polling.WaitUntil(() => window.Open is not null);
        window.Open!();
        await run.WaitAsync(TimeSpan.FromSeconds(10));
        window.Shown.ShouldBe(["finished"]);
    }

    [Fact]
    public async Task ReportLockedFile_ReleasesAQuestionWhenTheWindowGoesAway()
    {
        using var window = new TestWindow(Session());
        var run = Task.Run(window.Initialize);
        Polling.WaitUntil(() => window.Open is not null);
        window.Open!();
        var question = Task.Run(() => window.ReportLockedFile("/f", 1));
        Polling.WaitUntil(() => window.Answer is not null);

        window.GoAway(); // the display server goes away; nobody answers
        (await Should.ThrowAsync<InvalidOperationException>(() => question.WaitAsync(TimeSpan.FromSeconds(10)))).Message.ShouldBe("The installer window was closed.");
        await run.WaitAsync(TimeSpan.FromSeconds(10));
        Should.Throw<InvalidOperationException>(() => window.Fail(new IOException()));
    }

    [Fact]
    public void WindowProgressReporter_ReleasesWaitingCallsWhenItCannotOpen()
    {
        using var window = new TestWindow(Session(), new InvalidOperationException("XOpenDisplay failed"));
        Should.Throw<InvalidOperationException>(window.Initialize).Message.ShouldBe("XOpenDisplay failed");

        Should.Throw<InvalidOperationException>(() => window.ReportLockedFile("/f", 1)).Message.ShouldBe("The installer window is not open.");
        Should.Throw<InvalidOperationException>(() => window.Fail(new IOException()));
        window.ReportOperationProgress(1, "ignored");
        window.Terminate();
        window.Posts.ShouldBe(0);
        Should.Throw<ArgumentNullException>(() => new TestWindow(null!));
    }

    /// <summary>A window that opens when the test says so, records what it shows and closes when it finishes; posts run at once.</summary>
    private sealed class TestWindow(InstallerSession session, Exception? openFailure = null) : WindowProgressReporter(session)
    {
        private readonly ManualResetEventSlim _closed = new();
        private readonly ConcurrentQueue<string> _shown = new();
        private int _posts;

        public Action? Open { get; private set; }

        public Action<LockedFileDecision>? Answer { get; private set; }

        public Action? CloseError { get; private set; }

        public int Posts => _posts;

        public string[] Shown => _shown.ToArray();

        public void GoAway() => _closed.Set();

        protected override void RunWindow(Action shown)
        {
            if (openFailure is not null)
                throw openFailure;
            Open = shown;
            _closed.Wait();
        }

        protected override void Post(Action action)
        {
            Interlocked.Increment(ref _posts);
            action();
        }

        protected override void ShowProgress(float progress, string text) => _shown.Enqueue($"{progress}: {text}");

        protected override void AskAboutLockedFile(string filePath, Action<LockedFileDecision> answer)
        {
            _shown.Enqueue("locked " + filePath);
            Answer = answer;
        }

        protected override void ShowError(Exception exception, Action closed)
        {
            _shown.Enqueue("error " + exception.Message);
            CloseError = closed;
        }

        protected override void Finish()
        {
            _shown.Enqueue("finished");
            _closed.Set();
        }
    }
}
