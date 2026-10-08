using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Exceptions;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.App;

/// <summary>The main window: the project list, opening, unlocking, converting, creating and forgetting projects.</summary>
public class MainWindowTests
{
    private readonly AppTestContext _context = new();

    private MainWindowViewModel Main => _context.Factory.Create<MainWindowViewModel>();

    private static ProjectRegistration Registration(string name, string path) => new(Guid.NewGuid(), name, path);

    [Fact]
    public async Task Main_ListsProjectsAndForgets()
    {
        var a = Registration("A", "/a");
        _context.Store.ListAsync(Arg.Any<CancellationToken>()).Returns([Registration("B", "/b"), a]);
        var main = Main;
        await main.InitializeAsync();
        main.Projects.Count.ShouldBe(2);
        main.Status.ShouldBe("2 projects");
        main.Version.ShouldStartWith("nUpdate Administration");
        main.OpenSelectedCommand.CanExecute(null).ShouldBeFalse();
        main.ForgetSelectedCommand.CanExecute(null).ShouldBeFalse();

        main.SelectedProject = main.Projects.Single(p => p.Name == "A");
        main.ForgetSelectedCommand.CanExecute(null).ShouldBeTrue();
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);
        await main.ForgetSelectedCommand.ExecuteAsync(null);
        await _context.Store.DidNotReceive().UnregisterAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        _context.Store.ListAsync(Arg.Any<CancellationToken>()).Returns([Registration("B", "/b")]);
        await main.ForgetSelectedCommand.ExecuteAsync(null);
        await _context.Store.Received().UnregisterAsync(a.Id, Arg.Any<CancellationToken>());
        await _context.Passwords.Received().RemoveAsync(a.Id, Arg.Any<CancellationToken>());
        main.Status.ShouldBe("1 project");

