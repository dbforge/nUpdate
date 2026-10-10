using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using nUpdate.Installer;
using nUpdate.UpdateInstaller.UI.Avalonia;

namespace nUpdate.Tests.InstallerUi;

public class InstallerWindowTests
{
    private static T Find<T>(Window window, string name) where T : Control =>
        window.FindControl<T>(name).ShouldNotBeNull();

    [AvaloniaFact]
    public void Window_ShowsTheProgressAndCannotBeClosedWhileInstalling()
    {
        var viewModel = new InstallerWindowViewModel(InstallerWindowViewModelTests.Session());
        var window = new InstallerWindow(viewModel);
        window.Show();

        window.Title.ShouldBe("Updating Demo");
        Find<TextBlock>(window, "TitleText").Text.ShouldBe("Updating Demo");
        Find<ProgressBar>(window, "Progress").IsIndeterminate.ShouldBeTrue();
        Find<Image>(window, "IconImage").Source.ShouldNotBeNull();

        viewModel.Report(50, "Copying app.dll...");
        Dispatcher.UIThread.RunJobs();
        Find<TextBlock>(window, "StatusText").Text.ShouldBe("Copying app.dll...");
        Find<ProgressBar>(window, "Progress").Value.ShouldBe(50);
        Find<TextBlock>(window, "PercentageText").Text.ShouldBe("50 %");
        Find<StackPanel>(window, "LockedFilePanel").IsVisible.ShouldBeFalse();
        Find<StackPanel>(window, "ErrorPanel").IsVisible.ShouldBeFalse();

        window.Close();
        window.IsVisible.ShouldBeTrue();
        viewModel.Finish();
        window.IsVisible.ShouldBeFalse();
    }

    [AvaloniaFact]
    public void Window_AsksAboutALockedFileWithItsButtons()
    {
        var viewModel = new InstallerWindowViewModel(InstallerWindowViewModelTests.Session());
        var window = new InstallerWindow(viewModel);
        window.Show();
        var answers = new List<LockedFileDecision>();

        viewModel.AskAboutLockedFile("/opt/demo/app.dll", answers.Add);
        Dispatcher.UIThread.RunJobs();
        Find<StackPanel>(window, "LockedFilePanel").IsVisible.ShouldBeTrue();
        Find<TextBlock>(window, "LockedFileText").Text!.ShouldContain("/opt/demo/app.dll");
        Find<Button>(window, "RetryButton").Content.ShouldBe("Retry");
        Find<Button>(window, "SkipButton").Command!.Execute(null);

        answers.ShouldBe([LockedFileDecision.Skip]);
        Dispatcher.UIThread.RunJobs();
        Find<StackPanel>(window, "LockedFilePanel").IsVisible.ShouldBeFalse();
        viewModel.Finish();
    }

    [AvaloniaFact]
    public void Window_ShowsTheErrorWithTheLogFile()
    {
        var viewModel = new InstallerWindowViewModel(InstallerWindowViewModelTests.Session());
        var window = new InstallerWindow(viewModel);
        window.Show();
        var acknowledged = false;

        viewModel.ShowError(new UnauthorizedAccessException("Access to /opt/demo is denied."),
            () => acknowledged = true);
        Dispatcher.UIThread.RunJobs();

        Find<StackPanel>(window, "ErrorPanel").IsVisible.ShouldBeTrue();
        Find<TextBlock>(window, "ErrorCaptionText").Text.ShouldBe("Error while updating the application.");
        Find<SelectableTextBlock>(window, "ErrorMessageText").Text.ShouldBe("Access to /opt/demo is denied.");
        Find<SelectableTextBlock>(window, "LogFileText").Text!.ShouldEndWith("install.log");
        Find<TextBlock>(window, "StatusText").IsEffectivelyVisible.ShouldBeFalse();
        Find<Button>(window, "CloseButton").Command!.Execute(null);
        acknowledged.ShouldBeTrue();
        viewModel.Finish();
    }

    [AvaloniaFact]
    public void Window_UsesTheAccentColorAndTheApplicationsIcon()
    {
        var icon = Path.Combine(Path.GetTempPath(), $"nupdate-icon-{Guid.NewGuid():N}.png");
        using (var source =
               Avalonia.Platform.AssetLoader.Open(
                   new Uri("avares://nUpdate.UpdateInstaller.UI.Avalonia/Assets/nUpdate.png")))
        using (var target = File.Create(icon))
            source.CopyTo(target);
        try
        {
            var viewModel = new InstallerWindowViewModel(InstallerWindowViewModelTests.Session(configure: o =>
            {
                o.Ui.AccentColor = "#2E7D32";
                o.Ui.IconPath = icon;
            }));
            var window = new InstallerWindow(viewModel);
            window.Resources["InstallerAccentBrush"].ShouldBeOfType<SolidColorBrush>().Color
                .ShouldBe(Color.Parse("#2E7D32"));
            window.Resources["AccentButtonBackground"].ShouldBeOfType<SolidColorBrush>().Color
                .ShouldBe(Color.Parse("#2E7D32"));
            new InstallerWindow().Resources["AccentButtonBackground"].ShouldBeOfType<SolidColorBrush>().Color
                .ShouldBe(InstallerWindow.DefaultAccent);
            InstallerWindow.LoadCustomIcon(icon)!.PixelSize.Width.ShouldBe(256);
            window.Icon.ShouldNotBeNull();
        }
        finally
        {
            File.Delete(icon);
        }
    }

    [AvaloniaFact]
    public void LoadIcon_FallsBackToTheNUpdateIcon()
    {
        InstallerWindow.LoadDefaultIcon().PixelSize.Width.ShouldBe(256);
        InstallerWindow.LoadCustomIcon(null).ShouldBeNull();
        InstallerWindow.LoadCustomIcon("/nowhere/icon.png").ShouldBeNull();
        var notAnImage = Path.GetTempFileName();
        try
        {
            File.WriteAllText(notAnImage, "not a png");
            InstallerWindow.LoadCustomIcon(notAnImage).ShouldBeNull();
        }
        finally
        {
            File.Delete(notAnImage);
        }

        Should.Throw<ArgumentNullException>(() => new InstallerWindow(null!));
    }

    [AvaloniaFact]
    public void App_CreatesTheMainWindowOnceThePlatformIsUp()
    {
        var created = new Window();
        var lifetime = new ClassicDesktopStyleApplicationLifetime();
        var app = new App { CreateMainWindow = () => created, ApplicationLifetime = lifetime };
        app.Initialize();
        app.OnFrameworkInitializationCompleted();
        lifetime.MainWindow.ShouldBe(created);
        lifetime.ShutdownMode.ShouldBe(ShutdownMode.OnMainWindowClose);

        new App { CreateMainWindow = () => created }.OnFrameworkInitializationCompleted();
    }
}
