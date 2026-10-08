using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The new-project wizard: General, Authentication, Transfer, Statistics and Security.</summary>
public sealed class NewProjectWizardScenarios : ScenarioTest
{
    public NewProjectWizardScenarios(ServerFixture server)
        : base(server)
    {
    }

    private async Task<NewProjectWindow> OpenWizardAsync()
    {
        User.Click(App.Main.NewProjectButton);
        return await App.WindowAsync<NewProjectWindow>();
    }

    private static NewProjectViewModel ViewModel(NewProjectWindow wizard) => (NewProjectViewModel)wizard.DataContext!;

    private static string CurrentStep(NewProjectWindow wizard) => ViewModel(wizard).Steps.Single(s => s.IsCurrent).Title;

    private void FillTransfer(NewProjectWindow wizard, TransferProtocol protocol, string? password = null)
    {
        User.Select(User.Find<ComboBox>(wizard, "ProtocolBox"), protocol);
        var sftp = protocol == TransferProtocol.Sftp;
        User.Type(User.Find<TextBox>(wizard, "HostBox"), sftp ? Context.Server.SftpHost : Context.Server.FtpHost);
        User.Find<NumericUpDown>(wizard, "PortBox").Value = sftp ? Context.Server.SftpPort : Context.Server.FtpPort;
        User.Type(User.Find<TextBox>(wizard, "UsernameBox"), sftp ? ServerFixture.SftpUser : ServerFixture.FtpUser);
        User.Type(User.Find<TextBox>(wizard, "PasswordBox"), password ?? AdministrationApp.Password(protocol));
        User.Type(User.Find<TextBox>(wizard, "DirectoryBox"), sftp ? "/updates" : "/");
    }

    [AvaloniaFact]
    public Task Validates_every_step_before_moving_on() => Scenario(async () =>
    {
        var wizard = await OpenWizardAsync();
        await Then("the wizard starts on the General step", () =>
        {
            CurrentStep(wizard).ShouldBe("General");
            wizard.BackButton.IsEffectivelyEnabled.ShouldBeFalse();
            wizard.ErrorText.IsVisible.ShouldBeFalse();
        });
        await When("the user continues without a name", () => User.Click(wizard.ContinueButton));
        await Then("the wizard asks for the name and stays on the step", () =>
        {
            wizard.ErrorText.Text!.ShouldBe("Enter a project name.");
            CurrentStep(wizard).ShouldBe("General");
        });
        await When("the user enters a name and an invalid URL", () =>
        {
            User.Type(wizard.NameBox, "Trade Updater");
            User.Type(wizard.UpdateUrlBox, "ftp://not-http");
            User.Click(wizard.ContinueButton);
        });
        await Then("the wizard asks for an absolute HTTP(S) URL", () =>
        {
            wizard.ErrorText.Text!.ShouldContain("absolute HTTP(S) URL");
            wizard.FolderBox.Text!.ShouldBe(App.ProjectFolder("Trade Updater"));
        });
        await When("the URL is fixed", () =>
        {
            User.Type(wizard.UpdateUrlBox, Context.Server.HttpBaseUrl);
            User.Click(wizard.ContinueButton);
        });
        await Then("the Authentication step is shown and the first one is marked as done", () =>
        {
            CurrentStep(wizard).ShouldBe("Authentication");
            ViewModel(wizard).Steps[0].IsDone.ShouldBeTrue();
            wizard.ErrorText.IsVisible.ShouldBeFalse();
            wizard.BackButton.IsEffectivelyEnabled.ShouldBeTrue();
        });
        await When("HTTP authentication is enabled without a user name", () =>
        {
            User.Check(wizard.UseHttpAuthenticationBox, true);
            User.Click(wizard.ContinueButton);
        });
        await Then("the wizard asks for the user name", () => wizard.ErrorText.Text!.ShouldContain("user name for the HTTP authentication"));
        await When("authentication is switched off again", () =>
        {
            User.Check(wizard.UseHttpAuthenticationBox, false);
            User.Click(wizard.ContinueButton);
        });
        await Then("the Transfer step is shown", () => CurrentStep(wizard).ShouldBe("Transfer"));
        await When("the user continues without a host", () => User.Click(wizard.ContinueButton));
        await Then("the wizard asks for the host name", () => wizard.ErrorText.Text!.ShouldBe("Enter the server host name."));
        await When("the transfer settings are complete", () =>
        {
            FillTransfer(wizard, TransferProtocol.Sftp);
            User.Click(wizard.ContinueButton);
        });
        await Then("the Statistics step is shown", () => CurrentStep(wizard).ShouldBe("Statistics"));
        await When("statistics are enabled without a database", () =>
        {
            User.Check(User.Find<ToggleSwitch>(wizard, "EnabledBox"), true);
            User.Type(User.Find<TextBox>(wizard, "DbNameBox"), "");
            User.Click(wizard.ContinueButton);
        });
        await Then("the wizard asks for the database", () => wizard.ErrorText.Text!.ShouldContain("database"));
        await When("statistics are disabled again", () =>
        {
            User.Check(User.Find<ToggleSwitch>(wizard, "EnabledBox"), false);
            User.Click(wizard.ContinueButton);
        });
        await Then("the Security step offers to create the project", () =>
        {
            CurrentStep(wizard).ShouldBe("Security");
            ViewModel(wizard).ContinueText.ShouldBe("Create project");
        });
        await When("the user goes back", () => User.Click(wizard.BackButton));
        await Then("the Statistics step is shown again with the earlier steps still done", () =>
        {
            CurrentStep(wizard).ShouldBe("Statistics");
            ViewModel(wizard).Steps.Take(3).ShouldAllBe(s => s.IsDone);
        });
        await When("the user cancels", () => User.Click(wizard.CancelButton));
        await Then("the wizard closes and no project was created", async () =>
        {
            await App.ClosedAsync(wizard);
            App.ListedProjects.ShouldBeEmpty();
            Directory.Exists(App.ProjectFolder("Trade Updater")).ShouldBeFalse();
        });
    });

