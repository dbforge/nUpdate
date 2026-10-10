using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Threading;
using nUpdate.Installer;

namespace nUpdate.UpdateInstaller.UI.Avalonia;

/// <summary>Shows the installer window of <see cref="InstallerWindowViewModel" /> on Avalonia's UI thread.</summary>
public sealed class InstallerWindowReporter : WindowProgressReporter
{
    private readonly Action<InstallerWindowViewModel, Action> _runWindow;
    private readonly Action<Action> _post;

    public InstallerWindowReporter(InstallerSession session)
        : this(session, RunAvalonia, action => Dispatcher.UIThread.Post(action))
    {
    }

    /// <param name="session">The options and the log of this run.</param>
    /// <param name="runWindow">Shows the window, calls the action once it is open and returns when it has closed.</param>
    /// <param name="post">Runs an action on the UI thread.</param>
    internal InstallerWindowReporter(InstallerSession session, Action<InstallerWindowViewModel, Action> runWindow,
        Action<Action> post)
        : base(session)
    {
        ViewModel = new InstallerWindowViewModel(session);
        _runWindow = runWindow;
        _post = post;
    }

    public InstallerWindowViewModel ViewModel { get; }

    protected override void RunWindow(Action shown) => _runWindow(ViewModel, shown);

    protected override void Post(Action action) => _post(action);

    protected override void ShowProgress(float progress, string text) => ViewModel.Report(progress, text);

    protected override void AskAboutLockedFile(string filePath, Action<LockedFileDecision> answer) =>
        ViewModel.AskAboutLockedFile(filePath, answer);

    protected override void ShowError(Exception exception, Action closed) => ViewModel.ShowError(exception, closed);

    protected override void Finish() => ViewModel.Finish();

    [ExcludeFromCodeCoverage] // Starts the real Avalonia platform; covered by the published-installer tests under Xvfb and on macOS.
    private static void RunAvalonia(InstallerWindowViewModel viewModel, Action shown)
    {
        InstallerWindow CreateWindow()
        {
            var window = new InstallerWindow(viewModel);
            window.Opened += (_, _) => shown();
            return window;
        }

        AppBuilder.Configure(() => new App { CreateMainWindow = CreateWindow })
            .UsePlatformDetect()
            .WithInterFont()
            .StartWithClassicDesktopLifetime([]);
    }
}
