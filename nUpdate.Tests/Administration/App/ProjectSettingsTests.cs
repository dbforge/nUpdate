using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.App;

/// <summary>The project settings dialog: validation, saving with the project password, renaming, restoring after a failed save, migrating and deleting the project.</summary>
public class ProjectSettingsTests
{
    private readonly AppTestContext _context = new();

    [Fact]
    public async Task ProjectSettings_SavesRenamesAndDeletes()
    {
        var project = AppTestContext.NewProject();
        project.HttpAuthentication = new HttpAuthenticationSettings { Username = "web" };
        project.Secrets = "blob";
        var secrets = AppTestContext.NewSecrets();
        secrets.HttpAuthenticationPassword = "hp";
        _context.Passwords.GetAsync(project.Id, Arg.Any<CancellationToken>()).Returns("remembered");
        var viewModel = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets);
        viewModel.Title.ShouldBe("Settings of Demo");
        viewModel.Name.ShouldBe("Demo");
        viewModel.Folder.ShouldBe(project.Folder);
        viewModel.UseHttpAuthentication.ShouldBeTrue();
        viewModel.HttpPassword.ShouldBe("hp");
        viewModel.SaveCredentials.ShouldBeTrue();
        viewModel.PasswordHint.ShouldContain("keep the current one");
        viewModel.Transfer.Host.ShouldBe("ftp.example.com");
        var closed = new List<bool>();
        viewModel.CloseRequested += (_, a) => closed.Add(a);

        viewModel.Transfer.Password = "";
        await viewModel.SaveCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Enter the password.");
        viewModel.Transfer.Password = "pw";
        viewModel.UpdateUrl = "nope";
        await viewModel.SaveCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("absolute");

        viewModel.UpdateUrl = "https://new.example.com/u";
        viewModel.Name = "Renamed";
        viewModel.UseHttpAuthentication = false;
        viewModel.AssemblyVersionPath = " /app.exe ";
        viewModel.Statistics.Enabled = true;
        viewModel.Statistics.DatabaseName = "db";
        viewModel.Statistics.DatabaseUsername = "u";
        await viewModel.SaveCommand.ExecuteAsync(null);

        closed.ShouldBe([true]);
        project.UpdateUrl.ShouldBe("https://new.example.com/u/");
        project.HttpAuthentication.ShouldBeNull();
        project.AssemblyVersionPath.ShouldBe("/app.exe");
        project.Statistics.Enabled.ShouldBeTrue();
        secrets.HttpAuthenticationPassword.ShouldBeNull();
        await _context.Projects.Received().SetupStatisticsAsync(project, secrets, Arg.Any<CancellationToken>());
        await _context.Projects.Received().SaveAsync(project, secrets, "remembered", Arg.Any<CancellationToken>());
        await _context.Projects.Received().RenameAsync(project, "Renamed", Arg.Any<CancellationToken>());
        Received.InOrder(() =>
        {
            _context.Projects.RenameAsync(project, "Renamed", Arg.Any<CancellationToken>());
            _context.Projects.SetupStatisticsAsync(project, secrets, Arg.Any<CancellationToken>());
            _context.Projects.SaveAsync(project, secrets, "remembered", Arg.Any<CancellationToken>());
        });

        viewModel.Statistics.Enabled = false;
        viewModel.Name = project.Name;
        viewModel.UpdateUrl = "https://new.example.com/u/";
        viewModel.AssemblyVersionPath = " ";
        viewModel.ProjectPassword = "new-password";
        viewModel.ProjectPasswordConfirmation = "new-password";
        _context.Projects.ClearReceivedCalls();
        await viewModel.SaveCommand.ExecuteAsync(null);
        project.AssemblyVersionPath.ShouldBeNull();
        await _context.Projects.Received().SaveAsync(project, secrets, "new-password", Arg.Any<CancellationToken>());
        await _context.Projects.DidNotReceive().RenameAsync(Arg.Any<UpdateProject>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _context.Projects.DidNotReceive().SetupStatisticsAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>());

        viewModel.SaveCredentials = false;
        _context.Projects.ClearReceivedCalls();
        await viewModel.SaveCommand.ExecuteAsync(null);
        await _context.Projects.Received().SaveAsync(project, secrets, null, Arg.Any<CancellationToken>());

