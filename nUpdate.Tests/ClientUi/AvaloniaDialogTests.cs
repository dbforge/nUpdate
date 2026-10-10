using System.IO.Abstractions.TestingHelpers;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using nUpdate.Tests.Library.Support;
using nUpdate.Tests.Support;
using nUpdate.Ui;
using nUpdate.UI.Avalonia;
using nUpdate.UI.Avalonia.ViewModels;
using nUpdate.UI.Avalonia.Views;
using nUpdate.Updating;

namespace nUpdate.Tests.ClientUi;

/// <summary>The dialogs of nUpdate.UI.Avalonia, driven the way a user would.</summary>
public sealed class AvaloniaDialogTests : IDisposable
{
    private const string FeedUri = "https://h/u/nupdate.json";
    private readonly TestServices _services = new();
    private readonly UpdateManager _manager;

    public AvaloniaDialogTests()
    {
        _manager = new UpdateManager(new Uri(FeedUri), TestKeys.PublicKey, services: _services.Build());
        var root = _services.FileSystem.Path.GetPathRoot(_services.FileSystem.Path.GetTempPath())!;
        _services.FileSystem.AddDrive(root, new MockDriveData { AvailableFreeSpace = 10_000_000 });
    }

    public void Dispose() => _manager.Dispose();

    private void ServeUpdate()
    {
        var payload = new byte[300];
        new Random(1).NextBytes(payload);
        var package = TestPackages.Build("1.1.0", Guid.Empty, payload);
        _services.Http.Bytes("https://h/u/packages/1.1.0/any.zip", package);
        var info = new PackageInfo
        {
            Version = new UpdateVersion("1.1.0"),
            Changelog = { ["en"] = "Faster start." },
            Files =
            [
                new PackageFile
                {
                    Path = "packages/1.1.0/any.zip", Size = package.Length, Sha512 = TestKeys.Sha512(package),
                    Signature = new PackageSignature { Value = TestKeys.Sign(package) },
                    Touches = [nUpdate.Operations.OperationArea.Processes]
                }
            ],
        };
        _services.Http.Text(HttpMethod.Get, FeedUri, Serializer.Serialize(new UpdateFeed { Packages = [info] }));
    }

    private void ServeNoUpdates() =>
        _services.Http.Text(HttpMethod.Get, FeedUri, Serializer.Serialize(new UpdateFeed()));

    private static T Find<T>(Window window, string name) where T : Control =>
        window.GetVisualDescendants().OfType<T>().Single(c => c.Name == name);

    private static async Task<T> WaitFor<T>(Func<T?> value) where T : class
    {
        for (var i = 0; i < 500; i++)
        {
            Dispatcher.UIThread.RunJobs();
            if (value() is { } found)
                return found;
            await Task.Delay(5);
        }

        throw new TimeoutException();
    }

    [AvaloniaFact]
    public async Task UpdaterUI_SearchesConfirmsDownloadsAndStartsTheInstaller()
    {
        ServeUpdate();
        _services.AddInstaller();
        var owner = new Window();
        owner.Show();
        var ui = new UpdaterUi(_manager, owner);
        var dialogs = new List<UpdateDialog>();
        ui.Presenter.DialogShown = dialogs.Add;

        var run = ui.RunAsync();
        var confirm = await WaitFor(() => dialogs.FirstOrDefault(d => d.DataContext is NewUpdateDialogViewModel));
        Dispatcher.UIThread.RunJobs();
        Find<TextBlock>(confirm, "HeaderText").Text.ShouldBe("1 new update available.");
        Find<TextBlock>(confirm, "TouchesText").Text.ShouldBe("Accesses: Processes");
        Find<TextBlock>(confirm, "AfterInstallText").IsVisible.ShouldBeFalse();
        Find<TextBox>(confirm, "ChangelogBox").Text!.ShouldContain("Faster start.");
        Find<Button>(confirm, "InstallButton").Command!.Execute(null);

        (await run.WaitAsync(TimeSpan.FromSeconds(30))).ShouldBe(UpdateFlowResult.InstallerStarted);
        dialogs.Select(d => d.DataContext!.GetType().Name).ShouldBe([
            nameof(SearchDialogViewModel), nameof(NewUpdateDialogViewModel), nameof(DownloadDialogViewModel)
        ]);
        dialogs.ShouldAllBe(d => !d.IsVisible);
        _services.ProcessLauncher.ReceivedCalls().Count().ShouldBe(1);
        ui.UseHiddenSearch.ShouldBeFalse();
    }

