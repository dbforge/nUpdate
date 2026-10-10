using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Platform;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The Statistics and History tabs of the project window.</summary>
public sealed class ProjectStatisticsAndHistoryScenarios(ServerFixture server) : ScenarioTest(server)
{
    [AvaloniaFact]
    public Task Tells_when_statistics_are_disabled() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("an open project without statistics", async () =>
        {
            await App.ExistingProjectAsync("Quiet");
            window = await App.OpenListedProjectAsync("Quiet");
        });
        await When("the user looks at the Statistics tab", () => User.SelectPage(window.Nav, "Statistics"));
        await Then("it says that statistics are disabled and cannot be refreshed", () =>
        {
            window.StatisticsStatusText.Text!.ShouldBe("Statistics are disabled for this project.");
            window.RefreshStatisticsButton.IsEffectivelyEnabled.ShouldBeFalse();
            window.StatisticsGrid.ItemsSource!.Cast<VersionStatistics>().ShouldBeEmpty();
        });
    });

    [AvaloniaFact]
    public Task Shows_the_downloads_counted_by_the_server() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("a project with statistics whose package a client has downloaded", async () =>
        {
            var project = await App.ExistingProjectAsync("Counted", TransferProtocol.Ftp, statistics: true);
            await App.ExistingPackageAsync(project, "1.1.0", "First release", publish: true);
            await DownloadAsClientAsync(project.Project.PublicKey);
            window = await App.OpenListedProjectAsync("Counted");
        });
        await When("the user opens the Statistics tab and refreshes", async () =>
        {
            User.SelectPage(window.Nav, "Statistics");
            User.Click(window.RefreshStatisticsButton);
            // The window already loads the statistics when it opens, so "Last updated" alone may belong to that load.
            await User.WaitUntil(
                () => window.StatisticsStatusText.Text != "Loading..." &&
                      window.StatisticsUpdatedText.Text?.StartsWith("Last updated", StringComparison.Ordinal) == true,
                "the statistics to load");
        });
        await Then("the download is counted for the version", () =>
        {
            var viewModel = (ProjectViewModel)window.DataContext!;
            viewModel.TotalDownloads.ShouldBe(1);
            window.StatisticsStatusText.Text!.ShouldBe("across 1 version");
            window.StatisticsGrid.ItemsSource!.Cast<VersionStatistics>()
                .Select(v => (v.Version.ToString(), v.Downloads)).ShouldBe([("1.1.0", 1L)]);
        });
    });

    [AvaloniaFact]
    public Task Lists_what_happened_to_the_project() => Scenario(async () =>
    {
        ProjectWindow window = null!;
        await Given("a project whose package was created, uploaded and deleted", async () =>
        {
            var project = await App.ExistingProjectAsync("Busy");
            await App.ExistingPackageAsync(project, "1.0.0", "First release", publish: true);
            await App.Publisher.DeletePackageAsync(project.Project, project.Secrets, new UpdateVersion("1.0.0"));
            window = await App.OpenListedProjectAsync("Busy");
        });
        await When("the user opens the History tab", () => User.SelectPage(window.Nav, "History"));
        await Then("the entries are listed newest first with the user who made them", () =>
        {
            var rows = window.HistoryGrid.ItemsSource!.Cast<LogItemViewModel>().ToList();
            rows.Select(r => (r.Kind, r.Version)).ShouldBe([
                ("Delete", "1.0.0"), ("Upload", "1.0.0"), ("Create", "1.0.0"), ("Create", "-")
            ]);
            rows.ShouldAllBe(r => r.User.Contains(Environment.UserName, StringComparison.Ordinal));
            rows.ShouldAllBe(r => r.Time != "-");
        });
    });

    /// <summary>What a client application does: search for updates and download the package, which the server counts.</summary>
    private async Task DownloadAsClientAsync(string publicKey)
    {
        var applicationInfo = Substitute.For<IApplicationInfo>();
        applicationInfo.ProductName.Returns("CountedApp-" + Guid.NewGuid().ToString("N"));
        applicationInfo.DeclaredVersion.Returns("1.0.0");
        applicationInfo.UserAgentProduct.Returns("CountedApp/1.0");
        var systemInformation = Substitute.For<ISystemInformation>();
        systemInformation.RuntimeIdentifier.Returns("win-x64");
        systemInformation.OperatingSystemName.Returns("Windows 11");
        var services = new UpdateManagerServices
        { ApplicationInfo = applicationInfo, SystemInformation = systemInformation };
        using var manager = new UpdateManager(new Uri(Context.Server.HttpBaseUrl + "nupdate.json"), publicKey,
            services: services);
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        await manager.DownloadAsync();
        manager.DeleteDownloads();
    }
}
