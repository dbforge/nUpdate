using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The Overview tab of the project window: the project data, what to copy into an application, and the feed check.</summary>
public sealed class ProjectOverviewScenarios : ScenarioTest
{
    public ProjectOverviewScenarios(ServerFixture server)
        : base(server)
    {
    }

    private ProjectLoadResult _project = null!;

    private async Task<ProjectWindow> OpenOverviewAsync()
    {
        _project = await App.ExistingProjectAsync("Trade Updater");
        var window = await App.OpenListedProjectAsync("Trade Updater");
        User.SelectPage(window.Nav, "Overview");
        return window;
    }

    [AvaloniaFact]
    public Task Shows_the_project_data() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("an open project on its Overview tab", async () => window = await OpenOverviewAsync());
        await Then("the URLs, the upload target and the keys are shown", () =>
        {
            window.UpdateUrlBox.Text!.ShouldBe(Context.Server.HttpBaseUrl);
            window.FeedUrlBox.Text!.ShouldBe(Context.Server.HttpBaseUrl + "nupdate.json");
            window.FolderBox.Text!.ShouldBe(App.ProjectFolder("Trade Updater"));
            window.TransferBox.Text!.ShouldBe($"Sftp {ServerFixture.SftpUser}@{Context.Server.SftpHost}:{Context.Server.SftpPort}/updates");
            window.ProjectIdBox.Text!.ShouldBe(_project.Project.Id.ToString());
            window.PublicKeyBox.Text!.ShouldBe(_project.Project.PublicKey);
            window.PackagesSummaryText.Text!.ShouldBe("0 packages released");
            window.FeedStatusText.Text!.ShouldBe("Not checked yet.");
        });
    });

    [AvaloniaFact]
    public Task Copies_the_feed_url_the_public_key_and_the_source() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("an open project on its Overview tab", async () => window = await OpenOverviewAsync());
        await When("the user copies the feed URL", () => User.Click(window.CopyFeedUrlButton));
        await Then("the clipboard holds the URL of nupdate.json", () => App.Clipboard.Text!.ShouldBe(Context.Server.HttpBaseUrl + "nupdate.json"));
        await When("the user copies the public key", () => User.Click(window.CopyPublicKeyButton));
        await Then("the clipboard holds the key", () => App.Clipboard.Text!.ShouldBe(_project.Project.PublicKey));
        await When("the user copies the C# source", () => User.Click(window.CopySourceButton));
        await Then("the clipboard holds a C# snippet with the key and the feed URL", () =>
        {
            App.Clipboard.Text!.ShouldStartWith("var manager = new UpdateManager(");
            App.Clipboard.Text!.ShouldContain(_project.Project.PublicKey);
            App.Clipboard.Text!.ShouldContain(Context.Server.HttpBaseUrl + "nupdate.json");
        });
        await When("the user switches to Visual Basic and copies again", () =>
        {
            User.Select(window.SourceLanguageBox, "Visual Basic");
            User.Click(window.CopySourceButton);
        });
        await Then("the clipboard holds a Visual Basic snippet", () => App.Clipboard.Text!.ShouldStartWith("Dim manager As New UpdateManager("));
    });

    [AvaloniaFact]
    public Task Checks_whether_clients_can_reach_the_feed() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("an open project that has not published anything yet", async () => window = await OpenOverviewAsync());
        await When("the user checks the feed", async () =>
        {
            User.Click(window.CheckFeedButton);
            await User.WaitUntil(() => window.FeedStatusText.Text is not ("Not checked yet." or "Checking..."), "the feed check");
        });
        await Then("the check finds the server but no feed yet", () => window.FeedStatusText.Text!.ShouldBe("No nupdate.json on the server yet."));
        await When("a package is published and the user checks again", async () =>
        {
            await App.ExistingPackageAsync(_project, "1.0.0", "First release", publish: true);
            User.Click(window.CheckFeedButton);
            await User.WaitUntil(() => window.FeedStatusText.Text?.StartsWith("Reachable", StringComparison.Ordinal) == true, "the second feed check");
        });
        await Then("the check reports the feed with its one package", () => window.FeedStatusText.Text!.ShouldBe("Reachable, 1 package"));
        await And("the server has no files of nUpdate 3 or 4", async () =>
        {
            await User.WaitUntil(() => window.MigrationStatusText.Text != "Not checked yet.", "the legacy check");
            window.MigrationStatusText.Text!.ShouldBe("No updates.json of nUpdate 3 or 4 on the server.");
            window.RetireLegacySetupButton.IsEffectivelyVisible.ShouldBeFalse();
            window.MigrationAssistantButton.IsEffectivelyVisible.ShouldBeFalse();
        });
    });
}