    [AvaloniaFact]
    public async Task UpdaterUI_ShowsThatTheApplicationIsUpToDateWithoutAnOwner()
    {
        ServeNoUpdates();
        var ui = new UpdaterUi(_manager);
        var dialogs = new List<UpdateDialog>();
        ui.Presenter.DialogShown = dialogs.Add;

        var run = ui.RunAsync();
        var message = await WaitFor(() => dialogs.FirstOrDefault(d => d.DataContext is MessageDialogViewModel));
        Dispatcher.UIThread.RunJobs();
        Find<TextBlock>(message, "CaptionText").Text.ShouldBe("There are no new updates available.");
        Find<Expander>(message, "DetailsExpander").IsVisible.ShouldBeFalse();
        Find<Button>(message, "CloseButton").Command!.Execute(null);

        (await run.WaitAsync(TimeSpan.FromSeconds(30))).ShouldBe(UpdateFlowResult.NoUpdates);
        message.Accepted.ShouldBe(true);

        ui.UseHiddenSearch = true;
        dialogs.Clear();
        (await ui.RunAsync()).ShouldBe(UpdateFlowResult.NoUpdates);
        dialogs.ShouldBeEmpty();
        Should.Throw<ArgumentNullException>(() => new UpdaterUi(null!));
    }

    [AvaloniaFact]
    public async Task UpdaterUI_ShowsErrorsWithTheirDetailsAndLetsTheUserDecline()
    {
        _services.Http.Text(HttpMethod.Get, FeedUri, "{broken");
        var ui = new UpdaterUi(_manager);
        var dialogs = new List<UpdateDialog>();
        ui.Presenter.DialogShown = dialogs.Add;

        var run = ui.RunAsync();
        var error = await WaitFor(() => dialogs.FirstOrDefault(d => d.DataContext is MessageDialogViewModel));
        Dispatcher.UIThread.RunJobs();
        var viewModel = (MessageDialogViewModel)error.DataContext!;
        viewModel.Details!.ShouldContain("InvalidFeedException");
        Find<Expander>(error, "DetailsExpander").IsVisible.ShouldBeTrue();
        error.Close();
        (await run.WaitAsync(TimeSpan.FromSeconds(30))).ShouldBe(UpdateFlowResult.Failed);
        error.Accepted.ShouldBeNull(); // closed with the title bar

        ServeUpdate();
        dialogs.Clear();
        run = ui.RunAsync();
        var confirm = await WaitFor(() => dialogs.FirstOrDefault(d => d.DataContext is NewUpdateDialogViewModel));
        Dispatcher.UIThread.RunJobs();
        Find<Button>(confirm, "CancelInstallButton").Command!.Execute(null);
        (await run.WaitAsync(TimeSpan.FromSeconds(30))).ShouldBe(UpdateFlowResult.Declined);
    }

    [AvaloniaFact]
    public async Task SearchDialog_CancelsTheSearchWhenTheUserCloses()
    {
        var started = new TaskCompletionSource();
        using var viewModel = new SearchDialogViewModel(_manager, async token =>
        {
            started.SetResult();
            await Task.Delay(Timeout.Infinite, token);
            return true;
        });
        var dialog = new UpdateDialog(viewModel);
        dialog.Show();
        await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Dispatcher.UIThread.RunJobs();
        Find<ProgressBar>(dialog, "SearchProgress").IsIndeterminate.ShouldBeTrue();
        viewModel.Title.ShouldBe("Searching for updates...");

        dialog.Close(); // the search is still running: the dialog stays and the search stops
        dialog.IsVisible.ShouldBeTrue();
        await Should.ThrowAsync<TaskCanceledException>(() => WaitCompletion(viewModel.Completion));
        await WaitFor(() => dialog.IsVisible ? null : dialog);
        dialog.Accepted.ShouldBe(false);
        viewModel.CancelCommand.CanExecute(null).ShouldBeTrue();
        viewModel.CancelCommand.Execute(null); // after the search nothing is left to cancel
        Should.Throw<ArgumentNullException>(() => new SearchDialogViewModel(_manager, null!));
    }

    private static async Task WaitCompletion(Task task)
    {
        for (var i = 0; i < 500 && !task.IsCompleted; i++)
        {
            Dispatcher.UIThread.RunJobs();
            await Task.Delay(5);
        }

        await task;
    }