    [AvaloniaFact]
    public Task Tests_the_connection_and_trusts_the_sftp_host_key() => Scenario(async () =>
    {
        var wizard = await OpenWizardAsync();
        await Given("the user is on the Transfer step with the SFTP server filled in", () =>
        {
            User.Type(wizard.NameBox, "Sftp");
            User.Type(wizard.UpdateUrlBox, Context.Server.HttpBaseUrl);
            User.Click(wizard.ContinueButton);
            User.Click(wizard.ContinueButton);
            FillTransfer(wizard, TransferProtocol.Sftp);
        });
        await When("the user tests the connection but does not trust the unknown host key", async () =>
        {
            User.Click(User.Find<Button>(wizard, "TestButton"));
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Unknown host key");
            popup.Message.ShouldContain("SHA-256:");
            popup.Buttons.ShouldBe(["Cancel", "Trust"]);
            await popup.ClickAsync("Cancel");
        });
        await Then("the test reports that the host key was not trusted", () =>
            User.Find<TextBlock>(wizard, "TestResultText").Text!.ShouldBe("The host key was not trusted."));
        await When("the user tests again and trusts the host key", async () =>
        {
            User.Click(User.Find<Button>(wizard, "TestButton"));
            await (await App.PopupAsync()).ClickAsync("Trust");
        });
        await Then("the connection succeeds", async () =>
        {
            var result = User.Find<TextBlock>(wizard, "TestResultText");
            await User.WaitUntil(() => result.Text == "Connection successful.", "the connection test");
            ViewModel(wizard).Transfer.TrustedHostKeyFingerprint.ShouldNotBeNullOrEmpty();
        });
        await And("a second test needs no confirmation", async () =>
        {
            User.Click(User.Find<Button>(wizard, "TestButton"));
            var result = User.Find<TextBlock>(wizard, "TestResultText");
            await User.WaitUntil(() => result.Text == "Connection successful.", "the second connection test");
            App.Dialogs.Popups.Count.ShouldBe(2);
        });
    });

