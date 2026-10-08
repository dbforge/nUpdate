using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The project settings dialog: general data, transfer, statistics and deleting the project.</summary>
public sealed class ProjectSettingsScenarios : ScenarioTest
{
    public ProjectSettingsScenarios(ServerFixture server)
        : base(server)
    {
    }

    private ProjectLoadResult _project = null!;
    private ProjectWindow _projectWindow = null!;

    private async Task<ProjectSettingsWindow> OpenSettingsAsync(string name = "Trade Updater", TransferProtocol protocol = TransferProtocol.Sftp, string? releasedPackage = null)
    {
        _project = await App.ExistingProjectAsync(name, protocol);
        if (releasedPackage is not null)
            await App.ExistingPackageAsync(_project, releasedPackage, "First", publish: true);
        _projectWindow = await App.OpenListedProjectAsync(name);
        User.Click(_projectWindow.SettingsButton);
        return await App.WindowAsync<ProjectSettingsWindow>();
    }

    private static ProjectSettingsViewModel ViewModel(ProjectSettingsWindow settings) => (ProjectSettingsViewModel)settings.DataContext!;

    [AvaloniaFact]
    public Task Shows_the_current_settings_and_validates_changes() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("the settings of an SFTP project", async () => settings = await OpenSettingsAsync());
        await Then("the current values are shown", () =>
        {
            settings.NameBox.Text!.ShouldBe("Trade Updater");
            settings.FolderBox.Text!.ShouldBe(App.ProjectFolder("Trade Updater"));
            settings.UpdateUrlBox.Text!.ShouldBe(Context.Server.HttpBaseUrl);
            User.Find<ComboBox>(settings, "ProtocolBox").SelectedItem.ShouldBe(TransferProtocol.Sftp);
            User.Find<TextBox>(settings, "HostBox").Text!.ShouldBe(Context.Server.SftpHost);
            User.Find<TextBox>(settings, "PasswordBox").Text!.ShouldBe(ServerFixture.SftpPassword);
            settings.SaveCredentialsBox.IsChecked.ShouldBe(true);
            User.Find<ToggleSwitch>(settings, "EnabledBox").IsChecked.ShouldBe(false);
        });
        await When("the user clears the name and saves", () =>
        {
            User.Type(settings.NameBox, "");
            User.Click(settings.SaveButton);
        });
        await Then("the dialog asks for a name", () => settings.ErrorText.Text!.ShouldBe("Enter a project name."));
        await When("the user enters an invalid URL", () =>
        {
            User.Type(settings.NameBox, "Trade Updater");
            User.Type(settings.UpdateUrlBox, "ftp://nope");
            User.Click(settings.SaveButton);
        });
        await Then("the dialog asks for an HTTP(S) URL", () => settings.ErrorText.Text!.ShouldContain("HTTP(S)"));
        await When("the user removes the password", () =>
        {
            User.Type(settings.UpdateUrlBox, Context.Server.HttpBaseUrl);
            User.Type(User.Find<TextBox>(settings, "PasswordBox"), "");
            User.Click(settings.SaveButton);
        });
        await Then("the dialog asks for a password or a key file and stays open", () =>
        {
            settings.ErrorText.Text!.ShouldBe("Enter a password or choose a private key file.");
            settings.IsVisible.ShouldBeTrue();
        });
    });

    [AvaloniaFact]
    public Task Renames_the_project() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("the settings of a project with a package", async () =>
        {
            _project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(_project, "1.0.0", "First", publish: false);
            _projectWindow = await App.OpenListedProjectAsync("Trade Updater");
            User.Click(_projectWindow.SettingsButton);
            settings = await App.WindowAsync<ProjectSettingsWindow>();
        });
        await When("the user renames it and saves", async () =>
        {
            User.Type(settings.NameBox, "Trade Updater Pro");
            User.Click(settings.SaveButton);
            await App.ClosedAsync(settings);
        });
        await Then("the project window and the list carry the new name while the folder stays", async () =>
        {
            await User.WaitUntil(() => _projectWindow.Title == "Trade Updater Pro - nUpdate Administration", "the project window title");
            await User.WaitUntil(() => App.ListedProjects.Contains("Trade Updater Pro"), "the project list to follow the rename");
            App.ListedProjects.ShouldBe(["Trade Updater Pro"]);
            Directory.Exists(App.ProjectFolder("Trade Updater")).ShouldBeTrue();
            File.Exists(App.ProjectFile("Trade Updater")).ShouldBeTrue();
            File.Exists(_project.Project.PackageFilePath(new nUpdate.Updating.UpdateVersion("1.0.0"), "any")).ShouldBeTrue();
            (await App.SavedAsync("Trade Updater")).Project.Name.ShouldBe("Trade Updater Pro");
        });
    });

    [AvaloniaFact]
    public Task Refuses_a_name_that_belongs_to_another_project() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("two projects and the settings of the first", async () =>
        {
            await App.ExistingProjectAsync("Other");
            settings = await OpenSettingsAsync();
        });
        await When("the user renames it to the other project's name", async () =>
        {
            User.Type(settings.NameBox, "Other");
            User.Click(settings.SaveButton);
            await User.WaitUntil(() => settings.ErrorText.IsEffectivelyVisible || !settings.IsVisible, "the save to finish");
        });
        await Then("the dialog refuses and nothing changed", () =>
        {
            settings.ErrorText.Text!.ShouldBe("A project named \"Other\" already exists.");
            settings.IsVisible.ShouldBeTrue();
            _projectWindow.Title.ShouldBe("Trade Updater - nUpdate Administration");
        });
    });

    [AvaloniaFact]
    public Task Changes_the_transfer_settings_and_tests_them() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("the settings of an SFTP project", async () => settings = await OpenSettingsAsync());
        await When("the user switches to the FTP server and tests the connection", async () =>
        {
            User.Select(User.Find<ComboBox>(settings, "ProtocolBox"), TransferProtocol.Ftp);
            User.Type(User.Find<TextBox>(settings, "HostBox"), Context.Server.FtpHost);
            User.Find<NumericUpDown>(settings, "PortBox").Value = Context.Server.FtpPort;
            User.Type(User.Find<TextBox>(settings, "UsernameBox"), ServerFixture.FtpUser);
            User.Type(User.Find<TextBox>(settings, "PasswordBox"), ServerFixture.FtpPassword);
            User.Type(User.Find<TextBox>(settings, "DirectoryBox"), "/");
            User.Find<ToggleSwitch>(settings, "PassiveBox").IsEffectivelyVisible.ShouldBeTrue();
            User.Click(User.Find<Button>(settings, "TestButton"));
            var result = User.Find<TextBlock>(settings, "TestResultText");
            await User.WaitUntil(() => result.Text == "Connection successful.", "the connection test");
        });
        await And("saves", async () =>
        {
            User.Click(settings.SaveButton);
            await App.ClosedAsync(settings);
        });
        await Then("the project uses FTP from now on", async () =>
        {
            _projectWindow.TransferBox.Text!.ShouldBe($"Ftp {ServerFixture.FtpUser}@{Context.Server.FtpHost}:{Context.Server.FtpPort}/");
            var saved = await App.SavedAsync("Trade Updater");
            saved.Project.Transfer.Protocol.ShouldBe(TransferProtocol.Ftp);
            saved.Secrets.TransferPassword.ShouldBe(ServerFixture.FtpPassword);
        });
    });

    [AvaloniaFact]
    public Task Enables_statistics_for_an_existing_project() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("the settings of an FTP project without statistics", async () => settings = await OpenSettingsAsync(protocol: TransferProtocol.Ftp));
        await When("the user enables statistics with the database and saves", async () =>
        {
            User.Check(User.Find<ToggleSwitch>(settings, "EnabledBox"), true);
            User.Type(User.Find<TextBox>(settings, "DbHostBox"), "mysql");
            User.Type(User.Find<TextBox>(settings, "DbNameBox"), ServerFixture.DbName);
            User.Type(User.Find<TextBox>(settings, "DbUserBox"), ServerFixture.DbUser);
            User.Type(User.Find<TextBox>(settings, "DbPasswordBox"), ServerFixture.DbPassword);
            User.Click(settings.SaveButton);
            await App.ClosedAsync(settings);
        });
        await Then("the statistics script is on the server and the Statistics tab works", async () =>
        {
            using var response = await Context.HttpClient.GetAsync(Context.Server.HttpBaseUrl + "nupdate-statistics.php/v2/projects/" + _project.Project.Id + "/statistics");
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
            var saved = await App.SavedAsync("Trade Updater");
            saved.Project.Statistics.Enabled.ShouldBeTrue();
            saved.Secrets.StatisticsAdminSecret.ShouldNotBeNullOrEmpty();
            User.SelectPage(_projectWindow.Nav, "Statistics");
            _projectWindow.RefreshStatisticsButton.IsEffectivelyEnabled.ShouldBeTrue();
            User.Click(_projectWindow.RefreshStatisticsButton);
            await User.WaitUntil(() => _projectWindow.StatisticsStatusText.Text != "Loading..." && _projectWindow.StatisticsUpdatedText.Text?.StartsWith("Last updated", StringComparison.Ordinal) == true, "the statistics to load");
            ((ProjectViewModel)_projectWindow.DataContext!).TotalDownloads.ShouldBe(0);
        });
    });

    [AvaloniaFact]
    public Task Changes_the_project_password() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("the settings of a project", async () => settings = await OpenSettingsAsync());
        await When("the user enters a new project password and saves", async () =>
        {
            User.Type(settings.ProjectPasswordBox, "another-password");
            User.Type(settings.ProjectPasswordConfirmationBox, "another-password");
            User.Click(settings.SaveButton);
            await App.ClosedAsync(settings);
        });
        await Then("the file opens with the new password only", async () =>
        {
            (await App.Store.LoadAsync(App.ProjectFile("Trade Updater"), AdministrationApp.ProjectPassword)).SecretsState.ShouldBe(SecretsState.Unreadable);
            var saved = await App.Store.LoadAsync(App.ProjectFile("Trade Updater"), "another-password");
            saved.SecretsState.ShouldBe(SecretsState.Loaded);
            saved.Secrets.TransferPassword.ShouldBe(ServerFixture.SftpPassword);
            (await App.Passwords.GetAsync(_project.Project.Id)).ShouldBe("another-password");
        });
    });

    [AvaloniaFact]
    public Task Deletes_the_project_including_the_server_files() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("the settings of a project with a released package", async () => settings = await OpenSettingsAsync(releasedPackage: "1.0.0"));
        await When("the user deletes the project and the files on the server", async () =>
        {
            User.Click(settings.DeleteButton);
            var first = await App.PopupAsync();
            first.Title.ShouldBe("Delete project");
            await first.ClickAsync("Delete");
            var second = await App.PopupAsync();
            second.Title.ShouldBe("Delete server files");
            second.Buttons.ShouldBe(["Keep", "Delete on server"]);
            await second.ClickAsync("Delete on server");
        });
        await Then("the settings and the project window close and the project is gone", async () =>
        {
            await App.ClosedAsync(settings);
            await App.ClosedAsync(_projectWindow);
            App.ListedProjects.ShouldBeEmpty();
            Directory.Exists(App.ProjectFolder("Trade Updater")).ShouldBeFalse();
        });
        await And("the server no longer serves the project", async () =>
        {
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.NotFound);
            (await Context.StatusAsync("packages/1.0.0.zip")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        });
    });

    [AvaloniaFact]
    public Task Deletes_the_project_but_keeps_the_server_files() => Scenario(async () =>
    {
        ProjectSettingsWindow settings = null!;
        await Given("the settings of a project with a released package", async () => settings = await OpenSettingsAsync(releasedPackage: "1.0.0"));
        await When("the user deletes the project but keeps the server files", async () =>
        {
            User.Click(settings.DeleteButton);
            await (await App.PopupAsync()).ClickAsync("Delete");
            await (await App.PopupAsync()).ClickAsync("Keep");
            await App.ClosedAsync(_projectWindow);
        });
        await Then("the project is gone locally but clients still get updates", async () =>
        {
            App.ListedProjects.ShouldBeEmpty();
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.OK);
        });
    });
}
