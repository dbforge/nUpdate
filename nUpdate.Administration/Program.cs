using System.Diagnostics.CodeAnalysis;
using Avalonia;

namespace nUpdate.Administration;

[ExcludeFromCodeCoverage] // Process entry point.
internal static class Program
{
    [STAThread]
    public static int Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