        _context.Dialogs.ConfirmAsync("Delete project", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        await viewModel.DeleteProjectCommand.ExecuteAsync(null);
        viewModel.Deleted.ShouldBeFalse();
        _context.Dialogs.ConfirmAsync("Delete project", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        _context.Dialogs.ConfirmAsync("Delete server files", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        await viewModel.DeleteProjectCommand.ExecuteAsync(null);
        viewModel.Deleted.ShouldBeTrue();
        await _context.Projects.Received().DeleteAsync(project, secrets, true, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Settings_RequireAProjectPasswordWhenNoneIsKnown()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var viewModel = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets);
        viewModel.SaveCredentials.ShouldBeFalse();
        viewModel.PasswordHint.ShouldBe("Choose a project password.");

        viewModel.SaveCredentials = true;
        await viewModel.SaveCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("at least 8 characters");
        viewModel.ProjectPassword = "long-enough";
        viewModel.ProjectPasswordConfirmation = "long-enough";
        await viewModel.SaveCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBeNull();
        await _context.Projects.Received().SaveAsync(project, secrets, "long-enough", Arg.Any<CancellationToken>());

        // A file with secrets whose password is not remembered on this machine needs a new one.
        project.Secrets = "blob";
        var locked = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets);
        await locked.SaveCommand.ExecuteAsync(null);
        locked.ErrorMessage!.ShouldContain("not known on this machine");
    }

    [Fact]
    public async Task Settings_ValidateBeforeSavingAndRestoreTheProjectWhenSavingFails()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var originalUrl = project.UpdateUrl;
        var originalTransfer = project.Transfer;
        var viewModel = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets);

        viewModel.Name = " ";
        await viewModel.SaveCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Enter a project name.");
        viewModel.Name = "Demo";
        viewModel.UseHttpAuthentication = true;
        viewModel.HttpUsername = "";
        await viewModel.SaveCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("user name");
        viewModel.UseHttpAuthentication = false;
        viewModel.UpdateUrl = "ftp://not-http/";
        await viewModel.SaveCommand.ExecuteAsync(null);
        viewModel.ErrorMessage!.ShouldContain("HTTP(S)");
        await _context.Projects.DidNotReceive().SaveAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());

        viewModel.UpdateUrl = "https://new.example.com/u";
        viewModel.Transfer.Host = "new-host";
        viewModel.Transfer.Password = "changed";
        _context.Projects.SaveAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("disk full"));
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.ErrorMessage.ShouldBe("disk full");
        project.UpdateUrl.ShouldBe(originalUrl);
        project.Transfer.ShouldBeSameAs(originalTransfer);
        project.Secrets.ShouldBeNull();
        secrets.TransferPassword.ShouldBe(AppTestContext.NewSecrets().TransferPassword);
    }

    [Fact]
    public async Task ProjectSettings_KeepsTrailingSlashAndHttpPassword()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var viewModel = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets);
        viewModel.UpdateUrl = "https://u.example.com/x/";
        viewModel.UseHttpAuthentication = true;
        viewModel.HttpUsername = "web";
        viewModel.HttpPassword = "hp";
        await viewModel.SaveCommand.ExecuteAsync(null);
        project.UpdateUrl.ShouldBe("https://u.example.com/x/");
        secrets.HttpAuthenticationPassword.ShouldBe("hp");
        viewModel.HttpPassword = "";
        await viewModel.SaveCommand.ExecuteAsync(null);
        secrets.HttpAuthenticationPassword.ShouldBeNull();
    }

    [Fact]
    public async Task Settings_MigrateThePublishedPackages()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var viewModel = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets);

        // Closing the assistant without migrating changes nothing.
        _context.Dialogs.ShowDialogAsync(Arg.Any<MigrationViewModel>()).Returns(false);
        await viewModel.MigrateCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowDialogAsync(Arg.Is<MigrationViewModel>(m => m.Project == project && m.Secrets == secrets));
        viewModel.Migrated.ShouldBeFalse();

        _context.Dialogs.ShowDialogAsync(Arg.Do<MigrationViewModel>(m => m.Migrated = true)).Returns(true);
        await viewModel.MigrateCommand.ExecuteAsync(null);
        viewModel.Migrated.ShouldBeTrue();
    }

    [AvaloniaFact]
    public void ProjectSettingsWindow_Loads()
    {
        var settings = new ProjectSettingsWindow { DataContext = _context.Factory.Create<ProjectSettingsViewModel>(AppTestContext.NewProject(), AppTestContext.NewSecrets()) };
        settings.Show();
        settings.NameBox.Text.ShouldBe("Demo");
        settings.SaveCredentialsBox.IsChecked = true;
        settings.ProjectPasswordBox.Text = "secret-1";
        ((ProjectSettingsViewModel)settings.DataContext!).ProjectPassword.ShouldBe("secret-1");
        settings.Close();
    }

    [AvaloniaFact]
    public void ProjectSettingsWindow_ScrollsToTheSectionChosenInTheRail()
    {
        var window = new ProjectSettingsWindow
        {
            DataContext = _context.Factory.Create<ProjectSettingsViewModel>(AppTestContext.NewProject(),
                AppTestContext.NewSecrets()),
            Height = 500,
        };
        window.Show();
        window.UpdateLayout();
        window.Scroller.Offset.Y.ShouldBe(0);

        window.SectionList.SelectedIndex = 4;
        window.UpdateLayout();
        window.Scroller.Offset.Y.ShouldBeGreaterThan(0);
        window.SectionList.SelectedIndex = 0;
        window.UpdateLayout();
        window.Scroller.Offset.Y.ShouldBe(0);
        window.SectionList.SelectedIndex = -1; // nothing to scroll to
        window.Scroller.Offset.Y.ShouldBe(0);
        window.Close();
    }
}
