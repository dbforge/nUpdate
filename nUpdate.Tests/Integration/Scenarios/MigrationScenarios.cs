using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>
///     Opening a project of nUpdate 3 or 4: the file is converted, a project password is chosen, the migration assistant
///     publishes the packages for nUpdate 5 next to the old files and explains how to run both versions side by side,
///     and the old setup is retired at the end.
/// </summary>
public sealed class MigrationScenarios : ScenarioTest
{
    private static readonly Guid ProjectId = Guid.Parse("12345678-1234-1234-1234-123456789abc");

    public MigrationScenarios(ServerFixture server)
        : base(server)
    {
    }

    /// <summary>The folder the converted project is written to: a subfolder named after the project, since "legacy-project" is not its own folder.</summary>
    private static string MigratedFolder(string legacyPath) => Path.Combine(Path.GetDirectoryName(legacyPath)!, "Legacy");

    /// <summary>Writes the project file of nUpdate 4 pointing at the container servers and publishes one package the old way.</summary>
    private async Task<string> LegacyProjectAsync(bool published, bool statistics = false)
    {
        var folder = Path.Combine(Context.Root, "legacy-project");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, "Legacy.nupdproj");
        var transfer = Context.FtpSettings();
        if (published)
        {
            await using var ftp = await Context.ConnectTrustedAsync(transfer, IntegrationContext.FtpCredentials);
            await LegacyServer.PublishAsync(ftp, Context, ProjectId, "1.0.0.0", LegacyServer.Zip(("Program/app.exe", "version 1.0")));
            await LegacyServer.PublishStatisticsScriptAsync(ftp, Context);
        }

