using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using nUpdate.Tests.Administration.App;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace nUpdate.Tests.Administration.App;

public static class TestAppBuilder
{
    // Real (Skia) drawing so that tests can capture rendered frames; the headless platform still needs no display.
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<nUpdate.Administration.App>().UseSkia().UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
