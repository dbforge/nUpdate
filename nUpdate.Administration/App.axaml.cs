using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using nUpdate.Administration.Services;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;

namespace nUpdate.Administration;

[ExcludeFromCodeCoverage] // Composition root.
public sealed class App : Application
{
    private DialogService? _dialogs;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = AppServices.Build();
            var window = new MainWindow { DataContext = services.GetRequiredService<MainWindowViewModel>() };
            var dialogs = _dialogs = services.GetRequiredService<DialogService>();
            dialogs.Owner = window;
            ReportUnhandledExceptions(dialogs);
            desktop.MainWindow = window;
            var initialProject =
                desktop.Args?.FirstOrDefault(arg => arg.EndsWith(".nupdproj", StringComparison.OrdinalIgnoreCase));
            window.Opened += async (_, _) =>
                await services.GetRequiredService<MainWindowViewModel>().InitializeAsync(initialProject);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnAboutClicked(object? sender, EventArgs e) =>
        _ = _dialogs?.ShowInfoAsync("nUpdate Administration",
            $"Version {AdministrationVersion.Text}. Publishes updates for applications that use nUpdate on Windows, Linux and macOS.\n\nhttps://github.com/dbforge/nUpdate");

    /// <summary>Whatever escapes a command or a forgotten task is shown instead of taking the application down.</summary>
    private static void ReportUnhandledExceptions(DialogService dialogs)
    {
        Avalonia.Threading.Dispatcher.UIThread.UnhandledException += (_, e) =>
        {
            e.Handled = true;
            _ = dialogs.ShowErrorAsync("Unexpected error", e.Exception.Message);
        };
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            e.SetObserved();
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                _ = dialogs.ShowErrorAsync("Unexpected error", e.Exception.GetBaseException().Message));
        };
    }
}