        static string Encrypt(string value) => LegacyAesCredentialDecryptor.Encrypt(value, LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword);
        var json = nUpdate.Tests.Administration.Core.ProjectStoreTests
            .LegacyProjectJson(true, LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword)
            .Replace("\"UpdateUrl\": \"https://updates.example.com/legacy\"", $"\"UpdateUrl\": \"{Context.Server.HttpBaseUrl}\"", StringComparison.Ordinal)
            .Replace("\"FtpHost\": \"ftp.example.com\"", $"\"FtpHost\": \"{Context.Server.FtpHost}\"", StringComparison.Ordinal)
            .Replace("\"FtpPort\": 2121", $"\"FtpPort\": {Context.Server.FtpPort}", StringComparison.Ordinal)
            .Replace("\"FtpProtocol\": 1", "\"FtpProtocol\": 0", StringComparison.Ordinal)
            .Replace("\"FtpUsePassiveMode\": false", "\"FtpUsePassiveMode\": true", StringComparison.Ordinal)
            .Replace("\"FtpDirectory\": \"/updates\"", "\"FtpDirectory\": \"/\"", StringComparison.Ordinal)
            .Replace("\"FtpUsername\": \"ftpuser\"", $"\"FtpUsername\": \"{ServerFixture.FtpUser}\"", StringComparison.Ordinal)
            .Replace(Encrypt("ftp-pw"), Encrypt(ServerFixture.FtpPassword), StringComparison.Ordinal)
            .Replace("\"Proxy\": { \"Address\": \"http://proxy:8080\"", "\"Proxy\": { \"Address\": \"\"", StringComparison.Ordinal);
        json = statistics
            ? json.Replace("\"SqlWebUrl\": \"db.example.com\"", "\"SqlWebUrl\": \"mysql\"", StringComparison.Ordinal)
                .Replace("\"SqlDatabaseName\": \"stats\"", $"\"SqlDatabaseName\": \"{ServerFixture.DbName}\"", StringComparison.Ordinal)
                .Replace("\"SqlUsername\": \"sqluser\"", $"\"SqlUsername\": \"{ServerFixture.DbUser}\"", StringComparison.Ordinal)
                .Replace(Encrypt("sql-pw"), Encrypt(ServerFixture.DbPassword), StringComparison.Ordinal)
            : json.Replace("\"UseStatistics\": true", "\"UseStatistics\": false", StringComparison.Ordinal);
        await File.WriteAllTextAsync(path, json);
        return path;
    }

    /// <summary>Opens the legacy file, confirms the conversion and chooses the project password; returns the project window.</summary>
    private async Task<ProjectWindow> OpenLegacyProjectAsync(string path, bool saveCredentials = true)
    {
        App.Picker.UserPicksFile(path);
        User.Click(App.Main.OpenButton);
        var converted = await App.PopupAsync();
        converted.Title.ShouldBe("Project converted");
        await converted.ClickAsync("OK");
        var password = await App.WindowAsync<ProjectPasswordWindow>();
        if (saveCredentials)
        {
            User.Type(password.PasswordBox, AdministrationApp.ProjectPassword);
            User.Type(password.ConfirmationBox, AdministrationApp.ProjectPassword);
        }
        else
        {
            User.Check(password.SaveCredentialsBox, false);
        }

        User.Click(password.AcceptButton);
        await App.ClosedAsync(password);
        return await App.WindowAsync<ProjectWindow>();
    }

    /// <summary>Waits for the migration assistant and for it to have read the server.</summary>
    private async Task<(MigrationWindow Window, MigrationViewModel Assistant)> AssistantAsync()
    {
        var window = await App.WindowAsync<MigrationWindow>();
        var assistant = (MigrationViewModel)window.DataContext!;
        await User.WaitUntil(() => assistant.IsPrepared && !assistant.IsBusy, "the assistant to read the server");
        window.UpdateLayout();
        return (window, assistant);
    }

    /// <summary>Goes through the steps with the suggested choices, starts the migration and waits for the guide.</summary>
    private static async Task MigrateWithTheAssistantAsync(MigrationWindow window, MigrationViewModel assistant)
    {
        while (!assistant.IsMigrateStep)
            User.Click(window.ContinueButton);
        assistant.ContinueText.ShouldBe("Start migration");
        User.Click(window.ContinueButton);
        await User.WaitUntil(() => assistant.IsSideBySideStep && !assistant.IsBusy, "the migration to finish");
        window.UpdateLayout();
    }

    [AvaloniaFact]
    public Task Converts_a_project_of_nUpdate_4_and_protects_it_with_a_project_password() => Scenario(async () =>
    {
        string path = null!;
        await Given("a project file written by nUpdate Administration 4 that published nothing", async () => path = await LegacyProjectAsync(published: false));
        ProjectPasswordWindow password = null!;
        await When("the user opens it", async () =>
        {
            App.Picker.UserPicksFile(path);
            User.Click(App.Main.OpenButton);
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Project converted");
            popup.Message.ShouldContain("earlier version");
            await popup.ClickAsync("OK");
            password = await App.WindowAsync<ProjectPasswordWindow>();
        });
        await Then("the dialog asks for a project password and validates it", () =>
        {
            User.Click(password.AcceptButton);
            password.ErrorText.Text!.ShouldContain("at least 8 characters");
            User.Type(password.PasswordBox, AdministrationApp.ProjectPassword);
            User.Type(password.ConfirmationBox, "something-else");
            User.Click(password.AcceptButton);
            password.ErrorText.Text!.ShouldBe("The passwords do not match.");
        });
        await When("the user confirms the password", async () =>
        {
            User.Type(password.ConfirmationBox, AdministrationApp.ProjectPassword);
            User.Click(password.AcceptButton);
            await App.ClosedAsync(password);
        });
        await Then("the project opens with its old data and no migration is needed", async () =>
        {
            var window = await App.WindowAsync<ProjectWindow>();
            window.Title.ShouldBe("Legacy - nUpdate Administration");
            await User.WaitUntil(() => window.MigrationStatusText.Text != "Not checked yet.", "the legacy check");
            window.MigrationBanner.IsVisible.ShouldBeFalse();
            window.MigrationStatusText.Text!.ShouldBe("No updates.json of nUpdate 3 or 4 on the server.");
            User.SelectPage(window.Nav, "Overview");
            window.UpdateUrlBox.Text!.ShouldBe(Context.Server.HttpBaseUrl);
            window.FolderBox.Text!.ShouldBe(MigratedFolder(path));
            window.PackageGrid.ItemsSource!.Cast<PackageItemViewModel>().Select(p => p.Version).ShouldBe(["1.1.0-beta.2", "1.0.0"]);
        });
        await And("the project is saved as project.nupdproj with its secrets under the password and remembers the old file", async () =>
        {
            var file = Path.Combine(MigratedFolder(path), "project.nupdproj");
            File.Exists(file).ShouldBeTrue();
            File.Exists(path).ShouldBeTrue();
            var saved = await App.Store.LoadAsync(file, AdministrationApp.ProjectPassword);
            saved.Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
            saved.Secrets.TransferPassword.ShouldBe(ServerFixture.FtpPassword);
            saved.Project.PublicKey.ShouldBe(TestKeys.PublicKey);
            saved.Project.LegacyProjectFile.ShouldBe(path);
            App.ListedProjects.ShouldBe(["Legacy"]);
            (await App.Store.ListAsync()).Single().Path.ShouldBe(file);
        });
    });

    [AvaloniaFact]
    public Task Walks_through_the_migration_and_keeps_nUpdate_4_working_until_it_is_retired() => Scenario(async () =>
    {
        string path = null!;
        ProjectWindow window = null!;
        MigrationWindow assistantWindow = null!;
        MigrationViewModel assistant = null!;
        await Given("a converted project whose packages are still published the old way", async () =>
        {
            path = await LegacyProjectAsync(published: true);
            window = await OpenLegacyProjectAsync(path);
        });
        await Then("the migration assistant opens by itself and explains what changes and what stays", async () =>
        {
            (assistantWindow, assistant) = await AssistantAsync();
            assistantWindow.Title.ShouldBe("Move Legacy to nUpdate 5");
            assistant.ServerState.ShouldContain("updates.json with 1 package, which applications built with nUpdate 3 or 4 read.");
            assistant.ServerState.ShouldContain("No nupdate.json yet.");
            assistant.KeptItems.ShouldContain(i => i.Contains("1.0.0.0/", StringComparison.Ordinal));
            assistant.KeptItems.ShouldContain(i => i.Contains(path, StringComparison.Ordinal));
            assistantWindow.ServerStateList.ItemCount.ShouldBe(2);
        });
        await When("the user looks at the packages", () =>
        {
            User.Click(assistantWindow.ContinueButton);
            assistantWindow.UpdateLayout();
        });
        await Then("the old package is listed with its new version, ready to migrate", () =>
        {
            assistant.IsPackagesStep.ShouldBeTrue();
            var package = assistant.Packages.Single();
            package.Title.ShouldBe("1.0.0.0 → 1.0.0");
            package.Include.ShouldBeTrue();
            package.Details.ShouldStartWith("1 file, 2 operations,");
            package.Details.ShouldContain("downloaded from " + Context.Server.HttpBaseUrl);
            assistantWindow.SelectionSummaryText.Text.ShouldBe("1 of 1 package to migrate selected.");
        });
        await When("the user goes on and starts the migration", () => MigrateWithTheAssistantAsync(assistantWindow, assistant));
        await Then("the guide explains how to run both versions side by side", () =>
        {
            assistant.Migrated.ShouldBeTrue();
            assistantWindow.SideBySideIntroductionText.Text!.ShouldStartWith("The migration is done. Follow these steps");
            assistantWindow.ClientSnippetText.Text!.ShouldContain(Context.Server.HttpBaseUrl + "nupdate.json");
            assistantWindow.VersionExampleText.Text!.ShouldContain("ApplicationVersion(\"1.1.0\")");
            assistantWindow.VersionExampleText.Text!.ShouldContain("higher than 1.0.0.0");
            assistantWindow.BridgeReleaseText.Text!.ShouldContain(path);
        });
        await When("the user checks the new feed", async () =>
        {
            User.Click(assistantWindow.CheckFeedButton);
            await App.IdleAsync(assistant);
        });
        await Then("it works the way an nUpdate 5 client sees it", () =>
        {
            assistantWindow.CheckStatusText.Text.ShouldBe("An application built with nUpdate 5 can update from this server.");
            assistant.CheckResults.Single().ShouldStartWith("✓ 1.0.0");
        });
        await And("the server has the new feed next to the untouched files of nUpdate 4", async () =>
        {
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.OK);
            (await Context.StatusAsync("packages/1.0.0/win.zip")).ShouldBe(System.Net.HttpStatusCode.OK);
            (await Context.StatusAsync("updates.json")).ShouldBe(System.Net.HttpStatusCode.OK);
            (await Context.StatusAsync("1.0.0.0/" + ProjectId + ".zip")).ShouldBe(System.Net.HttpStatusCode.OK);
            (await Context.HttpClient.GetStringAsync(Context.Server.HttpBaseUrl + "statistics.php")).ShouldBe(LegacyServer.StatisticsScriptOutput);
            File.Exists(path).ShouldBeTrue();
        });
        await When("the user closes the assistant", async () =>
        {
            User.Click(assistantWindow.ContinueButton);
            await App.ClosedAsync(assistantWindow);
        });
        await Then("the project shows the migrated package and points at the old setup", async () =>
        {
            await User.WaitUntil(() => !window.MigrationBanner.IsVisible && window.MigrationStatusText.Text!.Contains("still have their updates.json", StringComparison.Ordinal), "the status after the migration");
            window.PackageGrid.ItemsSource!.Cast<PackageItemViewModel>().Select(p => (p.Version, p.State)).ShouldBe([("1.1.0-beta.2", "Local only"), ("1.0.0", "Released")]);
        });
        await When("every installation has moved and the user retires the nUpdate 4 setup", async () =>
        {
            User.SelectPage(window.Nav, "Overview");
            User.Click(window.RetireLegacySetupButton);
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Retire the nUpdate 4 setup");
            popup.Message.ShouldContain("On the server: updates.json, statistics.php, 1.0.0.0/");
            await popup.ClickAsync("Delete");
            await App.IdleAsync((ProjectViewModel)window.DataContext!);
        });
        await Then("only the files of nUpdate 5 are left on the server", async () =>
        {
            await User.WaitUntil(() => window.MigrationStatusText.Text == "No updates.json of nUpdate 3 or 4 on the server.", "the status after retiring the old setup");
            (await Context.StatusAsync("updates.json")).ShouldBe(System.Net.HttpStatusCode.NotFound);
            (await Context.StatusAsync("1.0.0.0/" + ProjectId + ".zip")).ShouldBe(System.Net.HttpStatusCode.NotFound);
            (await Context.StatusAsync("statistics.php")).ShouldBe(System.Net.HttpStatusCode.NotFound);
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.OK);
        });
    });

    [AvaloniaFact]
    public Task Refuses_to_publish_until_the_packages_are_migrated() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("a converted project whose migration was postponed", async () =>
        {
            window = await OpenLegacyProjectAsync(await LegacyProjectAsync(published: true));
            var (assistantWindow, _) = await AssistantAsync();
            User.Click(assistantWindow.CancelButton);
            await App.ClosedAsync(assistantWindow);
        });
        await Then("the window shows the banner", () =>
        {
            window.MigrationBanner.IsVisible.ShouldBeTrue();
            window.MigrationBannerText.Text!.ShouldContain("nothing can be published");
        });
        PackageEditorWindow editor = null!;
        await When("the user creates and publishes a new package anyway", async () =>
        {
            User.Click(window.AddPackageButton);
            editor = await App.WindowAsync<PackageEditorWindow>();
            FillPackage(editor, "2.0.0");
            User.Click(editor.SaveButton);
            await App.IdleAsync((PackageEditorViewModel)editor.DataContext!);
        });
        await Then("the editor explains that the migration comes first", async () =>
        {
            editor.IsVisible.ShouldBeTrue();
            editor.ErrorText.Text!.ShouldContain("Migrate the published packages first");
            User.Click(editor.CancelButton);
            await App.ClosedAsync(editor);
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        });
        await When("the user migrates from the banner", async () =>
        {
            User.Click(window.MigrateButton);
            var (assistantWindow, assistant) = await AssistantAsync();
            await MigrateWithTheAssistantAsync(assistantWindow, assistant);
            User.Click(assistantWindow.ContinueButton);
            await App.ClosedAsync(assistantWindow);
        });
        await Then("publishing works", async () =>
        {
            await User.WaitUntil(() => !window.MigrationBanner.IsVisible, "the banner to disappear");
            User.Click(window.AddPackageButton);
            editor = await App.WindowAsync<PackageEditorWindow>();
            FillPackage(editor, "2.0.0");
            User.Click(editor.SaveButton);
            await App.ClosedAsync(editor);
            window.PackageGrid.ItemsSource!.Cast<PackageItemViewModel>().Select(p => (p.Version, p.State)).ShouldBe([("2.0.0", "Released"), ("1.1.0-beta.2", "Local only"), ("1.0.0", "Released")]);
        });
    });

    /// <summary>Fills the editor with a version, an English changelog and one file.</summary>
    private void FillPackage(PackageEditorWindow editor, string version)
    {
        var viewModel = (PackageEditorViewModel)editor.DataContext!;
        User.Type(editor.VersionBox, version);
        User.SelectRow(editor.SectionList, 1);
        User.Type(User.TextBoxFor(editor, viewModel.Changelogs.Single(c => c.Culture.Name == "en")), $"Changes in {version}.");
        User.SelectRow(editor.SectionList, 2);
        App.Picker.UserPicksFiles(Context.WriteFile($"src/{version}/app.exe", $"version {version}"));
        User.Click(editor.AddFilesButton);
    }

    [AvaloniaFact]
    public Task Opens_the_migration_assistant_from_the_settings() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("a converted project whose migration was postponed", async () =>
        {
            window = await OpenLegacyProjectAsync(await LegacyProjectAsync(published: true));
            var (assistantWindow, _) = await AssistantAsync();
            User.Click(assistantWindow.CancelButton);
            await App.ClosedAsync(assistantWindow);
        });
        await When("the user migrates from the settings", async () =>
        {
            User.Click(window.SettingsButton);
            var settings = await App.WindowAsync<ProjectSettingsWindow>();
            User.Click(settings.MigrateButton);
            var (assistantWindow, assistant) = await AssistantAsync();
            await MigrateWithTheAssistantAsync(assistantWindow, assistant);
            User.Click(assistantWindow.ContinueButton);
            await App.ClosedAsync(assistantWindow);
            User.Click(settings.CancelButton);
            await App.ClosedAsync(settings);
        });
        await Then("the project window no longer shows the banner", async () =>
        {
            await User.WaitUntil(() => !window.MigrationBanner.IsVisible, "the banner to disappear");
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.OK);
        });
    });

    [AvaloniaFact]
    public Task Sets_up_the_statistics_of_nUpdate_5_next_to_those_of_nUpdate_4() => Scenario(async () =>
    {
        MigrationWindow assistantWindow = null!;
        MigrationViewModel assistant = null!;
        await Given("a converted project with statistics whose packages are still published the old way", async () =>
        {
            await OpenLegacyProjectAsync(await LegacyProjectAsync(published: true, statistics: true));
            (assistantWindow, assistant) = await AssistantAsync();
        });
        await When("the user reaches the statistics step", () =>
        {
            User.Click(assistantWindow.ContinueButton);
            User.Click(assistantWindow.ContinueButton);
            assistantWindow.UpdateLayout();
        });
        await Then("it explains where both versions report to and what the server needs", () =>
        {
            assistant.IsStatisticsStep.ShouldBeTrue();
            assistant.StatisticsDetails.ShouldContain($"Endpoint: {Context.Server.HttpBaseUrl}nupdate-statistics.php");
            assistant.StatisticsDetails.ShouldContain($"Database: {ServerFixture.DbName} on mysql, user {ServerFixture.DbUser}");
            assistant.StatisticsDetails.ShouldContain("Admin secret: present");
            assistantWindow.StatisticsProblemText.IsVisible.ShouldBeFalse();
        });
        await When("the user starts the migration", () => MigrateWithTheAssistantAsync(assistantWindow, assistant));
        await Then("the statistics API of nUpdate 5 answers and nUpdate 4 keeps its script", async () =>
        {
            assistant.Migrated.ShouldBeTrue();
            using var info = await Context.HttpClient.GetAsync(Context.Server.HttpBaseUrl + "nupdate-statistics.php/v2");
            info.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            (await info.Content.ReadAsStringAsync()).ShouldContain("\"version\":2");
            (await Context.HttpClient.GetStringAsync(Context.Server.HttpBaseUrl + "statistics.php")).ShouldBe(LegacyServer.StatisticsScriptOutput);
            User.Click(assistantWindow.CheckFeedButton);
            await App.IdleAsync(assistant);
            assistant.CheckResults.ShouldContain("✓ nupdate-statistics.php answers.");
        });
    });

    [AvaloniaFact]
    public Task A_converted_project_can_keep_its_credentials_out_of_the_file() => Scenario(async () =>
    {
        string path = null!;
        await Given("a project file of nUpdate 4", async () => path = await LegacyProjectAsync(published: false));
        await When("the user opens it and chooses not to save the credentials", async () => await OpenLegacyProjectAsync(path, saveCredentials: false));
        await Then("the file holds no secrets and asks for them next time", async () =>
        {
            var loaded = await App.Store.LoadAsync(Path.Combine(MigratedFolder(path), "project.nupdproj"));
            loaded.SecretsState.ShouldBe(nUpdate.Administration.Core.Projects.SecretsState.NotSaved);
            loaded.Project.Secrets.ShouldBeNull();
        });
    });
}
