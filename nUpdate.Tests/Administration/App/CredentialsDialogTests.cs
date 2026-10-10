using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;

namespace nUpdate.Tests.Administration.App;

/// <summary>The credentials dialog: unlocking with the project password, or entering the secrets of a project that stores none.</summary>
public class CredentialsDialogTests
{
    private readonly AppTestContext _context = new();

    [Fact]
    public async Task Credentials_UnlockWithTheProjectPassword()
    {
        var project = AppTestContext.NewProject(statistics: true);
        var stored = AppTestContext.NewSecrets(statistics: true);
        project.Secrets = ProjectSecretsProtection.Protect(stored, "pw");
        var secrets = new ProjectSecrets();
        var viewModel =
            _context.Factory.Create<CredentialsViewModel>(project, secrets, CredentialsMode.ProjectPassword);
        viewModel.Title.ShouldBe("Unlock Demo");
        viewModel.Mode.ShouldBe(CredentialsMode.ProjectPassword);
        viewModel.AsksForProjectPassword.ShouldBeTrue();
        viewModel.AsksForSecrets.ShouldBeFalse();
        var closed = new List<bool>();
        viewModel.CloseRequested += (_, a) => closed.Add(a);

        await viewModel.AcceptCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("Enter the project password.");
        viewModel.ProjectPassword = "wrong";
        await viewModel.AcceptCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("The project password is wrong.");
        closed.ShouldBeEmpty();

        project.Secrets = "!!not base64";
        await viewModel.AcceptCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldBe("The project password is wrong.");
        project.Secrets = ProjectSecretsProtection.Protect(stored, "pw");

        viewModel.ProjectPassword = "pw";
        await viewModel.AcceptCommand.ExecuteAsync(null);
        closed.ShouldBe([true]);
        viewModel.EnteredPassword.ShouldBe("pw");
        secrets.TransferPassword.ShouldBe(stored.TransferPassword);
        secrets.PrivateKey.ShouldBe(stored.PrivateKey);
        secrets.StatisticsAdminSecret.ShouldBe("admin-secret");
    }