    [AvaloniaFact]
    public async Task DownloadDialog_ShowsTheProgressAndCanBeCancelled()
    {
        using var viewModel = new DownloadDialogViewModel(_manager, async (progress, token) =>
        {
            progress.Report(new UpdateDownloadProgress(50, 200));
            await Task.Delay(Timeout.Infinite, token);
        });
        viewModel.InfoText.ShouldNotContain("\n");
        var dialog = new UpdateDialog(viewModel);
        dialog.Show();
        await WaitFor(() => viewModel.Progress > 0 ? viewModel : null);
        viewModel.Progress.ShouldBe(25);
        viewModel.InfoText.ShouldContain("25");
        Find<ProgressBar>(dialog, "DownloadProgress").Value.ShouldBe(25);
        viewModel.Title.ShouldBe("Downloading updates...");

        Find<Button>(dialog, "CancelDownloadButton").Command!.Execute(null);
        await Should.ThrowAsync<TaskCanceledException>(() => WaitCompletion(viewModel.Completion));
        await WaitFor(() => dialog.IsVisible ? null : dialog);
        Should.Throw<ArgumentNullException>(() => new DownloadDialogViewModel(_manager, null!));
    }

    [AvaloniaFact]
    public async Task NewUpdateDialog_SummarizesSeveralPackagesWithoutOperationsThatLeaveTheApplicationClosed()
    {
        var feed = new UpdateFeed();
        foreach (var version in new[] { "1.1.0", "1.2.0" })
        {
            feed.Packages.Add(new PackageInfo
            {
                Version = new UpdateVersion(version),
                Necessary = true,
                AfterInstall = version == "1.2.0" ? AfterInstall.Close : null,
                Changelog = { ["en"] = $"Changes in {version}." },
                Files =
                [
                    new PackageFile
                    {
                        Path = $"packages/{version}/any.zip", Size = 1000, Sha512 = "x",
                        Signature = new PackageSignature { Value = "s" }
                    }
                ],
            });
        }

        _services.Http.Text(HttpMethod.Get, FeedUri, Serializer.Serialize(feed));
        (await _manager.CheckForUpdatesAsync()).ShouldBeTrue();

        var viewModel = new NewUpdateDialogViewModel(_manager);
        viewModel.Summary.Header.ShouldBe("2 new updates available.");
        viewModel.Summary.TouchesText.ShouldBe("Accesses: -");
        viewModel.Summary.AvailableVersionsText.ShouldContain("1.1.0");
        viewModel.Summary.ChangelogText.ShouldContain("Changes in 1.2.0.");
        viewModel.InstallCommand.Execute(null); // no dialog listens: nothing happens

        var dialog = new UpdateDialog(viewModel);
        dialog.Show();
        Dispatcher.UIThread.RunJobs();
        var afterInstall = Find<TextBlock>(dialog, "AfterInstallText");
        afterInstall.Text.ShouldBe("TestApp stays closed after the update.");
        afterInstall.IsVisible.ShouldBeTrue();
        dialog.Close();
    }

    [AvaloniaFact]
    public void Dialogs_HaveTitlesAndCheckTheirArguments()
    {
        new MessageDialogViewModel(_manager, "c", "t").Title.ShouldBe("TestApp");
        new MessageDialogViewModel(_manager, "c", "t").Details.ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => new MessageDialogViewModel(_manager, null!, "t"));
        Should.Throw<ArgumentNullException>(() => new MessageDialogViewModel(_manager, "c", null!));
        Should.Throw<ArgumentNullException>(() => new MessageDialogViewModel(null!, "c", "t"));
        Should.Throw<ArgumentNullException>(() => new UpdateDialog(null!));
        new UpdateDialog().DataContext.ShouldBeNull();
        new UpdateDialog().Resources["AccentButtonBackground"].ShouldBeOfType<Avalonia.Media.SolidColorBrush>().Color
            .ShouldBe(UpdateDialog.DefaultAccent);
        var message = new MessageDialogViewModel(_manager, "c", "t");
        _manager.InstallerAccentColor = "#2E7D32";
        new UpdateDialog(message).Resources["UpdateAccentBrush"].ShouldBeOfType<Avalonia.Media.SolidColorBrush>().Color
            .ShouldBe(Avalonia.Media.Color.Parse("#2E7D32"));
        message.OnClosing().ShouldBeTrue();
        message.Texts.ShouldBe(_manager.Texts);
    }
}
