using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace nUpdate.UpdateInstaller.UI.Avalonia;

/// <summary>The installer's application: the Fluent theme in the system's light or dark variant, and one window.</summary>
public sealed class App : Application
{
    /// <summary>Creates the main window once the platform is up; windows cannot be created earlier.</summary>
    public required Func<Window> CreateMainWindow { get; init; }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            desktop.MainWindow = CreateMainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
