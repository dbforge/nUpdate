using Avalonia.Media;
using nUpdate.Installer;
using nUpdate.UpdateInstaller;
using nUpdate.UpdateInstaller.UI.Avalonia;

namespace nUpdate.Tests.InstallerUi;

public class InstallerWindowViewModelTests
{
    internal static InstallerSession Session(string? logPath = "/tmp/nUpdate Installer/App/install.log",
        Action<InstallerOptions>? configure = null)
    {
        var options = new InstallerOptions
        {
            Packages = [new InstallerPackage { Path = "/tmp/nUpdate/App/1.1.0.zip" }],
            Application = new ApplicationOptions
            { Name = "Demo", Directory = "/opt/demo", ExecutablePath = "/opt/demo/demo" },
        };
        configure?.Invoke(options);
        return new InstallerSession(options, logPath);
    }

    [Fact]
    public void Constructor_StartsIndeterminateWithTheLocalizedTexts()
    {
        var viewModel = new InstallerWindowViewModel(Session(configure: o =>
        {
            o.Texts[nameof(InstallerText.WindowTitle)] = "{0} wird aktualisiert";
            o.Texts[nameof(InstallerText.RetryButton)] = "Wiederholen";
        }));

        viewModel.Title.ShouldBe("Demo wird aktualisiert");
        viewModel.Status.ShouldBe("Extracting files...");
        viewModel.IsIndeterminate.ShouldBeTrue();
        viewModel.PercentageText.ShouldBe("");
        viewModel.IsAskingAboutLockedFile.ShouldBeFalse();
        viewModel.HasFailed.ShouldBeFalse();
        viewModel.RetryText.ShouldBe("Wiederholen");
        viewModel.SkipText.ShouldBe("Skip");
        viewModel.AbortText.ShouldBe("Abort");
        viewModel.CloseText.ShouldBe("Close");
        viewModel.ErrorCaption.ShouldBe("Error while updating the application.");
        viewModel.LogFileText.ShouldBe("The log file contains the details: /tmp/nUpdate Installer/App/install.log");
        viewModel.IconPath.ShouldBeNull();
        viewModel.AccentColor.ShouldBeNull();
        viewModel.CanClose.ShouldBeFalse();
        Should.Throw<ArgumentNullException>(() => new InstallerWindowViewModel(null!));
    }

    [Fact]
    public void Constructor_TakesTheIconAndTheAccentColorFromTheOptions()
    {
        var viewModel = new InstallerWindowViewModel(Session(logPath: null, configure: o =>
        {
            o.Ui.IconPath = "/tmp/icon.png";
            o.Ui.AccentColor = "#FF336699";
        }));

        viewModel.IconPath.ShouldBe("/tmp/icon.png");
        viewModel.AccentColor.ShouldBe(Color.FromRgb(0x33, 0x66, 0x99));
        viewModel.LogFileText.ShouldBeNull();
        new InstallerWindowViewModel(Session(configure: o => o.Ui.AccentColor = "not a color")).AccentColor
            .ShouldBeNull();
    }

    [Fact]
    public void Report_ShowsStatusAndPercentage()
    {
        var viewModel = new InstallerWindowViewModel(Session());
        var changed = new List<string?>();
        viewModel.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        viewModel.Report(0, "Waiting for Demo to close...");
        viewModel.IsIndeterminate.ShouldBeTrue();
        viewModel.Status.ShouldBe("Waiting for Demo to close...");

        viewModel.Report(42.4f, "Copying app.dll...");
        viewModel.IsIndeterminate.ShouldBeFalse();
        viewModel.Progress.ShouldBe(42.4f, 0.001);
        viewModel.PercentageText.ShouldBe("42 %");
        viewModel.Report(140, "done");
        viewModel.Progress.ShouldBe(100);
        viewModel.Report(-3, "odd");
        viewModel.Progress.ShouldBe(0);
        viewModel.IsIndeterminate.ShouldBeFalse(); // once determinate, it stays so

        changed.ShouldContain(nameof(InstallerWindowViewModel.Status));
        changed.ShouldContain(nameof(InstallerWindowViewModel.PercentageText));
        changed.ShouldContain(nameof(InstallerWindowViewModel.IsIndeterminate));
    }

    [Theory]
    [InlineData(LockedFileDecision.Retry)]
    [InlineData(LockedFileDecision.Skip)]
    [InlineData(LockedFileDecision.Abort)]
    public void AskAboutLockedFile_AsksAndAnswersOnce(LockedFileDecision decision)
    {
        var viewModel = new InstallerWindowViewModel(Session());
        var answers = new List<LockedFileDecision>();

        viewModel.AskAboutLockedFile("/opt/demo/app.dll", answers.Add);
        viewModel.IsAskingAboutLockedFile.ShouldBeTrue();
        viewModel.LockedFileMessage.ShouldBe(
            "The installer cannot overwrite the file '/opt/demo/app.dll' because it is being used by another process. Close the applications that block it and try again.");

        var command = decision switch
        {
            LockedFileDecision.Retry => viewModel.RetryCommand,
            LockedFileDecision.Skip => viewModel.SkipCommand,
            _ => viewModel.AbortCommand,
        };
        command.CanExecute(null).ShouldBeTrue();
        command.Execute(null);
        command.Execute(null);

        answers.ShouldBe([decision]);
        viewModel.IsAskingAboutLockedFile.ShouldBeFalse();
        Should.Throw<ArgumentNullException>(() => viewModel.AskAboutLockedFile("/f", null!));
    }

    [Fact]
    public void ShowError_StaysVisibleAndIsAcknowledgedOnce()
    {
        var viewModel = new InstallerWindowViewModel(Session());
        var acknowledged = 0;
        viewModel.AskAboutLockedFile("/f", _ => { });

        viewModel.ShowError(new IOException("The disk is full."), () => acknowledged++);
        viewModel.HasFailed.ShouldBeTrue();
        viewModel.IsAskingAboutLockedFile.ShouldBeFalse();
        viewModel.ErrorMessage.ShouldBe("The disk is full.");

        viewModel.CloseCommand.Execute(null);
        viewModel.CloseCommand.Execute(null);
        acknowledged.ShouldBe(1);
        viewModel.HasFailed.ShouldBeTrue();

        Should.Throw<ArgumentNullException>(() => viewModel.ShowError(null!, () => { }));
        Should.Throw<ArgumentNullException>(() => viewModel.ShowError(new IOException(), null!));
    }

    [Fact]
    public void Finish_AllowsClosingAndAsksForIt()
    {
        var viewModel = new InstallerWindowViewModel(Session());
        var requests = 0;
        viewModel.Finish();
        viewModel.CloseRequested += (_, _) => requests++;
        viewModel.Finish();
        viewModel.CanClose.ShouldBeTrue();
        requests.ShouldBe(1);
    }
}