        _context.Store.ListAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("broken list"));
        await main.RefreshCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while reading the project list", "broken list");
        _context.Store.ListAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new UnsupportedFormatException("newer list"));
        await main.RefreshCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while reading the project list", "newer list");
    }

    [Fact]
    public async Task Main_OpensProjectsAndAsksForMissingSecrets()
    {
        var project = AppTestContext.NewProject();
        var complete =
            new ProjectLoadResult(project, AppTestContext.NewSecrets(), migrated: false, SecretsState.NotSaved);
        _context.Store.LoadAsync("/p/project.nupdproj", Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(complete);
        var main = Main;

        await main.OpenProjectAsync("/p/project.nupdproj");

        await _context.Store.Received()
            .RegisterAsync(Arg.Is<ProjectRegistration>(c => c.Name == "Demo" && c.Id == project.Id),
                Arg.Any<CancellationToken>());
        await _context.Dialogs.Received().ShowWindowAsync(Arg.Any<ProjectViewModel>());

        var incomplete = new ProjectLoadResult(project, new ProjectSecrets(), migrated: false, SecretsState.NotSaved);
        _context.Store.LoadAsync("/p/project.nupdproj", Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(incomplete);
        _context.Dialogs.ClearReceivedCalls();
        _context.Dialogs.ShowDialogAsync(Arg.Any<CredentialsViewModel>()).Returns(false);
        await main.OpenProjectAsync("/p/project.nupdproj");
        await _context.Dialogs.Received().ShowDialogAsync(Arg.Is<CredentialsViewModel>(c => c.AsksForSecrets));
        await _context.Dialogs.DidNotReceive().ShowWindowAsync(Arg.Any<ProjectViewModel>());

        _context.Dialogs.ShowDialogAsync(Arg.Any<CredentialsViewModel>()).Returns(true);
        await main.OpenProjectAsync("/p/project.nupdproj");
        await _context.Dialogs.Received().ShowWindowAsync(Arg.Any<ProjectViewModel>());

        main.SelectedProject = new ProjectCardViewModel(Registration("Demo", "/p/project.nupdproj"));
        await main.OpenSelectedCommand.ExecuteAsync(null);
        await _context.Store.Received(4)
            .LoadAsync("/p/project.nupdproj", Arg.Any<string?>(), Arg.Any<CancellationToken>());

        await Should.ThrowAsync<ArgumentException>(() => main.OpenProjectAsync(" "));
    }

    [Fact]
    public async Task Main_UnlocksEncryptedSecretsWithTheRememberedOrEnteredPassword()
    {
        var project = AppTestContext.NewProject();
        project.Secrets = ProjectSecretsProtection.Protect(AppTestContext.NewSecrets(), "pw");
        var locked =
            new ProjectLoadResult(project, new ProjectSecrets(), migrated: false, SecretsState.PasswordRequired);
        var unlocked =
            new ProjectLoadResult(project, AppTestContext.NewSecrets(), migrated: false, SecretsState.Loaded);
        _context.Store.LoadAsync("/p/project.nupdproj", null, Arg.Any<CancellationToken>()).Returns(locked);
        _context.Store.LoadAsync("/p/project.nupdproj", "pw", Arg.Any<CancellationToken>()).Returns(unlocked);
        _context.Store.LoadAsync("/p/project.nupdproj", "stale", Arg.Any<CancellationToken>())
            .Returns(new ProjectLoadResult(project, new ProjectSecrets(), false, SecretsState.Unreadable));
        var main = Main;

        // The remembered password unlocks the project without a dialog.
        _context.Passwords.GetAsync(project.Id, Arg.Any<CancellationToken>()).Returns("pw");
        await main.OpenProjectAsync("/p/project.nupdproj");
        await _context.Dialogs.DidNotReceive().ShowDialogAsync(Arg.Any<CredentialsViewModel>());
        await _context.Dialogs.Received(1).ShowWindowAsync(Arg.Any<ProjectViewModel>());

        // A stale remembered password falls back to the dialog, which decrypts the secrets itself.
        _context.Passwords.GetAsync(project.Id, Arg.Any<CancellationToken>()).Returns("stale");
        _context.Dialogs.ShowDialogAsync(Arg.Any<CredentialsViewModel>()).Returns(async call =>
        {
            var c = call.Arg<CredentialsViewModel>();
            c.ProjectPassword = "pw";
            await c.AcceptCommand.ExecuteAsync(null);
            return true;
        });
        await main.OpenProjectAsync("/p/project.nupdproj");
        await _context.Dialogs.Received(1).ShowDialogAsync(Arg.Is<CredentialsViewModel>(c => c.AsksForProjectPassword));
        await _context.Passwords.Received().SetAsync(project.Id, "pw", Arg.Any<CancellationToken>());
        await _context.Dialogs.Received(2).ShowWindowAsync(Arg.Any<ProjectViewModel>());

        // Without a remembered password the dialog is shown; cancelling opens nothing.
        _context.Passwords.GetAsync(project.Id, Arg.Any<CancellationToken>()).Returns((string?)null);
        _context.Dialogs.ShowDialogAsync(Arg.Any<CredentialsViewModel>()).Returns(false);
        await main.OpenProjectAsync("/p/project.nupdproj");
        await _context.Dialogs.Received(2).ShowWindowAsync(Arg.Any<ProjectViewModel>());

        // Declining to remember keeps the password out of the store.
        _context.Passwords.ClearReceivedCalls();
        _context.Dialogs.ShowDialogAsync(Arg.Any<CredentialsViewModel>()).Returns(async call =>
        {
            var c = call.Arg<CredentialsViewModel>();
            c.ProjectPassword = "pw";
            c.RememberPassword = false;
            await c.AcceptCommand.ExecuteAsync(null);
            return true;
        });
        await main.OpenProjectAsync("/p/project.nupdproj");
        await _context.Passwords.DidNotReceive()
            .SetAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _context.Dialogs.Received(3).ShowWindowAsync(Arg.Any<ProjectViewModel>());
    }

    [Fact]
    public async Task Main_ConvertsLegacyProjectsAndAsksForAProjectPassword()
    {
        var project = AppTestContext.NewProject();
        project.Path = "/old/Legacy.nupdproj";
        var secrets = AppTestContext.NewSecrets();
        _context.Store.LoadAsync(project.Path, Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new ProjectLoadResult(project, secrets, migrated: true, SecretsState.Loaded));
        _context.Dialogs.ShowDialogAsync(Arg.Do<ProjectPasswordViewModel>(p =>
        {
            p.Password = "project-pw";
            p.PasswordConfirmation = "project-pw";
            p.AcceptCommand.Execute(null);
        })).Returns(true);
        var main = Main;

        await main.OpenProjectAsync(project.Path);

        await _context.Dialogs.Received().ShowInfoAsync("Project converted", Arg.Any<string>());
        await _context.Projects.Received()
            .SaveMigratedAsync(project, secrets, "project-pw", Arg.Any<CancellationToken>());
        await _context.Store.DidNotReceive()
            .RegisterAsync(Arg.Any<ProjectRegistration>(), Arg.Any<CancellationToken>());
        await _context.Dialogs.Received().ShowWindowAsync(Arg.Any<ProjectViewModel>());

        // Cancelling the password dialog leaves the project unconverted and closed.
        _context.Dialogs.ClearReceivedCalls();
        _context.Projects.ClearReceivedCalls();
        _context.Dialogs.ShowDialogAsync(Arg.Any<ProjectPasswordViewModel>()).Returns(false);
        await main.OpenProjectAsync(project.Path);
        await _context.Projects.DidNotReceiveWithAnyArgs().SaveMigratedAsync(default!, default!, default, default);
        await _context.Dialogs.DidNotReceive().ShowWindowAsync(Arg.Any<ProjectViewModel>());

        // A legacy project whose secrets could not be decrypted explains why before asking for them.
        _context.Store.LoadAsync(project.Path, Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(
            new ProjectLoadResult(project, new ProjectSecrets(), migrated: true, SecretsState.Unreadable));
        _context.Dialogs.ShowDialogAsync(Arg.Any<CredentialsViewModel>()).Returns(true);
        _context.Dialogs.ShowDialogAsync(Arg.Any<ProjectPasswordViewModel>()).Returns(true);
        await main.OpenProjectAsync(project.Path);
        await _context.Dialogs.Received().ShowInfoAsync("Cannot read the project's credentials", Arg.Any<string>());
        await _context.Dialogs.Received().ShowDialogAsync(Arg.Is<CredentialsViewModel>(c => c.AsksForSecrets));
        await _context.Projects.Received().SaveMigratedAsync(project, Arg.Any<ProjectSecrets>(), Arg.Any<string?>(),
            Arg.Any<CancellationToken>());

        // A save that fails is reported.
        _context.Dialogs.ClearReceivedCalls();
        _context.Projects
            .SaveMigratedAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(), Arg.Any<string?>(),
                Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("read only"));
        await main.OpenProjectAsync(project.Path);
        await _context.Dialogs.Received().ShowErrorAsync("Error while saving the converted project", "read only");
        await _context.Dialogs.DidNotReceive().ShowWindowAsync(Arg.Any<ProjectViewModel>());
    }

    [Fact]
    public async Task MainWindow_ReportsEveryLoadErrorType()
    {
        var main = _context.Factory.Create<MainWindowViewModel>();
        foreach (var exception in new Exception[]
                 {
                     new IOException("io"), new UnauthorizedAccessException("denied"), new InvalidDataException("data"),
                     new UnsupportedFormatException("too new"), new FormatException("bad base64"),
                     new Newtonsoft.Json.JsonReaderException("json")
                 })
        {
            _context.Store.LoadAsync("/p.nupdproj", Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(exception);
            await main.OpenProjectAsync("/p.nupdproj");
            await _context.Dialogs.Received().ShowErrorAsync("Error while opening the project", exception.Message);
        }

        _context.Store.LoadAsync("/p.nupdproj", Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("unexpected"));
        await Should.ThrowAsync<InvalidOperationException>(() => main.OpenProjectAsync("/p.nupdproj"));
    }

    [Fact]
    public async Task MainWindow_HandlesInvalidDataOnRefresh()
    {
        var main = _context.Factory.Create<MainWindowViewModel>();
        _context.Store.ListAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidDataException("bad json"));
        await main.RefreshCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while reading the project list", "bad json");
        _context.Store.ListAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Newtonsoft.Json.JsonReaderException("json"));
        await main.RefreshCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while reading the project list", "json");
    }

    [Fact]
    public async Task Main_RefreshesTheListWhenAProjectIsRenamed()
    {
        var main = _context.Factory.Create<MainWindowViewModel>();
        var project = AppTestContext.NewProject();
        project.Path = "/p/project.nupdproj";
        _context.Store.LoadAsync(project.Path, Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(AppTestContext.Loaded(project, AppTestContext.NewSecrets()));
        ProjectViewModel? opened = null;
        _context.Dialogs.ShowWindowAsync(Arg.Do<ProjectViewModel>(v => opened = v))
            .Returns(new TaskCompletionSource().Task);

        await main.OpenProjectAsync(project.Path);
        _context.Store.ClearReceivedCalls();

        opened!.Refresh();
        await _context.Store.DidNotReceive().ListAsync(Arg.Any<CancellationToken>());
        project.Name = "Renamed";
        opened.Refresh();
        await Task.Yield();
        await _context.Store.Received(1).ListAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Main_ReportsAListThatCannotBeUpdated()
    {
        var main = _context.Factory.Create<MainWindowViewModel>();
        var demo = Registration("Demo", "/p/project.nupdproj");
        main.SelectedProject = new ProjectCardViewModel(demo);
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        _context.Store.UnregisterAsync(demo.Id, Arg.Any<CancellationToken>()).ThrowsAsync(new IOException("locked"));
        await main.ForgetSelectedCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while updating the project list", "locked");
    }

    [Fact]
    public async Task Main_NewAndOpenFileCommands()
    {
        var main = Main;
        var project = AppTestContext.NewProject();
        _context.Dialogs.ShowDialogAsync(Arg.Any<NewProjectViewModel>()).Returns(false);
        await main.NewProjectCommand.ExecuteAsync(null);
        await _context.Dialogs.DidNotReceive().ShowWindowAsync(Arg.Any<ViewModelBase>());

        _context.Dialogs.ShowDialogAsync(Arg.Any<NewProjectViewModel>()).Returns(call =>
        {
            typeof(NewProjectViewModel).GetProperty(nameof(NewProjectViewModel.Result))!.SetValue(
                call.Arg<NewProjectViewModel>(), AppTestContext.Loaded(project, new ProjectSecrets()));
            return Task.FromResult(true);
        });
        await main.NewProjectCommand.ExecuteAsync(null);
        await _context.Dialogs.Received(1).ShowWindowAsync(Arg.Any<ProjectViewModel>());

        _context.Files.PickFileAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>()).Returns((string?)null);
        await main.OpenProjectFileCommand.ExecuteAsync(null);
        _context.Files.PickFileAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>())
            .Returns("/picked/project.nupdproj");
        _context.Store.LoadAsync("/picked/project.nupdproj", Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(AppTestContext.Loaded(project, AppTestContext.NewSecrets()));
        await main.OpenProjectFileCommand.ExecuteAsync(null);
        await _context.Dialogs.Received(2).ShowWindowAsync(Arg.Any<ProjectViewModel>());
    }

    [Fact]
    public async Task Main_RefreshesTheListWhenAProjectWindowCloses()
    {
        var main = _context.Factory.Create<MainWindowViewModel>();
        var project = AppTestContext.NewProject();
        project.Path = "/p/project.nupdproj";
        _context.Store.LoadAsync(project.Path, Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(AppTestContext.Loaded(project, AppTestContext.NewSecrets()));
        var closed = new TaskCompletionSource();
        _context.Dialogs.ShowWindowAsync(Arg.Any<ProjectViewModel>()).Returns(closed.Task);

        await main.OpenProjectAsync(project.Path);
        _context.Store.ClearReceivedCalls();

        closed.SetResult();
        await Task.Yield();
        await _context.Store.Received(1).ListAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Main_DoesNotOpenAProjectTwice()
    {
        var main = _context.Factory.Create<MainWindowViewModel>();
        var project = AppTestContext.NewProject();
        project.Path = Path.Combine(Path.GetTempPath(), "p", "project.nupdproj");
        _context.Store.LoadAsync(project.Path, Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(AppTestContext.Loaded(project, AppTestContext.NewSecrets()));
        var closed = new TaskCompletionSource();
        _context.Dialogs.ShowWindowAsync(Arg.Any<ProjectViewModel>()).Returns(closed.Task);

        await main.OpenProjectAsync(project.Path);
        await main.OpenProjectAsync(Path.Combine(Path.GetTempPath(), "p", "..", "p", "project.nupdproj"));

        await _context.Dialogs.Received(1).ShowWindowAsync(Arg.Any<ProjectViewModel>());
        await _context.Dialogs.Received().ShowInfoAsync("Project already open", Arg.Any<string>());

        closed.SetResult();
        await Task.Yield();
        await main.OpenProjectAsync(project.Path);
        await _context.Dialogs.Received(2).ShowWindowAsync(Arg.Any<ProjectViewModel>());

        // A path that cannot be normalised is compared as it is.
        _context.Store.LoadAsync("\0", Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("no such file"));
        await main.OpenProjectAsync("\0");
        await _context.Dialogs.Received().ShowErrorAsync("Error while opening the project", "no such file");
    }

    [Fact]
    public async Task Main_ForgetsAnUnopenedLegacyFileByItsPath()
    {
        var legacy = new ProjectRegistration(Guid.Empty, "Legacy", "/old/Legacy.nupdproj");
        _context.Store.ListAsync(Arg.Any<CancellationToken>()).Returns([legacy]);
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        var main = Main;
        await main.InitializeAsync();
        main.SelectedProject = main.Projects.Single();

        await main.ForgetSelectedCommand.ExecuteAsync(null);

        await _context.Store.Received().UnregisterPathAsync("/old/Legacy.nupdproj", Arg.Any<CancellationToken>());
        await _context.Store.DidNotReceive().UnregisterAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _context.Passwords.DidNotReceive().RemoveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public void MainWindow_BindsProjectsAndStatus()
    {
        _context.Store.ListAsync(Arg.Any<CancellationToken>()).Returns([Registration("A", "/a")]);
        var viewModel = _context.Factory.Create<MainWindowViewModel>();
        var window = new MainWindow { DataContext = viewModel };
        window.Show();
        viewModel.RefreshCommand.ExecuteAsync(null).GetAwaiter().GetResult();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        window.ProjectList.ItemCount.ShouldBe(1);
        window.StatusText.Text.ShouldBe("1 project");
        window.NewProjectButton.Command.ShouldBe(viewModel.NewProjectCommand);
        window.Close();
    }

    [AvaloniaFact]
    public void MainWindow_OffersItsActionsInTheMacOsMenuBar()
    {
        var viewModel = _context.Factory.Create<MainWindowViewModel>();
        var window = new MainWindow { DataContext = viewModel };
        window.Show();
        var file = (NativeMenuItem)NativeMenu.GetMenu(window)!.Items.Single();
        file.Header.ShouldBe("File");
        file.Menu!.Items.OfType<NativeMenuItem>().Where(i => i is not NativeMenuItemSeparator).Select(i => (i.Header, i.Command, i.Gesture?.ToString())).ShouldBe([
            ("New Project…", viewModel.NewProjectCommand, "Cmd+N"),
            ("Open Project File…", viewModel.OpenProjectFileCommand, "Cmd+O"),
            ("Refresh", viewModel.RefreshCommand, "Cmd+R"),
        ]);
        window.Close();
    }

    [Fact]
    public async Task Main_DescribesTheProjectsOnTheirCardsAndFiltersThem()
    {
        UpdateProject Project(string name, TransferProtocol protocol, params (string Version, bool Released)[] packages)
        {
            var project = AppTestContext.NewProject();
            project.Name = name;
            project.UpdateUrl = $"https://updates.example.com/{name.ToLowerInvariant()}/";
            project.Transfer.Protocol = protocol;
            foreach (var (version, released) in packages)
                project.Packages.Add(new UpdatePackage { Version = new UpdateVersion(version), Released = released });
            return project;
        }

        var cases = new (string Name, UpdateProject? Project)[]
        {
            ("Aurora", Project("Aurora", TransferProtocol.Sftp, ("2.0.0", true), ("2.1.0", true), ("2.2.0-beta.1", false))),
            ("Beacon", Project("Beacon", TransferProtocol.Ftp, ("1.0.0", false))),
            ("Comet", Project("Comet", TransferProtocol.FtpsExplicit)),
            ("Delta", Project("Delta", TransferProtocol.FtpsImplicit)),
            ("Echo", Project("Echo", TransferProtocol.Plugin)),
            ("Foxtrot", null),
        };
        _context.Store.ListAsync(Arg.Any<CancellationToken>())
            .Returns(cases.Select(c => Registration(c.Name, $"/projects/{c.Name}/project.nupdproj")).ToList());
        foreach (var (name, project) in cases.Where(c => c.Project is not null))
            _context.Store.LoadAsync(Arg.Is<string>(p => p.Contains(name)), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(AppTestContext.Loaded(project!, AppTestContext.NewSecrets()));
        _context.Store.LoadAsync(Arg.Is<string>(p => p.Contains("Foxtrot")), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("The file is locked."));

        var main = Main;
        await main.RefreshCommand.ExecuteAsync(null);

        main.Projects.Select(c => (c.Initials, c.Latest, c.HasRelease, c.ShowsUnreleased, c.Packages, c.Protocol))
            .ShouldBe([
                ("A", "2.1.0 released", true, false, "3 packages", "SFTP"),
                ("B", "Not published yet", false, true, "1 package", "FTP"),
                ("C", "No packages yet", false, true, "0 packages", "FTPS"),
                ("D", "No packages yet", false, true, "0 packages", "FTPS (implicit)"),
                ("E", "No packages yet", false, true, "0 packages", "Plugin"),
                ("F", "", false, false, "", ""),
            ]);
        main.Projects[0].UpdateUrl.ShouldBe("https://updates.example.com/aurora/");
        main.Projects[5].Problem.ShouldBe("The file is locked.");
        main.Projects.Take(5).ShouldAllBe(c => c.Problem == null);
        ProjectCardViewModel.InitialsOf("Media Tool Pro").ShouldBe("MT");

        // Every word must match the name, the path or the update URL.
        main.SearchText = "updates.example.com/comet";
        main.VisibleProjects.Select(c => c.Name).ShouldBe(["Comet"]);
        main.SearchText = "projects  aurora";
        main.VisibleProjects.Select(c => c.Name).ShouldBe(["Aurora"]);
        main.SearchText = "nothing";
        main.VisibleProjects.ShouldBeEmpty();
        main.SearchText = "";
        main.VisibleProjects.Count.ShouldBe(6);
    }
}
