using Avalonia.Headless.XUnit;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The credentials dialog that appears when a project does not carry its secrets. Unlocking with the project password is in <see cref="PortableProjectScenarios" />.</summary>
public sealed class CredentialsDialogScenarios : ScenarioTest
{
    public CredentialsDialogScenarios(ServerFixture server)
        : base(server)
    {
    }

    [AvaloniaFact]
    public Task Asks_for_the_credentials_of_a_project_that_does_not_store_them() => Scenario(async () =>
    {
        string privateKey = null!;
        await Given("a project saved without its credentials", async () =>
        {
            var created = await App.ExistingProjectAsync("Secretive", saveCredentials: false);
            privateKey = created.Secrets.PrivateKey!;
            (await App.Store.LoadAsync(created.Project.Path)).Secrets.PrivateKey.ShouldBeNull();
        });
        await When("the user opens it but cancels the credentials dialog", async () =>
        {
            App.ClickOpen(0);
            var dialog = await App.WindowAsync<CredentialsWindow>();
            User.Click(dialog.CancelButton);
            await App.ClosedAsync(dialog);
        });
        await Then("no project window opens", () => App.Dialogs.Open.OfType<ProjectWindow>().ShouldBeEmpty());
        CredentialsWindow credentials = null!;
        await When("the user opens it again and continues without entering anything", async () =>
        {
            App.ClickOpen(0);
            credentials = await App.WindowAsync<CredentialsWindow>();
            User.Click(credentials.AcceptButton);
        });
        await Then("the dialog explains what is required", () =>
        {
            credentials.ErrorText.IsVisible.ShouldBeTrue();
            credentials.ErrorText.Text!.ShouldContain("transfer password");
            credentials.IsVisible.ShouldBeTrue();
        });
        await When("the user enters the password and loads the private key from a file", () =>
        {
            User.Type(credentials.TransferPasswordBox, ServerFixture.SftpPassword);
            App.Picker.UserPicksFile(Context.WriteFile("key.pem", privateKey));
            User.Click(credentials.LoadKeyButton);
            credentials.PrivateKeyBox.Text!.ShouldBe(privateKey);
            User.Click(credentials.AcceptButton);
        });
        await Then("the project window opens", async () =>
        {
            await App.ClosedAsync(credentials);
            (await App.WindowAsync<ProjectWindow>()).Title.ShouldStartWith("Secretive");
        });
    });
}
