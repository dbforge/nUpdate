using Avalonia.Headless.XUnit;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The main window: the list of known projects and the ways to open one.</summary>
public sealed class MainWindowScenarios(ServerFixture server) : ScenarioTest(server)
{
    [AvaloniaFact]
    public Task Starts_with_an_empty_project_list() => Scenario(async () =>
    {
        await Then("the list of known projects is empty", () =>
        {
            App.ListedProjects.ShouldBeEmpty();
            App.Main.StatusText.Text!.ShouldBe("0 projects");
        });
        await And("only the actions that need no project are available", () =>
        {
            App.Main.NewProjectButton.IsEffectivelyEnabled.ShouldBeTrue();
            App.Main.OpenButton.IsEffectivelyEnabled.ShouldBeTrue();
            App.Main.EmptyState.IsVisible.ShouldBeTrue();
            App.Main.ProjectList.IsVisible.ShouldBeFalse();
        });
        await When("the user refreshes the list", () => User.Click(App.Main.RefreshButton));
        await Then("it is still empty", () => App.Main.StatusText.Text!.ShouldBe("0 projects"));
    });

    [AvaloniaFact]
    public Task Opens_a_project_file_the_user_picks() => Scenario(async () =>
    {
        string path = null!;
        await Given("a project file that is not in the list", async () =>
        {
            var created = await App.ExistingProjectAsync("Trade Updater");
            path = created.Project.Path;
            await App.Store.UnregisterAsync(created.Project.Id);
            await App.MainViewModel.RefreshAsync();
            App.ListedProjects.ShouldBeEmpty();
        });
        await When("the user opens it with the file dialog", () =>
        {
            App.Picker.UserPicksFile(path);
            User.Click(App.Main.OpenButton);
        });
        await Then("the project window opens", async () =>
        {
            var window = await App.WindowAsync<ProjectWindow>();
            window.Title.ShouldBe("Trade Updater - nUpdate Administration");
        });
        await And("the project is listed from now on", () =>
        {
            App.ListedProjects.ShouldBe(["Trade Updater"]);
            App.Main.StatusText.Text!.ShouldBe("1 project");
        });
    });

    [AvaloniaFact]
    public Task Reports_a_project_file_that_cannot_be_opened() => Scenario(async () =>
    {
        await When("the user picks a file that does not exist", () =>
        {
            App.Picker.UserPicksFile(Path.Combine(Context.Root, "missing.nupdproj"));
            User.Click(App.Main.OpenButton);
        });
        await Then("an error explains the problem", async () =>
        {
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Error while opening the project");
            popup.Buttons.ShouldBe(["Close"]);
            await popup.ClickAsync("Close");
        });
        await And("nothing was added to the list", () => App.ListedProjects.ShouldBeEmpty());
    });

    [AvaloniaFact]
    public Task Opens_a_known_project_from_the_list() => Scenario(async () =>
    {
        await Given("two known projects", async () =>
        {
            await App.ExistingProjectAsync("Alpha");
            await App.ExistingProjectAsync("Beta");
            App.ListedProjects.ShouldBe(["Alpha", "Beta"]);
        });
        await When("the user selects the second one and opens it", () =>
        {
            App.ClickOpen(1);
        });
        await Then("the project window of that project opens", async () =>
        {
            var window = await App.WindowAsync<ProjectWindow>();
            window.Title.ShouldStartWith("Beta");
        });
    });

    [AvaloniaFact]
    public Task Removes_a_project_from_the_list_but_keeps_its_files() => Scenario(async () =>
    {
        string path = null!;
        await Given("a known project", async () => path = (await App.ExistingProjectAsync("Demo")).Project.Path);
        await When("the user removes it from the list but changes their mind", async () =>
        {
            App.ClickRemove(0);
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Remove from list");
            popup.Message.ShouldContain("The project folder stays where it is.");
            await popup.ClickAsync("Cancel");
        });
        await Then("the project is still listed", () => App.ListedProjects.ShouldBe(["Demo"]));
        await When("the user removes it and confirms", async () =>
        {
            App.ClickRemove(0);
            await (await App.PopupAsync()).ClickAsync("Remove");
        });
        await Then("the list is empty but the project file still exists", async () =>
        {
            await User.WaitUntil(() => App.ListedProjects.Count == 0, "the list to update");
            App.Main.StatusText.Text!.ShouldBe("0 projects");
            File.Exists(path).ShouldBeTrue();
        });
    });
}