    [Fact]
    public async Task Credentials_CompletesSecrets()
    {
        var project = AppTestContext.NewProject(statistics: true);
        project.Transfer.Protocol = TransferProtocol.Sftp;
        project.Transfer.SftpPrivateKeyPath = "/key";
        project.Transfer.Proxy = new ProxySettings();
        project.HttpAuthentication = new HttpAuthenticationSettings();
        var secrets = new ProjectSecrets { TransferPassword = "old" };
        // Even a file with encrypted secrets asks for the secrets themselves when they turned out incomplete after unlocking.
        project.Secrets = ProjectSecretsProtection.Protect(secrets, "pw");
        var viewModel = _context.Factory.Create<CredentialsViewModel>(project, secrets, CredentialsMode.Secrets);
        viewModel.Title.ShouldBe("Credentials for Demo");
        viewModel.AsksForSecrets.ShouldBeTrue();
        viewModel.AsksForProjectPassword.ShouldBeFalse();
        viewModel.NeedsSftpPassphrase.ShouldBeTrue();
        viewModel.NeedsProxyPassword.ShouldBeTrue();
        viewModel.NeedsHttpPassword.ShouldBeTrue();
        viewModel.NeedsStatisticsSecret.ShouldBeTrue();
        viewModel.TransferPassword.ShouldBe("old");
        var closed = new List<bool>();
        viewModel.CloseRequested += (_, a) => closed.Add(a);

        viewModel.TransferPassword = "";
        await viewModel.AcceptCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldNotBeNull();
        closed.ShouldBeEmpty();

        viewModel.PrivateKey = " key ";
        viewModel.StatisticsAdminSecret = "s";
        viewModel.SftpKeyPassphrase = "pp";
        viewModel.ProxyPassword = "px";
        viewModel.HttpPassword = "hp";
        await viewModel.AcceptCommand.ExecuteAsync(null);
        closed.ShouldBe([true]);
        viewModel.EnteredPassword.ShouldBeNull();
        secrets.PrivateKey.ShouldBe("key");
        secrets.TransferPassword.ShouldBeNull();
        secrets.SftpKeyPassphrase.ShouldBe("pp");
        secrets.ProxyPassword.ShouldBe("px");
        secrets.HttpAuthenticationPassword.ShouldBe("hp");
        secrets.StatisticsAdminSecret.ShouldBe("s");

        var keyFile = Path.Combine(Path.GetTempPath(), "nupdate-key-" + Guid.NewGuid().ToString("N") + ".pem");
        await File.WriteAllTextAsync(keyFile, "-----BEGIN PRIVATE KEY-----");
        try
        {
            _context.Files.PickFileAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>()).Returns(keyFile);
            await viewModel.LoadPrivateKeyCommand.ExecuteAsync(null);
            viewModel.PrivateKey.ShouldBe("-----BEGIN PRIVATE KEY-----");
            _context.Files.PickFileAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>()).Returns((string?)null);
            await viewModel.LoadPrivateKeyCommand.ExecuteAsync(null);
            viewModel.PrivateKey.ShouldBe("-----BEGIN PRIVATE KEY-----");
        }
        finally
        {
            File.Delete(keyFile);
        }

        Should.Throw<ArgumentNullException>(() =>
            new CredentialsViewModel(_context.Files, null!, secrets, CredentialsMode.Secrets));
        Should.Throw<ArgumentNullException>(() =>
            new CredentialsViewModel(_context.Files, project, null!, CredentialsMode.Secrets));
        Should.Throw<ArgumentNullException>(() =>
            new CredentialsViewModel(null!, project, secrets, CredentialsMode.Secrets));
        // Without encrypted secrets there is no password to ask for.
        Should.Throw<ArgumentException>(() =>
            new CredentialsViewModel(_context.Files, AppTestContext.NewProject(), secrets,
                CredentialsMode.ProjectPassword));
    }

    [Fact]
    public async Task Credentials_KeepPasswordsAsTyped()
    {
        var project = AppTestContext.NewProject();
        var secrets = new ProjectSecrets();
        var viewModel = _context.Factory.Create<CredentialsViewModel>(project, secrets, CredentialsMode.Secrets);
        viewModel.TransferPassword = " spaced pw ";
        viewModel.PrivateKey = "  -----BEGIN PRIVATE KEY-----  ";
        await viewModel.AcceptCommand.ExecuteAsync(null);
        secrets.TransferPassword.ShouldBe(" spaced pw ");
        secrets.PrivateKey.ShouldBe("-----BEGIN PRIVATE KEY-----");
    }

    [Fact]
    public async Task Credentials_ReportUnreadableKeyFiles()
    {
        var viewModel = _context.Factory.Create<CredentialsViewModel>(AppTestContext.NewProject(), new ProjectSecrets(),
            CredentialsMode.Secrets);
        _context.Files.PickFileAsync(Arg.Any<string>(), Arg.Any<nUpdate.Administration.Services.FileTypeFilter[]>())
            .Returns(Path.Combine(Path.GetTempPath(), "nupdate-missing-" + Guid.NewGuid().ToString("N") + ".pem"));
        await viewModel.LoadPrivateKeyCommand.ExecuteAsync(null);
        viewModel.ErrorMessage.ShouldNotBeNullOrEmpty();
        viewModel.PrivateKey.ShouldBe("");
    }

    [Fact]
    public void ProjectPassword_ValidatesAndReturnsTheChoice()
    {
        var viewModel = _context.Factory.Create<ProjectPasswordViewModel>();
        viewModel.Title.ShouldBe("Protect the project");
        var closed = new List<bool>();
        viewModel.CloseRequested += (_, a) => closed.Add(a);

        viewModel.AcceptCommand.Execute(null);
        viewModel.ErrorMessage!.ShouldContain("at least 8 characters");
        viewModel.Password = "longenough";
        viewModel.PasswordConfirmation = "different";
        viewModel.AcceptCommand.Execute(null);
        viewModel.ErrorMessage.ShouldBe("The passwords do not match.");
        closed.ShouldBeEmpty();

        viewModel.PasswordConfirmation = "longenough";
        viewModel.AcceptCommand.Execute(null);
        viewModel.Result.ShouldBe("longenough");
        closed.ShouldBe([true]);

        var unsaved = _context.Factory.Create<ProjectPasswordViewModel>();
        unsaved.SaveCredentials = false;
        unsaved.AcceptCommand.Execute(null);
        unsaved.Result.ShouldBeNull();
        unsaved.ErrorMessage.ShouldBeNull();
    }

    [AvaloniaFact]
    public void CredentialsWindow_BindsBothModes()
    {
        var credentials = new CredentialsWindow
        {
            DataContext = _context.Factory.Create<CredentialsViewModel>(AppTestContext.NewProject(),
                new ProjectSecrets(), CredentialsMode.Secrets)
        };
        credentials.Show();
        credentials.TransferPasswordBox.Text = "pw";
        ((CredentialsViewModel)credentials.DataContext!).TransferPassword.ShouldBe("pw");
        credentials.ProjectPasswordBox.IsEffectivelyVisible.ShouldBeFalse();
        credentials.Close();

        var locked = AppTestContext.NewProject();
        locked.Secrets = ProjectSecretsProtection.Protect(new ProjectSecrets(), "pw");
        var unlock = new CredentialsWindow
        {
            DataContext =
                _context.Factory.Create<CredentialsViewModel>(locked, new ProjectSecrets(),
                    CredentialsMode.ProjectPassword)
        };
        unlock.Show();
        unlock.ProjectPasswordBox.Text = "pw";
        ((CredentialsViewModel)unlock.DataContext!).ProjectPassword.ShouldBe("pw");
        unlock.TransferPasswordBox.IsEffectivelyVisible.ShouldBeFalse();
        unlock.Close();

        var password = new ProjectPasswordWindow { DataContext = _context.Factory.Create<ProjectPasswordViewModel>() };
        password.Show();
        password.PasswordBox.Text = "secret-1";
        ((ProjectPasswordViewModel)password.DataContext!).Password.ShouldBe("secret-1");
        password.Close();
    }
}
