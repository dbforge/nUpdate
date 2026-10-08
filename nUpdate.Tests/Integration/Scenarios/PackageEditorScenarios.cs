using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Tests.Integration.Scenarios.Support;
using nUpdate.Tests.Integration.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Integration.Scenarios;

/// <summary>The package editor: creating packages with files, operations and conditions, and editing published ones.</summary>
public sealed class PackageEditorScenarios(ServerFixture server) : ScenarioTest(server)
{
    private ProjectLoadResult _project = null!;
    private ProjectWindow _projectWindow = null!;

    private async Task<PackageEditorWindow> OpenNewPackageAsync(bool statistics = false)
    {
        _project = await App.ExistingProjectAsync("Trade Updater", statistics: statistics);
        _projectWindow = await App.OpenListedProjectAsync("Trade Updater");
        User.Click(_projectWindow.AddPackageButton);
        return await App.WindowAsync<PackageEditorWindow>();
    }

    private static PackageEditorViewModel ViewModel(PackageEditorWindow editor) =>
        (PackageEditorViewModel)editor.DataContext!;

    private static void GoTo(PackageEditorWindow editor, string section)
    {
        var item = editor.SectionList.Items.Cast<EditorSection>().Single(s => s.Title == section);
        User.SelectRow(editor.SectionList, editor.SectionList.Items.IndexOf(item));
    }

    private static TextBox EnglishChangelog(PackageEditorWindow editor) =>
        User.TextBoxFor(editor, ViewModel(editor).Changelogs.Single(c => c.Culture.Name == "en"));

    private List<(string Version, string State)> Rows() =>
        _projectWindow.PackageGrid.ItemsSource!.Cast<PackageItemViewModel>().Select(p => (p.Version, p.State)).ToList();

    private async Task<List<PackageInfo>> RemoteAsync() =>
        (await App.Feeds.LoadRemoteAsync(_project.Project, _project.Secrets))!.Packages;

    /// <summary>The operations as the installer will read them from the manifest of the package file of a platform.</summary>
    private async Task<List<Operation>> PackagedOperationsAsync(UpdateVersion version,
        string platform = PackagePlatform.Any)
    {
        var path = Path.Combine(_project.Project.PlatformDirectory(version, platform), PackageLayout.ManifestFileName);
        return Serializer.Deserialize<PackageManifest>(await File.ReadAllTextAsync(path))!.Operations;
    }

