using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.UI.Avalonia;

namespace nUpdate.Tests.InstallerUi;

public sealed class InstallerWindowReporterTests : IDisposable
{
    private readonly ManualResetEventSlim _closed = new();
    private readonly List<Action> _posted = [];
    private Action? _shown;

    public void Dispose() => _closed.Dispose();

    /// <summary>A window that opens when the test says so and closes when the view model asks for it; posts run at once.</summary>
    private InstallerWindowReporter Reporter() =>
        new(InstallerWindowViewModelTests.Session(), (viewModel, shown) =>
        {
            viewModel.CloseRequested += (_, _) => _closed.Set();
            _shown = shown;
            _closed.Wait();
        }, action =>
        {
            lock (_posted)
                _posted.Add(action);
            action();
        });

    [Fact]
    public async Task InstallerWindowReporter_DropsProgressBeforeTheWindowIsOpenAndPostsItAfterwards()
    {
        using var reporter = Reporter();
        var window = Task.Run(() => reporter.Initialize());
        reporter.ReportOperationProgress(10, "too early");
        _posted.ShouldBeEmpty();
        Polling.WaitUntil(() => _shown is not null);
        _shown!();

        reporter.ReportUnpackingProgress(25, "app.dll");
        reporter.ViewModel.Status.ShouldBe("Copying app.dll...");
        reporter.ReportOperationProgress(50, "Deleting file \"x\"...");
        reporter.ViewModel.Status.ShouldBe("Deleting file \"x\"...");
        reporter.ViewModel.Progress.ShouldBe(50);

        reporter.Terminate();
        await window.WaitAsync(TimeSpan.FromSeconds(10));
        reporter.ViewModel.CanClose.ShouldBeTrue();
    }

    [Fact]
    public async Task InstallerWindowReporter_WaitsForTheAnswerToALockedFileAndForTheErrorToBeClosed()
    {
        using var reporter = Reporter();
        var window = Task.Run(() => reporter.Initialize());
        Polling.WaitUntil(() => _shown is not null);
        _shown!();

        var decision = Task.Run(() => reporter.ReportLockedFile("/opt/demo/app.dll", 1));
        Polling.WaitUntil(() => reporter.ViewModel.IsAskingAboutLockedFile);
        decision.IsCompleted.ShouldBeFalse();
        reporter.ViewModel.RetryCommand.Execute(null);
        (await decision.WaitAsync(TimeSpan.FromSeconds(10))).ShouldBe(LockedFileDecision.Retry);

        var failure = Task.Run(() => reporter.Fail(new IOException("The disk is full.")));
        Polling.WaitUntil(() => reporter.ViewModel.HasFailed);
        failure.IsCompleted.ShouldBeFalse();
        reporter.ViewModel.ErrorMessage.ShouldBe("The disk is full.");
        reporter.ViewModel.CloseCommand.Execute(null);
        await failure.WaitAsync(TimeSpan.FromSeconds(10));

        reporter.Terminate();
        await window.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Constructor_CreatesTheViewModel()
    {
        var session = InstallerWindowViewModelTests.Session();
        using var production = new InstallerWindowReporter(session);
        production.ViewModel.Title.ShouldBe("Updating Demo");
    }
}
