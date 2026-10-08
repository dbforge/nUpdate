using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>A project is a folder: it can be moved, copied to another computer and opened there with its project password.</summary>
public sealed class PortableProjectScenarios : ScenarioTest
{
    public PortableProjectScenarios(ServerFixture server)
        : base(server)
    {
    }

    [AvaloniaFact]
    public Task Opens_a_project_folder_that_was_moved() => Scenario(async () =>
    {
        var moved = Path.Combine(Context.Root, "elsewhere", "Trade Updater");
        await Given("a project with a local package whose folder was moved and forgotten", async () =>
        {
            var created = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(created, "1.0.0", "First", publish: false);
            Directory.CreateDirectory(Path.GetDirectoryName(moved)!);
            Directory.Move(App.ProjectFolder("Trade Updater"), moved);
            await App.Store.UnregisterAsync(created.Project.Id);
            await App.MainViewModel.RefreshAsync();
            App.ListedProjects.ShouldBeEmpty();
        });
        await When("the user opens the project file from the new folder", () =>
        {
            App.Picker.UserPicksFile(Path.Combine(moved, UpdateProject.FileName));
            User.Click(App.Main.OpenButton);
        });
        await Then("the project window opens without asking anything, since the password is remembered", async () =>
        {
            var window = await App.WindowAsync<ProjectWindow>();
            window.Title.ShouldBe("Trade Updater - nUpdate Administration");
            App.Dialogs.Open.OfType<CredentialsWindow>().ShouldBeEmpty();
            window.PackageGrid.ItemsSource!.Cast<PackageItemViewModel>().Select(p => (p.Version, p.State)).ShouldBe([("1.0.0", "Local only")]);
            User.SelectPage(window.Nav, "Overview");
            window.FolderBox.Text!.ShouldBe(moved);
        });
        await And("it is listed from its new location", async () =>
        {
            App.ListedProjects.ShouldBe(["Trade Updater"]);
            (await App.Store.ListAsync()).Single().Path.ShouldBe(Path.Combine(moved, UpdateProject.FileName));
        });
    });

    [AvaloniaFact]
    public Task Asks_for_the_project_password_on_another_computer() => Scenario(async () =>
    {
        ProjectLoadResult created = null!;
        await Given("a project whose password this computer does not know", async () =>
        {
            created = await App.ExistingProjectAsync("Trade Updater");
            await App.Passwords.RemoveAsync(created.Project.Id);
        });
        CredentialsWindow unlock = null!;
        await When("the user opens it with the wrong password", async () =>
        {
            App.ClickOpen(0);
            unlock = await App.WindowAsync<CredentialsWindow>();
            unlock.Title.ShouldBe("Unlock Trade Updater");
            User.Type(unlock.ProjectPasswordBox, "wrong-password");
            User.Click(unlock.AcceptButton);
        });
        await Then("the dialog says so and stays open", async () =>
        {
            // The password is checked off the UI thread, so the message appears a moment after the click.
            await User.WaitUntil(() => !string.IsNullOrEmpty(unlock.ErrorText.Text), "the password error");
            unlock.ErrorText.Text!.ShouldBe("The project password is wrong.");
            unlock.IsVisible.ShouldBeTrue();
        });
        await When("the user enters the right password and lets this computer remember it", () =>
        {
            User.Type(unlock.ProjectPasswordBox, AdministrationApp.ProjectPassword);
            User.Check(unlock.RememberBox, true);
            User.Click(unlock.AcceptButton);
        });
        await Then("the project window opens", async () =>
        {
            await App.ClosedAsync(unlock);
            (await App.WindowAsync<ProjectWindow>()).Title.ShouldStartWith("Trade Updater");
        });
        await And("the password is remembered for the next time", async () => (await App.Passwords.GetAsync(created.Project.Id)).ShouldBe(AdministrationApp.ProjectPassword));
    });

    [AvaloniaFact]
    public Task Cancelling_the_password_dialog_opens_nothing() => Scenario(async () =>
    {
        await Given("a project whose password this computer does not know", async () =>
        {
            var created = await App.ExistingProjectAsync("Trade Updater");
            await App.Passwords.RemoveAsync(created.Project.Id);
        });
        await When("the user opens it but cancels the password dialog", async () =>
        {
            App.ClickOpen(0);
            var unlock = await App.WindowAsync<CredentialsWindow>();
            User.Click(unlock.CancelButton);
            await App.ClosedAsync(unlock);
        });
        await Then("no project window opens", () => App.Dialogs.Open.OfType<ProjectWindow>().ShouldBeEmpty());
    });
}
