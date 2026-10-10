using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The Packages tab of the project window and the buttons in its header.</summary>
public sealed class ProjectWindowScenarios(ServerFixture server) : ScenarioTest(server)
{
    private static List<(string Version, string State)> Rows(ProjectWindow window)
    {
        User.Pump();
        return window.PackageGrid.ItemsSource!.Cast<PackageItemViewModel>().Select(p => (p.Version, p.State)).ToList();
    }

    private static bool ShowsText(Window window, string text) =>
        window.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == text);

    private static ProjectViewModel ViewModel(ProjectWindow window) => (ProjectViewModel)window.DataContext!;

    [AvaloniaFact]
    public Task Shows_the_packages_newest_first() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("a project with a released and a local package", async () =>
        {
            var project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(project, "1.0.0", "First release", publish: true);
            await App.ExistingPackageAsync(project, "1.1.0", "Dark mode", publish: false);
        });
        await When("the user opens the project",
            async () => window = await App.OpenListedProjectAsync("Trade Updater"));
        await Then("both packages are listed, newest first, with their state", () =>
        {
            Rows(window).ShouldBe([("1.1.0", "Local only"), ("1.0.0", "Released")]);
            ShowsText(window, "2 packages").ShouldBeTrue();
            window.TransferBox.Text!.ShouldBe(
                $"Sftp {ServerFixture.SftpUser}@{Context.Server.SftpHost}:{Context.Server.SftpPort}/updates");
        });
        await And("the package buttons need a selection", () =>
        {
            window.AddPackageButton.IsEffectivelyEnabled.ShouldBeTrue();
            window.EditPackageButton.IsEffectivelyEnabled.ShouldBeFalse();
            window.PublishPackageButton.IsEffectivelyEnabled.ShouldBeFalse();
            window.DeletePackageButton.IsEffectivelyEnabled.ShouldBeFalse();
        });
        await When("the released package is selected", () => User.SelectRow(window.PackageGrid, 1));
        await Then("it can be edited and deleted but not published again", () =>
        {
            window.EditPackageButton.IsEffectivelyEnabled.ShouldBeTrue();
            window.DeletePackageButton.IsEffectivelyEnabled.ShouldBeTrue();
            window.PublishPackageButton.IsEffectivelyEnabled.ShouldBeFalse();
        });
        await When("the local package is selected", () => User.SelectRow(window.PackageGrid, 0));
        await Then("it can be published", () => window.PublishPackageButton.IsEffectivelyEnabled.ShouldBeTrue());
    });

    [AvaloniaFact]
    public Task Filters_the_package_list_with_the_search_box() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("a project with three packages", async () =>
        {
            var project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(project, "1.0.0", "First release", publish: false);
            await App.ExistingPackageAsync(project, "1.1.0", "Dark mode", publish: false);
            await App.ExistingPackageAsync(project, "2.0.0", "New importer", publish: false);
            window = await App.OpenListedProjectAsync("Trade Updater");
        });
        await When("the user searches for a word of a description", () => User.Type(window.SearchBox, "dark"));
        await Then("only the matching package is shown", () => Rows(window).Select(r => r.Version).ShouldBe(["1.1.0"]));
        await When("the user searches for a version prefix", () => User.Type(window.SearchBox, "1."));
        await Then("both 1.x packages are shown",
            () => Rows(window).Select(r => r.Version).ShouldBe(["1.1.0", "1.0.0"]));
        await When("the search is cleared", () => User.Type(window.SearchBox, ""));
        await Then("all packages are shown again", () => Rows(window).Count.ShouldBe(3));
    });

    [AvaloniaFact]
    public Task Publishes_a_local_package() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        ProjectLoadResult project = null!;
        await Given("a project with a package that was only built locally", async () =>
        {
            project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(project, "1.0.0", "First release", publish: false);
            window = await App.OpenListedProjectAsync("Trade Updater");
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        });
        await When("the user publishes it but cancels the confirmation", async () =>
        {
            User.SelectRow(window.PackageGrid, 0);
            User.Click(window.PublishPackageButton);
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Publish package");
            popup.Message.ShouldContain("1.0.0");
            await popup.ClickAsync("Cancel");
        });
        await Then("the package is still local", () => Rows(window).ShouldBe([("1.0.0", "Local only")]));
        await When("the user publishes it and confirms", async () =>
        {
            User.Click(window.PublishPackageButton);
            await (await App.PopupAsync()).ClickAsync("Publish");
            await App.IdleAsync(ViewModel(window));
        });
        await Then("the package is released", () =>
        {
            Rows(window).ShouldBe([("1.0.0", "Released")]);
            window.ErrorText.IsEffectivelyVisible.ShouldBeFalse();
        });
        await And("clients find it in nupdate.json on the server", async () =>
        {
            var remote = (await App.Feeds.LoadRemoteAsync(project.Project, project.Secrets))!;
            remote.Packages.Single().Version.ToString().ShouldBe("1.0.0");
            remote.Packages.Single().Files.Single().Path.ShouldBe("packages/1.0.0/any.zip");
            (await Context.StatusAsync("packages/1.0.0/any.zip")).ShouldBe(System.Net.HttpStatusCode.OK);
        });
        await And("the history records the upload", () =>
        {
            User.SelectPage(window.Nav, "History");
            window.HistoryGrid.ItemsSource!.Cast<LogItemViewModel>().Select(l => (l.Kind, l.Version))
                .ShouldContain(("Upload", "1.0.0"));
        });
    });

    [AvaloniaFact]
    public Task Deletes_a_released_package_from_the_server() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        ProjectLoadResult project = null!;
        await Given("a project with two released packages", async () =>
        {
            project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(project, "1.0.0", "First release", publish: true);
            await App.ExistingPackageAsync(project, "1.1.0", "Dark mode", publish: true);
            window = await App.OpenListedProjectAsync("Trade Updater");
        });
        await When("the user deletes the older one and confirms", async () =>
        {
            User.SelectRow(window.PackageGrid, 1);
            User.Click(window.DeletePackageButton);
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Delete package");
            popup.Message.ShouldContain("from the server and locally");
            await popup.ClickAsync("Delete");
            await App.IdleAsync(ViewModel(window));
        });
        await Then("only the newer package is left", () =>
        {
            Rows(window).ShouldBe([("1.1.0", "Released")]);
            ShowsText(window, "1 package").ShouldBeTrue();
        });
        await And("the server no longer offers it", async () =>
        {
            var remote = (await App.Feeds.LoadRemoteAsync(project.Project, project.Secrets))!;
            remote.Packages.Select(c => c.Version.ToString()).ShouldBe(["1.1.0"]);
            (await Context.StatusAsync("packages/1.0.0/any.zip")).ShouldBe(System.Net.HttpStatusCode.NotFound);
            Directory.Exists(project.Project.PackageDirectory(new nUpdate.Updating.UpdateVersion("1.0.0")))
                .ShouldBeFalse();
        });
    });

    [AvaloniaFact]
    public Task Reports_a_server_that_cannot_be_reached_when_publishing() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("a project whose server is wrong", async () =>
        {
            var project = await App.ExistingProjectAsync("Broken", TransferProtocol.Ftp);
            await App.ExistingPackageAsync(project, "1.0.0", "First release", publish: false);
            project.Project.Transfer.Port = 1;
            await App.Projects.SaveAsync(project.Project, project.Secrets, AdministrationApp.ProjectPassword);
            window = await App.OpenListedProjectAsync("Broken");
        });
        await When("the user publishes the package", async () =>
        {
            User.SelectRow(window.PackageGrid, 0);
            User.Click(window.PublishPackageButton);
            await (await App.PopupAsync()).ClickAsync("Publish");
        });
        await Then("an error explains what failed and the package stays local", async () =>
        {
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Error while publishing the package");
            await popup.ClickAsync("Close");
            Rows(window).ShouldBe([("1.0.0", "Local only")]);
        });
    });

    [AvaloniaFact]
    public Task Opens_the_settings() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("an open project", async () =>
        {
            await App.ExistingProjectAsync("Trade Updater");
            window = await App.OpenListedProjectAsync("Trade Updater");
        });
        await When("the user opens the settings", () => User.Click(window.SettingsButton));
        await Then("the settings dialog of the project appears", async () =>
        {
            var settings = await App.WindowAsync<ProjectSettingsWindow>();
            settings.Title.ShouldBe("Settings of Trade Updater");
            User.Click(settings.CancelButton);
            await App.ClosedAsync(settings);
        });
    });
}