    [AvaloniaFact]
    public Task Suggests_the_next_version_and_validates_the_input() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        await Given("a project with a package 1.2.0 and the editor for a new one", async () =>
        {
            _project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(_project, "1.2.0", "Current", publish: false);
            _projectWindow = await App.OpenListedProjectAsync("Trade Updater");
            User.Click(_projectWindow.AddPackageButton);
            editor = await App.WindowAsync<PackageEditorWindow>();
        });
        await Then("the editor suggests the next revision and offers all sections", () =>
        {
            editor.Title.ShouldBe("New package for Trade Updater");
            editor.VersionBox.Text!.ShouldBe("1.2.0.1");
            editor.SectionList.Items.Cast<EditorSection>().Select(s => s.Title).ShouldBe([
                "General", "Changelog", "Files", "Operations", "Availability", "Rollout"
            ]);
            editor.SummaryText.Text!.ShouldBe("0 files · 0 operations · English changelog required");
            editor.PublishBox.IsChecked.ShouldBe(true);
            editor.IncludeInStatisticsBox.IsEffectivelyEnabled.ShouldBeFalse();
        });
        await When("the user saves with an invalid version", () =>
        {
            User.Type(editor.VersionBox, "not-a-version");
            User.Click(editor.SaveButton);
        });
        await Then("the editor explains the version formats",
            () => editor.ErrorText.Text!.ShouldContain("2.1.0-beta.1"));
        await When("the user writes the version the way nUpdate 4 did", () =>
        {
            User.Type(editor.VersionBox, "1.2.0.0");
            User.Click(editor.SaveButton);
        });
        await Then("the editor asks for the canonical form",
            () => editor.ErrorText.Text!.ShouldContain("a fourth number only when it is not 0"));
        await When("the user enters the version of the existing package", () =>
        {
            User.Type(editor.VersionBox, "1.2.0");
            User.Click(editor.SaveButton);
        });
        await Then("the editor refuses the duplicate",
            () => editor.ErrorText.Text!.ShouldBe("The project already has a package 1.2.0."));
        await When("the version is unique but the changelog is empty", () =>
        {
            User.Type(editor.VersionBox, "1.3.0");
            User.Click(editor.SaveButton);
        });
        await Then("the editor asks for the English changelog",
            () => editor.ErrorText.Text!.ShouldBe("Enter the English changelog."));
        await When("the changelog is filled in but nothing is in the package", () =>
        {
            GoTo(editor, "Changelog");
            User.Type(EnglishChangelog(editor), "Fixes.");
            User.Click(editor.SaveButton);
        });
        await Then("the editor asks for a file or an operation", () =>
        {
            editor.ErrorText.Text!.ShouldBe("Add at least one file or operation.");
            editor.IsVisible.ShouldBeTrue();
        });
    });

    [AvaloniaFact]
    public Task Creates_a_local_package_with_files() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        await Given("the editor for the first package", async () => editor = await OpenNewPackageAsync());
        await When("the user fills in the general data without publishing", () =>
        {
            User.Type(editor.VersionBox, "1.0.0");
            User.Type(editor.DescriptionBox, "First release");
            User.Check(editor.PublishBox, false);
            GoTo(editor, "Changelog");
            User.Type(EnglishChangelog(editor), "Everything is new.");
        });
        await And("adds two files into a plugins folder and removes one again", () =>
        {
            GoTo(editor, "Files");
            User.Type(editor.TargetFolderBox, "plugins");
            App.Picker.UserPicksFiles(Context.WriteFile("src/Importer.dll", "importer"),
                Context.WriteFile("src/Exporter.dll", "exporter"));
            User.Click(editor.AddFilesButton);
            editor.FileList.Items.Cast<PackageFileItem>().Select(f => f.Display)
                .ShouldBe(["Program/plugins/Importer.dll", "Program/plugins/Exporter.dll"]);
            User.Click(User.ButtonFor(editor.FileList, editor.FileList.Items[1]!, "Remove"));
            editor.FileList.Items.Cast<PackageFileItem>().Select(f => f.Display)
                .ShouldBe(["Program/plugins/Importer.dll"]);
            editor.SummaryText.Text!.ShouldStartWith("1 file · 0 operations");
        });
        await And("saves the package", async () =>
        {
            User.Click(editor.SaveButton);
            await App.ClosedAsync(editor);
        });
        await Then("the package is listed as local only", () => Rows().ShouldBe([("1.0.0", "Local only")]));
        await And("it exists in the project folder but not on the server", async () =>
        {
            var version = new UpdateVersion("1.0.0");
            File.Exists(_project.Project.PackageFilePath(version, "any")).ShouldBeTrue();
            _project.Project.PackageFilePath(version, "any").ShouldStartWith(App.ProjectFolder("Trade Updater"));
            var entry = (await App.Feeds.LoadEntryAsync(_project.Project, version))!;
            entry.Changelog["en"].ShouldBe("Everything is new.");
            (await Context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        });
    });

    [AvaloniaFact]
    public Task Creates_and_publishes_a_pre_release_with_operations() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        await Given("the editor for a new package with a semantic pre-release version", async () =>
        {
            editor = await OpenNewPackageAsync();
            User.Type(editor.VersionBox, "2.0.0-beta.1");
            User.Check(editor.NecessaryBox, true);
            GoTo(editor, "Changelog");
            User.Type(EnglishChangelog(editor), "Beta of the importer.");
        });
        await When("the user adds a file-deletion and a process-termination operation", () =>
        {
            GoTo(editor, "Operations");
            var viewModel = ViewModel(editor);
            User.Click(User.ButtonFor(editor, OperationKind.FromType(DeleteFilesOperation.TypeName), "Delete files"));
            User.Click(User.ButtonFor(editor, OperationKind.FromType(TerminateProcessOperation.TypeName), "Terminate a process"));
            viewModel.Operations.Select(o => o.Kind.Type).ShouldBe(["deleteFiles", "terminateProcess"]);
            viewModel.SelectedOperation!.Kind.Type.ShouldBe("terminateProcess");
            User.Type(User.Find<TextBox>(editor, "ValueBox"), "TradeUpdater");

            User.SelectRow(editor.OperationList, 0);
            User.Type(User.Find<TextBox>(editor, "ValueBox"), "%program%");
            User.Type(User.Find<TextBox>(editor, "ListBox"), "old.dll");
            User.Check(User.Find<ToggleSwitch>(editor, "BeforeReplacingBox"), true);
        });
        await And("moves the termination first", () =>
        {
            var terminate = ViewModel(editor).Operations[1];
            User.Click(User.ButtonFor(editor, terminate, "Move up"));
            ViewModel(editor).Operations.Select(o => o.Kind.Type).ShouldBe(["terminateProcess", "deleteFiles"]);
            editor.SummaryText.Text!.ShouldStartWith("0 files · 2 operations");
        });
        await And("creates the package", async () =>
        {
            User.Click(editor.SaveButton);
            await App.ClosedAsync(editor);
        });
        await Then("the package is listed as released under the version as typed",
            () => Rows().ShouldBe([("2.0.0-beta.1", "Released")]));
        await And("the package carries the operations in that order", async () =>
        {
            var entry = (await RemoteAsync()).Single();
            entry.Version.ShouldBe(new UpdateVersion("2.0.0-beta.1"));
            entry.Necessary.ShouldBeTrue();
            entry.Files.Single().Touches.ShouldBe([OperationArea.Files, OperationArea.Processes]);
            var operations = await PackagedOperationsAsync(entry.Version);
            operations.Select(o => o.Type).ShouldBe(["terminateProcess", "deleteFiles"]);
            operations[0].ShouldBeOfType<TerminateProcessOperation>().ProcessName.ShouldBe("TradeUpdater");
            var delete = operations[1].ShouldBeOfType<DeleteFilesOperation>();
            delete.Files.ShouldBe(["old.dll"]);
            delete.RunBeforeFileReplacement.ShouldBeTrue();
        });
    });

    [AvaloniaFact]
    public Task Builds_one_package_file_per_platform() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        await Given("a new package with a file for any platform", async () =>
        {
            editor = await OpenNewPackageAsync();
            User.Type(editor.VersionBox, "1.0.0");
            GoTo(editor, "Changelog");
            User.Type(EnglishChangelog(editor), "Builds for Windows and everything else.");
            GoTo(editor, "Files");
            editor.FilesPlatformBox.IsVisible.ShouldBeFalse();
            App.Picker.UserPicksFiles(Context.WriteFile("portable/readme.txt", "any"));
            User.Click(editor.AddFilesButton);
        });
        await When("the user adds a Windows x64 platform with its own file and a service operation", () =>
        {
            GoTo(editor, "General");
            User.Select(editor.NewPlatformBox,
                ViewModel(editor).AvailablePlatforms.Single(p => p.Platform == "win-x64"));
            User.Click(editor.AddPlatformButton);
            editor.PlatformList.ItemCount.ShouldBe(2);
            GoTo(editor, "Files");
            editor.FilesPlatformBox.IsVisible.ShouldBeTrue();
            editor.FileList.ItemCount.ShouldBe(0);
            App.Picker.UserPicksFiles(Context.WriteFile("windows/TradeUpdater.exe", "windows"));
            User.Click(editor.AddFilesButton);
            GoTo(editor, "Operations");
            User.Click(User.ButtonFor(editor, OperationKind.FromType(StopServiceOperation.TypeName), "Stop a service"));
            User.Type(User.Find<TextBox>(editor, "ValueBox"), "TradeService");
            editor.SummaryText.Text!.ShouldBe("2 platforms · 2 files · 1 operation · English changelog required");
        });
        await And("checks that the package for any platform offers no service operations", () =>
        {
            User.Select(editor.OperationsPlatformBox, ViewModel(editor).Platforms[0]);
            ViewModel(editor).OperationPalette.ShouldNotContain(k => k.RequiresWindows);
            editor.OperationList.ItemCount.ShouldBe(0);
        });
        await And("creates the package", async () =>
        {
            User.Click(editor.SaveButton);
            await App.ClosedAsync(editor);
        });
        await Then("the feed lists a file per platform and the Windows one stops the service", async () =>
        {
            var entry = (await RemoteAsync()).Single();
            entry.Files.Select(f => (f.Platform, f.Path)).ShouldBe([
                ("any", "packages/1.0.0/any.zip"), ("win-x64", "packages/1.0.0/win-x64.zip")
            ]);
            entry.Files[0].Touches
                .ShouldBeEmpty(); // touches name what operations change; copying files is what every package does
            entry.Files[1].Touches.ShouldBe([OperationArea.Services]);
            (await PackagedOperationsAsync(entry.Version, "win-x64")).Single().ShouldBeOfType<StopServiceOperation>()
                .ServiceName.ShouldBe("TradeService");
            (await Context.StatusAsync("packages/1.0.0/win-x64.zip")).ShouldBe(System.Net.HttpStatusCode.OK);
            (await Context.StatusAsync("packages/1.0.0/any.zip")).ShouldBe(System.Net.HttpStatusCode.OK);
        });
    });

    [AvaloniaFact]
    public Task Saves_availability_and_rollout_conditions() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        await Given("a new package with a file", async () =>
        {
            editor = await OpenNewPackageAsync();
            User.Type(editor.VersionBox, "3.0.0");
            GoTo(editor, "Changelog");
            User.Type(EnglishChangelog(editor), "Regional rollout.");
            GoTo(editor, "Files");
            App.Picker.UserPicksFiles(Context.WriteFile("src/app.exe", "3.0"));
            User.Click(editor.AddFilesButton);
        });
        await When("the user excludes an old version and adds a rollout condition", () =>
        {
            GoTo(editor, "Availability");
            editor.UnsupportedVersionsBox.IsEffectivelyEnabled.ShouldBeFalse();
            User.Check(editor.RestrictVersionsBox, true);
            User.Type(editor.UnsupportedVersionsBox, "1.0.0");
            GoTo(editor, "Rollout");
            User.Click(editor.AddConditionButton);
            var condition = ViewModel(editor).Conditions.Single();
            var boxes = editor.GetVisualDescendants().OfType<TextBox>()
                .Where(t => ReferenceEquals(t.DataContext, condition)).ToList();
            User.Type(boxes[0], "Region");
            User.Type(boxes[1], "EU");
            User.Select(editor.ConditionModeBox, RolloutConditionMode.All);
        });
        await And("publishes the package", async () =>
        {
            User.Click(editor.SaveButton);
            await App.ClosedAsync(editor);
        });
        await Then("the configuration on the server carries both", async () =>
        {
            var entry = (await RemoteAsync()).Single();
            entry.UnsupportedVersions.ShouldBe([new UpdateVersion("1.0.0")]);
            entry.Rollout.Mode.ShouldBe(RolloutConditionMode.All);
            var condition = entry.Rollout.Conditions.Single();
            (condition.Key, condition.Value, condition.Negated).ShouldBe(("Region", "EU", false));
        });
    });

    [AvaloniaFact]
    public Task Edits_a_published_package() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        await Given("a project with a published package", async () =>
        {
            _project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(_project, "1.0.0", "First release", publish: true);
            _projectWindow = await App.OpenListedProjectAsync("Trade Updater");
        });
        await When("the user edits it", async () =>
        {
            User.SelectRow(_projectWindow.PackageGrid, 0);
            User.Click(_projectWindow.EditPackageButton);
            editor = await App.WindowAsync<PackageEditorWindow>();
        });
        await Then("the editor shows the package with a fixed version", () =>
        {
            editor.Title.ShouldBe("Edit package 1.0.0 of Trade Updater");
            editor.VersionBox.IsEffectivelyEnabled.ShouldBeFalse();
            editor.DescriptionBox.Text!.ShouldBe("First release");
            editor.PublishBox.IsEffectivelyVisible.ShouldBeFalse();
            editor.SummaryText.Text!.ShouldBe("Files and operations unchanged");
            editor.SaveButton.GetVisualDescendants().OfType<TextBlock>().ShouldContain(t => t.Text == "Save and publish");
        });
        await When("the user marks it as necessary, changes the description and the changelog, and saves", async () =>
        {
            User.Type(editor.DescriptionBox, "First release (required)");
            User.Check(editor.NecessaryBox, true);
            GoTo(editor, "Changelog");
            User.Type(EnglishChangelog(editor), "Changes in 1.0.0, now required.");
            User.Click(editor.SaveButton);
            await App.ClosedAsync(editor);
        });
        await Then("the list and the server reflect the change", async () =>
        {
            _projectWindow.PackageGrid.ItemsSource!.Cast<PackageItemViewModel>().Single().Description
                .ShouldBe("First release (required)");
            var entry = (await RemoteAsync()).Single();
            entry.Necessary.ShouldBeTrue();
            entry.Changelog["en"].ShouldBe("Changes in 1.0.0, now required.");
        });
    });

    [AvaloniaFact]
    public Task Changes_the_files_of_a_published_package() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        string oldPath = null!;
        await Given("a project with a published package", async () =>
        {
            _project = await App.ExistingProjectAsync("Trade Updater");
            await App.ExistingPackageAsync(_project, "1.0.0", "First release", publish: true);
            oldPath = (await RemoteAsync()).Single().Files.Single().Path;
            _projectWindow = await App.OpenListedProjectAsync("Trade Updater");
        });
        await When("the user edits it and looks at its files", async () =>
        {
            User.SelectRow(_projectWindow.PackageGrid, 0);
            User.Click(_projectWindow.EditPackageButton);
            editor = await App.WindowAsync<PackageEditorWindow>();
            GoTo(editor, "Files");
        });
        await Then("the editor lists the files of the published package", () =>
            editor.FileList.Items.Cast<PackageFileItem>().Select(f => (f.Display, f.Change))
                .ShouldBe([("Program/app.exe", FileChange.Unchanged)]));
        await When("the user replaces the program, adds a file and saves", async () =>
        {
            App.Picker.UserPicksFiles(Context.WriteFile("1.0.0-fix/app.exe", "fixed"),
                Context.WriteFile("1.0.0-fix/readme.txt", "read me"));
            User.Click(editor.AddFilesButton);
            editor.FileList.Items.Cast<PackageFileItem>().Select(f => (f.Display, f.Change))
                .ShouldBe([("Program/app.exe", FileChange.Changed), ("Program/readme.txt", FileChange.Added)]);
            editor.SummaryText.Text!.ShouldBe("Any platform: 1 file added, 1 file changed");
            User.Click(editor.SaveButton);
            await App.ClosedAsync(editor);
        });
        await Then("the server serves the new package file under a new name and no longer the old one", async () =>
        {
            var file = (await RemoteAsync()).Single().Files.Single();
            file.Path.ShouldBe("packages/1.0.0/any-r2.zip");
            (await Context.StatusAsync(file.Path)).ShouldBe(System.Net.HttpStatusCode.OK);
            (await Context.StatusAsync(oldPath)).ShouldBe(System.Net.HttpStatusCode.NotFound);
            ((ProjectViewModel)_projectWindow.DataContext!).History.First().Kind.ShouldBe("Rebuild");
        });
    });

    [AvaloniaFact]
    public Task Cancelling_discards_the_new_package() => Scenario(async () =>
    {
        PackageEditorWindow editor = null!;
        await Given("a filled-in editor", async () =>
        {
            editor = await OpenNewPackageAsync();
            User.Type(editor.VersionBox, "1.0.0");
            GoTo(editor, "Files");
            App.Picker.UserPicksFiles(Context.WriteFile("src/app.exe", "1.0"));
            User.Click(editor.AddFilesButton);
        });
        await When("the user cancels", async () =>
        {
            User.Click(editor.CancelButton);
            await App.ClosedAsync(editor);
        });
        await Then("no package exists", () =>
        {
            Rows().ShouldBeEmpty();
            Directory.Exists(_project.Project.PackageDirectory(new UpdateVersion("1.0.0"))).ShouldBeFalse();
        });
    });
}
