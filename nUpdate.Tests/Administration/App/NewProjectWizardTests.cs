using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Administration.Views.Controls;

namespace nUpdate.Tests.Administration.App;

/// <summary>The new-project wizard: its steps, validation per step, the project folder, the project password and the creation of the project.</summary>
public class NewProjectWizardTests
{
    private readonly AppTestContext _context = new();

    [Fact]
    public async Task NewProject_WalksThroughTheStepsAndValidatesEachOne()
    {
        var viewModel = _context.Factory.Create<NewProjectViewModel>();
        viewModel.Steps.Select(s => s.Title).ShouldBe(["General", "Authentication", "Transfer", "Statistics", "Security"]);
        viewModel.Steps.Select(s => s.Marker).ShouldBe(["1", "2", "3", "4", "5"]);
        viewModel.Steps[0].IsCurrent.ShouldBeTrue();
        viewModel.IsGeneralStep.ShouldBeTrue();
        viewModel.IsLastStep.ShouldBeFalse();
        viewModel.ContinueText.ShouldBe("Continue");
        viewModel.BackCommand.CanExecute(null).ShouldBeFalse();
        viewModel.Folder.ShouldBe(_context.Paths.DefaultProjectsDirectory);

        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Enter a project name.");
        viewModel.Step.ShouldBe(0);

        viewModel.Name = "P";
        viewModel.Folder.ShouldBe(_context.Paths.SuggestedProjectFolder("P"));
        viewModel.UpdateUrl = "https://u.example.com/p";
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBeNull();
        viewModel.Step.ShouldBe(1);
        viewModel.IsAuthenticationStep.ShouldBeTrue();
        viewModel.Steps[0].IsDone.ShouldBeTrue();
        viewModel.Steps[0].Marker.ShouldBe("✓");
        viewModel.Steps[1].IsCurrent.ShouldBeTrue();
        viewModel.BackCommand.CanExecute(null).ShouldBeTrue();

        viewModel.UseHttpAuthentication = true;
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("HTTP authentication");
        viewModel.UseHttpAuthentication = false;
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.IsTransferStep.ShouldBeTrue();

        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Enter the server host name.");
        viewModel.Transfer.Host = "h";
        viewModel.Transfer.Username = "u";
        viewModel.Transfer.Password = "p";
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.IsStatisticsStep.ShouldBeTrue();

        viewModel.Statistics.Enabled = true;
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("database");
        viewModel.Statistics.Enabled = false;
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.IsSecurityStep.ShouldBeTrue();
        viewModel.IsLastStep.ShouldBeTrue();
        viewModel.ContinueText.ShouldBe("Create project");

        viewModel.BackCommand.Execute(null);
        viewModel.Step.ShouldBe(3);
        viewModel.Steps[4].IsCurrent.ShouldBeFalse();
        viewModel.Steps[4].IsDone.ShouldBeFalse();
        viewModel.ErrorMessage = "stale";
        viewModel.Step = 4;
        viewModel.ErrorMessage.ShouldBeNull();

        viewModel.TestConnectionFirst = false;
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("at least 8 characters");
        viewModel.ProjectPassword = "project-pw";
        viewModel.ProjectPasswordConfirmation = "other";
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("The passwords do not match.");
        viewModel.ProjectPasswordConfirmation = "project-pw";
        var result = AppTestContext.Loaded(AppTestContext.NewProject(), new ProjectSecrets());
        _context.Projects.CreateAsync(Arg.Any<NewProjectRequest>(), Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>()).Returns(result);
        await viewModel.ContinueCommand.ExecuteAsync(null);
        viewModel.Result.ShouldBe(result);
        await _context.Projects.Received().CreateAsync(Arg.Is<NewProjectRequest>(r => r.ProjectPassword == "project-pw" && r.Folder == _context.Paths.SuggestedProjectFolder("P")), Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task NewProject_ValidatesAndCreates()
    {
        var viewModel = _context.Factory.Create<NewProjectViewModel>();
        viewModel.Title.ShouldBe("New project");
        viewModel.KeySizes.ShouldBe([2048, 4096, 8192]);
        var closed = new List<bool>();
        viewModel.CloseRequested += (_, a) => closed.Add(a);

        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Enter a project name.");
        viewModel.Name = "a/b";
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("folder name");
        viewModel.Name = "P";
        viewModel.Folder = " ";
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Choose the folder of the project.");
        viewModel.Folder = "/projects/p";
        viewModel.UpdateUrl = "nope";
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("absolute HTTP(S) URL");
        viewModel.UpdateUrl = "https://u.example.com/p";
        viewModel.UseHttpAuthentication = true;
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("HTTP authentication");
        viewModel.HttpUsername = "web";
        viewModel.HttpPassword = "wp";
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Enter the server host name.");

        viewModel.Transfer.Host = "h";
        viewModel.Transfer.Username = "u";
        viewModel.Transfer.Password = "p";
        viewModel.Statistics.Enabled = true;
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("database");
        viewModel.Statistics.Enabled = false;
        viewModel.SaveCredentials = false;

        _context.Projects.TestConnectionAsync(Arg.Any<TransferSettings>(), Arg.Any<TransferCredentials>(), Arg.Any<CancellationToken>()).ThrowsAsync(new TransferException("down"));
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("down");
        closed.ShouldBeEmpty();

        viewModel.TestConnectionFirst = false;
        var result = AppTestContext.Loaded(AppTestContext.NewProject(), new ProjectSecrets());
        _context.Projects.CreateAsync(Arg.Any<NewProjectRequest>(), Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>()).Returns(result);
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.Result.ShouldBe(result);
        closed.ShouldBe([true]);
        await _context.Projects.Received().CreateAsync(Arg.Is<NewProjectRequest>(r =>
            r.Name == "P" && r.Folder == "/projects/p" && r.HttpAuthentication!.Username == "web" && r.Secrets.HttpAuthenticationPassword == "wp" && r.Secrets.TransferPassword == "p"
            && !r.TestConnection && r.KeySize == 8192 && r.ProjectPassword == null), Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>());

        _context.Projects.CreateAsync(Arg.Any<NewProjectRequest>(), Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>()).ThrowsAsync(new PipelineException("Saving the project", new IOException("full"), []));
        viewModel.UseHttpAuthentication = false;
        viewModel.HttpPassword = "";
        await viewModel.CreateCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("full");
    }

    [Fact]
    public async Task NewProject_FolderFollowsTheNameUntilEdited()
    {
        var viewModel = _context.Factory.Create<NewProjectViewModel>();
        viewModel.Name = "First";
        viewModel.Folder.ShouldBe(_context.Paths.SuggestedProjectFolder("First"));
        viewModel.Name = " Second ";
        viewModel.Folder.ShouldBe(_context.Paths.SuggestedProjectFolder("Second"));

        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns((string?)null);
        await viewModel.BrowseFolderCommand.ExecuteAsync(null);
        viewModel.Folder.ShouldBe(_context.Paths.SuggestedProjectFolder("Second"));
        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns("/custom");
        await viewModel.BrowseFolderCommand.ExecuteAsync(null);
        viewModel.Folder.ShouldBe("/custom");
        viewModel.Name = "Third";
        viewModel.Folder.ShouldBe("/custom");

        var typed = _context.Factory.Create<NewProjectViewModel>();
        typed.Folder = "/typed";
        typed.Name = "Name";
        typed.Folder.ShouldBe("/typed");
    }

    [Fact]
    public void WizardStep_RequiresATitle()
    {
        Should.Throw<ArgumentNullException>(() => new WizardStep(1, null!));
        var step = new WizardStep(2, "Transfer");
        step.Number.ShouldBe(2);
        step.Marker.ShouldBe("2");
        step.IsDone = true;
        step.Marker.ShouldBe("✓");
    }

    [AvaloniaFact]
    public void NewProjectWindow_BindsEditors()
    {
        var viewModel = _context.Factory.Create<NewProjectViewModel>();
        var window = new NewProjectWindow { DataContext = viewModel };
        window.Show();
        window.NameBox.Text = "Typed";
        viewModel.Name.ShouldBe("Typed");
        window.FolderBox.Text.ShouldBe(_context.Paths.SuggestedProjectFolder("Typed"));
        viewModel.ErrorMessage = "problem";
        window.ErrorText.IsVisible.ShouldBeTrue();
        window.ErrorText.Text.ShouldBe("problem");
        var transfer = window.GetVisualDescendants().OfType<TransferSettingsEditor>().Single();
        transfer.HostBox.Text = "ftp.example.com";
        viewModel.Transfer.Host.ShouldBe("ftp.example.com");
        viewModel.Transfer.Protocol = nUpdate.Administration.TransferInterface.TransferProtocol.Sftp;
        transfer.PassiveBox.IsEffectivelyVisible.ShouldBeFalse();
        var statistics = window.GetVisualDescendants().OfType<StatisticsSettingsEditor>().Single();
        statistics.EnabledBox.IsChecked = true;
        viewModel.Statistics.Enabled.ShouldBeTrue();
        statistics.DbNameBox.IsVisible.ShouldBeTrue();
        viewModel.Step = 4;
        window.ProjectPasswordBox.Text = "secret-1";
        viewModel.ProjectPassword.ShouldBe("secret-1");
        window.Close();
    }
}