    [AvaloniaFact]
    public Task Rejects_a_wrong_password_when_testing_the_connection() => Scenario(async () =>
    {
        var wizard = await OpenWizardAsync();
        await Given("the FTP server with a wrong password", () =>
        {
            User.Type(wizard.NameBox, "Ftp");
            User.Type(wizard.UpdateUrlBox, Context.Server.HttpBaseUrl);
            User.Click(wizard.ContinueButton);
            User.Click(wizard.ContinueButton);
            FillTransfer(wizard, TransferProtocol.Ftp, password: "wrong");
        });
        await When("the user tests the connection", () => User.Click(User.Find<Button>(wizard, "TestButton")));
        await Then("the test fails with the server's message", async () =>
        {
            var result = User.Find<TextBlock>(wizard, "TestResultText");
            await User.WaitUntil(() => result.Text is not (null or "" or "Connecting..."), "the connection test to finish");
            result.Text!.ShouldNotBe("Connection successful.");
            ViewModel(wizard).Transfer.IsTesting.ShouldBeFalse();
        });
    });

    [AvaloniaFact]
    public Task Creates_a_project_over_sftp() => Scenario(async () =>
    {
        var wizard = await OpenWizardAsync();
        await Given("every step is filled in", () =>
        {
            User.Type(wizard.NameBox, "Trade Updater");
            User.Type(wizard.UpdateUrlBox, Context.Server.HttpBaseUrl.TrimEnd('/'));
            User.Click(wizard.ContinueButton);
            User.Click(wizard.ContinueButton);
            FillTransfer(wizard, TransferProtocol.Sftp);
            User.Click(wizard.ContinueButton);
            User.Click(wizard.ContinueButton);
            CurrentStep(wizard).ShouldBe("Security");
            User.Select(wizard.KeySizeBox, 2048);
            wizard.SaveCredentialsBox.IsChecked.ShouldBe(true);
            wizard.TestConnectionFirstBox.IsChecked.ShouldBe(true);
        });
        await When("the user creates the project without a project password", () => User.Click(wizard.ContinueButton));
        await Then("the wizard asks for one", () => wizard.ErrorText.Text!.ShouldContain("at least 8 characters"));
        await When("the user enters the password, creates the project and trusts the server", async () =>
        {
            User.Type(wizard.ProjectPasswordBox, AdministrationApp.ProjectPassword);
            User.Type(wizard.ProjectPasswordConfirmationBox, AdministrationApp.ProjectPassword);
            User.Click(wizard.ContinueButton);
            var popup = await App.PopupAsync();
            popup.Title.ShouldBe("Unknown host key");
            await popup.ClickAsync("Trust");
        });
        await Then("the wizard closes and the project window opens", async () =>
        {
            await App.ClosedAsync(wizard);
            var window = await App.WindowAsync<ProjectWindow>();
            window.Title.ShouldBe("Trade Updater - nUpdate Administration");
        });
        await And("the project folder holds project.nupdproj with the secrets under the password and a trailing slash on the URL", async () =>
        {
            App.ListedProjects.ShouldBe(["Trade Updater"]);
            File.Exists(App.ProjectFile("Trade Updater")).ShouldBeTrue();
            Directory.Exists(Path.Combine(App.ProjectFolder("Trade Updater"), "packages")).ShouldBeTrue();
            (await App.Store.LoadAsync(App.ProjectFile("Trade Updater"))).SecretsState.ShouldBe(nUpdate.Administration.Core.Projects.SecretsState.PasswordRequired);
            var saved = await App.SavedAsync("Trade Updater");
            saved.Project.UpdateUrl.ShouldBe(Context.Server.HttpBaseUrl);
            saved.Project.Transfer.Protocol.ShouldBe(TransferProtocol.Sftp);
            saved.Project.Transfer.TrustedHostKeyFingerprint.ShouldNotBeNullOrEmpty();
            saved.Secrets.TransferPassword.ShouldBe(ServerFixture.SftpPassword);
            saved.Secrets.PrivateKey.ShouldNotBeNullOrEmpty();
        });
    });

    [AvaloniaFact]
    public Task Creates_a_project_with_statistics_over_ftp() => Scenario(async () =>
    {
        var wizard = await OpenWizardAsync();
        await Given("a project over plain FTP with statistics on the MySQL container", () =>
        {
            User.Type(wizard.NameBox, "Stats");
            User.Type(wizard.UpdateUrlBox, Context.Server.HttpBaseUrl);
            User.Click(wizard.ContinueButton);
            User.Click(wizard.ContinueButton);
            FillTransfer(wizard, TransferProtocol.Ftp);
            User.Click(wizard.ContinueButton);
            User.Check(User.Find<ToggleSwitch>(wizard, "EnabledBox"), true);
            User.Type(User.Find<TextBox>(wizard, "DbHostBox"), "mysql");
            User.Type(User.Find<TextBox>(wizard, "DbNameBox"), ServerFixture.DbName);
            User.Type(User.Find<TextBox>(wizard, "DbUserBox"), ServerFixture.DbUser);
            User.Type(User.Find<TextBox>(wizard, "DbPasswordBox"), ServerFixture.DbPassword);
            User.Click(wizard.ContinueButton);
            User.Select(wizard.KeySizeBox, 2048);
            User.Check(wizard.SaveCredentialsBox, false);
        });
        await When("the user creates the project", () => User.Click(wizard.ContinueButton));
        await Then("the project window opens", async () =>
        {
            await App.ClosedAsync(wizard);
            (await App.WindowAsync<ProjectWindow>()).Title.ShouldStartWith("Stats");
        });
        await And("the statistics script is on the server and protected by the admin secret", async () =>
        {
            var saved = await App.Store.LoadAsync(App.ProjectFile("Stats"));
            saved.SecretsState.ShouldBe(nUpdate.Administration.Core.Projects.SecretsState.NotSaved);
            saved.Project.Statistics.Enabled.ShouldBeTrue();
            using var response = await Context.HttpClient.GetAsync(Context.Server.HttpBaseUrl + "nupdate-statistics.php/v2/projects/" + saved.Project.Id + "/statistics");
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
        });
    });

    [AvaloniaFact]
    public Task Refuses_a_name_that_is_already_in_use() => Scenario(async () =>
    {
        await Given("a project named Demo", async () => await App.ExistingProjectAsync("Demo"));
        var wizard = await OpenWizardAsync();
        await When("the user creates another project named Demo", () =>
        {
            User.Type(wizard.NameBox, "Demo");
            User.Type(wizard.UpdateUrlBox, Context.Server.HttpBaseUrl);
            User.Click(wizard.ContinueButton);
            User.Click(wizard.ContinueButton);
            FillTransfer(wizard, TransferProtocol.Ftp);
            User.Click(wizard.ContinueButton);
            User.Click(wizard.ContinueButton);
            User.Select(wizard.KeySizeBox, 2048);
            User.Check(wizard.SaveCredentialsBox, false);
            User.Click(wizard.ContinueButton);
        });
        await Then("the wizard stays open and explains the conflict", async () =>
        {
            await User.WaitUntil(() => wizard.ErrorText.IsEffectivelyVisible || !wizard.IsVisible, "the creation to finish");
            wizard.IsVisible.ShouldBeTrue();
            wizard.ErrorText.Text!.ShouldContain("already exists");
            App.ListedProjects.ShouldBe(["Demo"]);
        });
    });
}
