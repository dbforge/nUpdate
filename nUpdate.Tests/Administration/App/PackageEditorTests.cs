using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Services;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.App;

/// <summary>The package editor: versions, changelogs, files, conditions, operations, sections and saving in create or edit mode.</summary>
public class PackageEditorTests
{
    private readonly AppTestContext _context = new();

    private PackageEditorViewModel Editor(UpdateProject? project = null, PackageInfo? entry = null,
        PackageDefinition? content = null)
    {
        project ??= AppTestContext.NewProject();
        return entry is null
            ? _context.Factory.Create<PackageEditorViewModel>(project, AppTestContext.NewSecrets())
            : _context.Factory.Create<PackageEditorViewModel>(project, AppTestContext.NewSecrets(),
                new ExistingPackage(entry, content ?? ContentOf(entry)));
    }

    /// <summary>The content of an existing package: its platforms without files or operations.</summary>
    private static PackageDefinition ContentOf(PackageInfo entry)
    {
        var content = new PackageDefinition(entry.Version);
        foreach (var file in entry.Files)
            content.GetOrAddPlatform(file.Platform);
        return content;
    }

    [Fact]
    public void PackageEditor_CreateMode_DefaultsAndValidation()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.2.0") });
        var editor = Editor(project);
        editor.IsCreateMode.ShouldBeTrue();
        editor.Title.ShouldBe("New package for Demo");
        editor.Version.ShouldBe("1.2.0.1"); // the highest existing package plus one revision
        editor.Changelogs.Single().Display.ShouldBe("English (required)");
        editor.StatisticsAvailable.ShouldBeFalse();
        editor.Platforms.Single().Platform.ShouldBe("any");
        editor.SelectedPlatform.ShouldBe(editor.Platforms[0]);
        editor.AvailablePlatforms.Count.ShouldBe(10);
        editor.HasSeveralPlatforms.ShouldBeFalse();
        editor.RolloutConditionModes.Count.ShouldBe(2);
        editor.Roots.Count.ShouldBe(4);
        editor.OperationKinds.Count.ShouldBe(10);

        editor.Version = "x";
        editor.Validate()!.ShouldContain("valid version");
        editor.Version = "0.0.0-beta.1";
        editor.Validate()!.ShouldContain("reserved");
        editor.Version = "1.2.0.0"; // the spelling of nUpdate 4 is no longer a version
        editor.Validate()!.ShouldContain("major.minor.patch");
        editor.Version = "1.2.0";
        editor.Validate()!.ShouldContain("already has");
        editor.Version = "1.3.0";
        editor.Validate().ShouldBe("Enter the English changelog.");
        editor.Changelogs[0].Text = "Changes";
        editor.Validate().ShouldBe("Add at least one file or operation.");
        editor.AddFile(PackageRoot.Program, "\\a.dll", "/src/a.dll");
        editor.RestrictVersions = true;
        editor.UnsupportedVersionsText = "1.0.0, nope";
        editor.Validate()!.ShouldContain("unsupported version");
        editor.UnsupportedVersionsText = "1.0.0; 1.1.0";
        editor.AddConditionCommand.Execute(null);
        editor.Validate()!.ShouldContain("rollout condition");
        editor.Conditions[0].Key = "R";
        editor.Conditions[0].Value = "east";
        editor.Validate().ShouldBeNull();

        editor.SelectedOperationKind = OperationKind.FromType(RenameFileOperation.TypeName);
        editor.AddOperationCommand.Execute(null);
        editor.SelectedOperation.ShouldNotBeNull();
        editor.Validate()!.ShouldContain("Rename a file");
        editor.RemoveOperationCommand.Execute(editor.SelectedOperation);
        editor.SelectedOperation.ShouldBeNull();
        editor.RemoveOperationCommand.Execute(null);
        editor.Validate().ShouldBeNull();

        var request = editor.BuildRequest();
        request.Package.Version.ToString().ShouldBe("1.3.0");
        request.Package.Platforms.Single().Platform.ShouldBe("any");
        request.Package.Platforms.Single().Files.Single().EntryName.ShouldBe("Program/a.dll");
        request.UnsupportedVersions.Select(v => v.ToString()).ShouldBe(["1.0.0", "1.1.0"]);
        request.RolloutConditions.Single().Key.ShouldBe("R");
        request.Changelog[new CultureInfo("en")].ShouldBe("Changes");
        request.Necessary.ShouldBeFalse();
        editor.RestrictVersions = false;
        editor.BuildRequest().UnsupportedVersions.ShouldBeEmpty();
    }

    [Fact]
    public void PackageEditor_SuggestsTheNextVersion()
    {
        var project = AppTestContext.NewProject();
        PackageEditorViewModel.SuggestVersion(project).ShouldBe(new UpdateVersion("1.0.0"));
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.2.0") });
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.1.9.9") });
        PackageEditorViewModel.SuggestVersion(project).ShouldBe(new UpdateVersion("1.2.0.1"));

        // The configured executable wins when its version is newer than every package.
        project.AssemblyVersionPath = typeof(PackageEditorTests).Assembly.Location;
        var assemblyVersion = typeof(PackageEditorTests).Assembly.GetName().Version!;
        PackageEditorViewModel.SuggestVersion(project).ShouldBe(new UpdateVersion(assemblyVersion.Major,
            assemblyVersion.Minor, assemblyVersion.Build, assemblyVersion.Revision));
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("99.0.0") });
        PackageEditorViewModel.SuggestVersion(project).ShouldBe(new UpdateVersion("99.0.0.1"));
        Should.Throw<ArgumentNullException>(() => PackageEditorViewModel.SuggestVersion(null!));
    }

    [Fact]
    public void PackageEditor_FilesCulturesAndConditions()
    {
        var editor = Editor();
        editor.AddFile(PackageRoot.Program, "a.dll", "/1");
        editor.AddFile(PackageRoot.Program, "A.DLL", "/2");
        editor.Files.Single().SourcePath.ShouldBe("/2");
        editor.Files.Single().Display.ShouldBe("Program/A.DLL");
        editor.RemoveFileCommand.Execute(editor.Files.Single());
        editor.RemoveFileCommand.Execute(null);
        editor.Files.ShouldBeEmpty();
        Should.Throw<ArgumentNullException>(() => editor.AddFile(PackageRoot.Program, null!, "/x"));
        Should.Throw<ArgumentNullException>(() => editor.AddFile(PackageRoot.Program, "x", null!));

        editor.NewCultureName = " de-DE ";
        editor.AddCultureCommand.Execute(null);
        editor.Changelogs.Count.ShouldBe(2);
        editor.Changelogs[1].Display.ShouldContain("de-DE");
        editor.NewCultureName.ShouldBe("");
        editor.NewCultureName = "de-DE";
        editor.AddCultureCommand.Execute(null);
        editor.Changelogs.Count.ShouldBe(2);
        editor.NewCultureName = "";
        editor.AddCultureCommand.Execute(null);
        editor.NewCultureName = "not a culture!!";
        editor.AddCultureCommand.Execute(null);
        editor.ErrorMessage!.ShouldContain("not a culture name");
        editor.RemoveCultureCommand.Execute(editor.Changelogs[0]);
        editor.Changelogs.Count.ShouldBe(2);
        editor.RemoveCultureCommand.Execute(editor.Changelogs[1]);
        editor.RemoveCultureCommand.Execute(null);
        editor.Changelogs.Count.ShouldBe(1);

        editor.AddConditionCommand.Execute(null);
        editor.RemoveConditionCommand.Execute(editor.Conditions[0]);
        editor.RemoveConditionCommand.Execute(null);
        editor.Conditions.ShouldBeEmpty();
    }

    [Fact]
    public async Task PackageEditor_PicksFilesAndFolders()
    {
        var editor = Editor();
        _context.Files.PickFilesAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>())
            .Returns(["/src/a.dll", "/src/b.dll"]);
        editor.SelectedRoot = PackageRoot.AppData;
        await editor.AddFilesCommand.ExecuteAsync(null);
        editor.Files.Select(f => f.Display).ShouldBe(["AppData/a.dll", "AppData/b.dll"]);

        _context.FileSystem.AddFile("/folder/sub/x.txt", new MockFileData("x"));
        _context.FileSystem.AddFile("/folder/y.txt", new MockFileData("y"));
        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns("/folder/");
        editor.SelectedRoot = PackageRoot.Program;
        await editor.AddFolderCommand.ExecuteAsync(null);
        editor.Files.Select(f => f.Display).OrderBy(x => x, StringComparer.Ordinal).ShouldBe([
            "AppData/a.dll", "AppData/b.dll", "Program/folder/sub/x.txt", "Program/folder/y.txt"
        ]);
        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns((string?)null);
        await editor.AddFolderCommand.ExecuteAsync(null);
        editor.Files.Count.ShouldBe(4);
    }

    [Fact]
    public async Task PackageEditor_PlacesPickedFilesInTheTargetFolderAndReportsFolderErrors()
    {
        var editor =
            _context.Factory.Create<PackageEditorViewModel>(AppTestContext.NewProject(), AppTestContext.NewSecrets());
        _context.Files.PickFilesAsync(Arg.Any<string>(), Arg.Any<nUpdate.Administration.Services.FileTypeFilter[]>())
            .Returns(["/src/a.dll"]);
        editor.TargetFolder = " /plugins\\ ";
        await editor.AddFilesCommand.ExecuteAsync(null);
        editor.Files.Single().Display.ShouldBe("Program/plugins/a.dll");

        _context.FileSystem.AddFile("/data/x.txt", new MockFileData("x"));
        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns("/data");
        await editor.AddFolderCommand.ExecuteAsync(null);
        editor.Files.Select(f => f.Display).ShouldContain("Program/plugins/data/x.txt");

        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns("/missing");
        await editor.AddFolderCommand.ExecuteAsync(null);
        editor.ErrorMessage.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task PackageEditor_AddFolderWithTrailingSeparator()
    {
        var editor =
            _context.Factory.Create<PackageEditorViewModel>(AppTestContext.NewProject(), AppTestContext.NewSecrets());
        _context.FileSystem.AddFile("/data/a.txt", new MockFileData("a"));
        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns("/data");
        await editor.AddFolderCommand.ExecuteAsync(null);
        editor.Files.Single().Display.ShouldBe("Program/data/a.txt");
        editor.Files.Single().Root.ShouldBe(PackageRoot.Program);
    }

    [Fact]
    public void PackageEditor_SectionsPaletteAndSummary()
    {
        var project = AppTestContext.NewProject();
        var editor = _context.Factory.Create<PackageEditorViewModel>(project, AppTestContext.NewSecrets());
        editor.Sections.Select(s => s.Key)
            .ShouldBe(["general", "changelog", "files", "operations", "availability", "conditions"]);
        editor.Sections.Select(s => s.Title)
            .ShouldBe(["General", "Changelog", "Files", "Operations", "Availability", "Rollout"]);
        editor.Sections.Select(s => s.Hint).ShouldBe(["", "English", "", "", "", "Everyone"]);
        editor.SelectedSection.Key.ShouldBe("general");
        editor.IsGeneralSection.ShouldBeTrue();
        editor.SaveText.ShouldBe("Create package");
        editor.Summary.ShouldBe("0 files · 0 operations · English changelog required");

        // Registry and services exist only on Windows, so a package for any platform does not offer them.
        editor.OperationPalette.Select(k => k.AreaTitle).Distinct().ShouldBe(["Files", "Processes"]);
        editor.OperationPalette.Count.ShouldBe(OperationKind.All.Count(k => !k.RequiresWindows));

        editor.SelectedSection = editor.Sections[3];
        editor.IsOperationsSection.ShouldBeTrue();
        editor.IsGeneralSection.ShouldBeFalse();
        editor.SelectedSection = editor.Sections[1];
        editor.IsChangelogSection.ShouldBeTrue();
        editor.SelectedSection = editor.Sections[2];
        editor.IsFilesSection.ShouldBeTrue();
        editor.SelectedSection = editor.Sections[4];
        editor.IsAvailabilitySection.ShouldBeTrue();
        editor.SelectedSection = editor.Sections[5];
        editor.IsConditionsSection.ShouldBeTrue();

        editor.AddFile(PackageRoot.Program, "a.dll", "/a.dll");
        editor.Summary.ShouldStartWith("1 file · 0 operations");
        editor.AddOperationOfKindCommand.Execute(null);
        editor.Operations.ShouldBeEmpty();
        var delete = OperationKind.FromType(DeleteFilesOperation.TypeName);
        var rename = OperationKind.FromType(RenameFileOperation.TypeName);
        var script = OperationKind.FromType(TerminateProcessOperation.TypeName);
        editor.AddOperationOfKindCommand.Execute(delete);
        editor.AddOperationOfKindCommand.Execute(rename);
        editor.AddOperationOfKindCommand.Execute(script);
        editor.Summary.ShouldStartWith("1 file · 3 operations");
        editor.SelectedOperation!.Kind.ShouldBe(script);
        editor.Operations.Select(o => o.Kind).ShouldBe([delete, rename, script]);

        editor.MoveOperationUpCommand.Execute(editor.Operations[0]);
        editor.Operations.Select(o => o.Kind).ShouldBe([delete, rename, script]);
        editor.MoveOperationDownCommand.Execute(editor.Operations[2]);
        editor.Operations.Select(o => o.Kind).ShouldBe([delete, rename, script]);
        editor.MoveOperationUpCommand.Execute(editor.Operations[2]);
        editor.Operations.Select(o => o.Kind).ShouldBe([delete, script, rename]);
        editor.SelectedOperation!.Kind.ShouldBe(script);
        editor.MoveOperationDownCommand.Execute(editor.Operations[0]);
        editor.Operations.Select(o => o.Kind).ShouldBe([script, delete, rename]);
        editor.MoveOperationUpCommand.Execute(null);
        editor.MoveOperationDownCommand.Execute(null);
        var foreign = new OperationEditorViewModel(delete);
        editor.MoveOperationUpCommand.Execute(foreign);
        editor.Operations.Select(o => o.Kind).ShouldBe([script, delete, rename]);

        editor.Sections.Select(s => s.Hint).ShouldBe(["", "English", "1 file", "3", "", "Everyone"]);
        editor.RestrictVersions = true;
        editor.AddConditionCommand.Execute(null);
        editor.Sections.Select(s => s.Hint).ShouldBe(["", "English", "1 file", "3", "Restricted", "1 condition"]);
        editor.AddConditionCommand.Execute(null);
        editor.NewCultureName = "de";
        editor.AddCultureCommand.Execute(null);
        editor.Sections.Select(s => s.Hint).ShouldBe(["", "2 languages", "1 file", "3", "Restricted", "2 conditions"]);

        var existing = Editor(project, new PackageInfo { Version = new UpdateVersion("1.0.0") });
        existing.Sections.Count.ShouldBe(6);
        existing.State.ShouldBe("Local only");
        existing.SaveText.ShouldBe("Save package");
        existing.Summary.ShouldBe("Files and operations unchanged");
        editor.State.ShouldBe("New");
    }

    [Fact]
    public async Task PackageEditor_SaveCreatesOrUpdates()
    {
        var project = AppTestContext.NewProject(statistics: true);
        var editor = Editor(project);
        var closed = new List<bool>();
        editor.CloseRequested += (_, a) => closed.Add(a);
        await editor.SaveCommand.ExecuteAsync(null);
        editor.ErrorMessage.ShouldNotBeNull();

        editor.Changelogs[0].Text = "c";
        editor.AddFile(PackageRoot.Program, "a", "/a");
        var created = new UpdatePackage { Version = new UpdateVersion("1.0.0") };
        _context.Publisher.CreatePackageAsync(Arg.Any<PublishRequest>(), Arg.Any<IProgress<PipelineProgress>>(),
            Arg.Any<CancellationToken>()).Returns(created);
        await editor.SaveCommand.ExecuteAsync(null);
        editor.CreatedPackage.ShouldBe(created);
        closed.ShouldBe([true]);

        _context.Publisher
            .CreatePackageAsync(Arg.Any<PublishRequest>(), Arg.Any<IProgress<PipelineProgress>>(),
                Arg.Any<CancellationToken>())
            .ThrowsAsync(new PipelineException("Building the package", new IOException("nope"), []));
        await editor.SaveCommand.ExecuteAsync(null);
        editor.ErrorMessage!.ShouldContain("nope");

        project.Packages.Add(new UpdatePackage
        { Version = new UpdateVersion("2.0.0"), Description = "old", Released = true });
        var entry = new PackageInfo
        {
            Version = new UpdateVersion("2.0.0"),
            Changelog = { ["en"] = "Two", ["de-DE"] = "Zwei" },
            UnsupportedVersions = [new UpdateVersion("1.0.0")],
            Rollout = new RolloutSettings
            { Mode = RolloutConditionMode.All, Conditions = [new RolloutCondition("R", "east", true)] },
            Statistics = new PackageStatistics { Url = "nupdate-statistics.php", Enabled = true },
            Necessary = true,
            Files = [new PackageFile { Platform = "win-x86" }, new PackageFile { Platform = "linux-arm64" }],
        };
        var editing = Editor(project, entry);
        editing.IsEditMode.ShouldBeTrue();
        editing.Title.ShouldBe("Edit package 2.0.0 of Demo");
        editing.IsReleased.ShouldBeTrue();
        editing.State.ShouldBe("Released");
        editing.SaveText.ShouldBe("Save and publish");
        editing.RebuildNotice.ShouldStartWith("2.0.0 is published.");
        editing.Description.ShouldBe("old");
        editing.Changelogs.Count.ShouldBe(2);
        editing.Changelogs[1].Text.ShouldBe("Zwei");
        editing.RestrictVersions.ShouldBeTrue();
        editing.Conditions.Single().IsNegative.ShouldBeTrue();
        editing.RolloutConditionMode.ShouldBe(RolloutConditionMode.All);
        editing.IncludeInStatistics.ShouldBeTrue();
        editing.Platforms.Select(p => p.Display).ShouldBe(["Windows x86 (win-x86)", "Linux ARM64 (linux-arm64)"]);
        editing.SelectedPlatform.Platform.ShouldBe("win-x86");
        editing.Validate().ShouldBeNull();

        editing.Description = " new ";
        editing.Necessary = false;
        editing.Changelogs[1].Text = " ";
        var closedEdit = new List<bool>();
        editing.CloseRequested += (_, a) => closedEdit.Add(a);
        await editing.SaveCommand.ExecuteAsync(null);
        closedEdit.ShouldBe([true]);
        await _context.Publisher.Received().UpdateEntryAsync(project, Arg.Any<ProjectSecrets>(),
            Arg.Is<PackageInfo>(c =>
                !c.Necessary && c.Changelog.Count == 1 && c.UnsupportedVersions.Count == 1 &&
                c.Rollout.Conditions.Count == 1 && c.Statistics!.Enabled &&
                c.Statistics.Url == "nupdate-statistics.php"), Arg.Any<IProgress<PipelineProgress>>(),
            Arg.Any<CancellationToken>());
        project.Packages.Single().Description.ShouldBe("new");

        Should.Throw<InvalidOperationException>(() => editor.BuildEditedEntry());
        var entryWithoutPackage = new PackageInfo
        { Version = new UpdateVersion("3.0.0"), Changelog = { ["en"] = "x" } };
        var withoutPackage = Editor(AppTestContext.NewProject(), entryWithoutPackage);
        withoutPackage.IncludeInStatistics.ShouldBeTrue();
        var edited = withoutPackage.BuildEditedEntry();
        edited.UnsupportedVersions.ShouldBeEmpty();
        edited.Statistics.ShouldBeNull();
    }

    [AvaloniaFact]
    public void PackageEditorWindow_BindsFieldsAndOperationEditor()
    {
        var project = AppTestContext.NewProject();
        var viewModel = _context.Factory.Create<PackageEditorViewModel>(project, AppTestContext.NewSecrets());
        var window = new PackageEditorWindow { DataContext = viewModel };
        window.Show();
        window.VersionBox.Text = "2.0.0.0";
        viewModel.Version.ShouldBe("2.0.0.0");
        window.DescriptionBox.Text = "desc";
        viewModel.Description.ShouldBe("desc");
        viewModel.AddFile(PackageRoot.Program, "a.dll", "/a.dll");
        viewModel.AddConditionCommand.Execute(null);
        viewModel.SelectedOperationKind = OperationKind.FromType(SetRegistryValuesOperation.TypeName);
        viewModel.AddOperationCommand.Execute(null);
        viewModel.Operations[0].AddRegistryValueCommand.Execute(null);
        viewModel.SelectedOperationKind = OperationKind.FromType(StartProcessOperation.TypeName);
        viewModel.AddOperationCommand.Execute(null);
        foreach (var section in viewModel.Sections)
        {
            viewModel.SelectedSection = section;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
        }

        window.SectionList.ItemCount.ShouldBe(6);

        window.OperationList.ItemCount.ShouldBe(2);
        window.FileList.ItemCount.ShouldBe(1);
        window.PlatformList.ItemCount.ShouldBe(1);
        window.FilesPlatformBox.IsVisible.ShouldBeFalse();
        viewModel.NewPlatform = new PlatformChoice("osx-arm64");
        viewModel.AddPlatformCommand.Execute(null);
        Dispatcher.UIThread.RunJobs();
        window.PlatformList.ItemCount.ShouldBe(2);
        window.FilesPlatformBox.IsVisible.ShouldBeTrue();
        window.OperationsPlatformBox.SelectedItem.ShouldBe(viewModel.SelectedPlatform);
        window.FileList.ItemCount.ShouldBe(0);
        viewModel.SelectedPlatform = viewModel.Platforms[0];
        Dispatcher.UIThread.RunJobs();
        window.FileList.ItemCount.ShouldBe(1);
        window.Close();
    }

    [AvaloniaFact]
    public void PackageEditorWindow_KeepsAPlatformSelectedWhenTheSelectedOneIsRemoved()
    {
        var viewModel =
            _context.Factory.Create<PackageEditorViewModel>(AppTestContext.NewProject(), AppTestContext.NewSecrets());
        var window = new PackageEditorWindow { DataContext = viewModel };
        window.Show();
        viewModel.NewPlatform = new PlatformChoice("linux-x64");
        viewModel.AddPlatformCommand.Execute(null);
        viewModel.SelectedSection = viewModel.Sections.Single(s => s.Key == "files");
        Dispatcher.UIThread.RunJobs();
        window.FilesPlatformBox.SelectedItem.ShouldBe(viewModel.Platforms[1]);

        viewModel.RemovePlatformCommand.Execute(viewModel.Platforms[1]); // the selected one, bound to both switchers
        Dispatcher.UIThread.RunJobs();

        viewModel.SelectedPlatform.Platform.ShouldBe("any");
        viewModel.AddFile(PackageRoot.Program, "a.dll", "/a.dll");
        viewModel.Files.Single().RelativePath.ShouldBe("a.dll");
        viewModel.SelectedPlatform = null!; // what a combo box writes when its item goes away
        viewModel.SelectedPlatform.Platform.ShouldBe("any");
        window.Close();
    }

    [Fact]
    public void PackageEditor_KeepsFilesAndOperationsPerPlatform()
    {
        var editor = Editor();
        editor.Changelogs[0].Text = "Changes";
        editor.AddFile(PackageRoot.Program, "app.dll", "/src/app.dll");
        var changed = new List<string?>();
        editor.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        editor.NewPlatform = editor.AvailablePlatforms.Single(p => p.Platform == "win-x64");
        editor.AddPlatformCommand.Execute(null);
        editor.NewPlatform.ShouldBeNull();
        editor.AddPlatformCommand.Execute(null); // nothing chosen
        editor.Platforms.Select(p => p.Platform).ShouldBe(["any", "win-x64"]);
        editor.AvailablePlatforms.Select(p => p.Platform).ShouldNotContain("win-x64");
        editor.HasSeveralPlatforms.ShouldBeTrue();
        editor.SelectedPlatform.Platform.ShouldBe("win-x64");
        editor.Files.ShouldBeEmpty();
        changed.ShouldContain(nameof(PackageEditorViewModel.Files));
        changed.ShouldContain(nameof(PackageEditorViewModel.OperationPalette));

        editor.Validate().ShouldBe("Add at least one file or operation for Windows x64 (win-x64).");
        editor.OperationPalette.Select(k => k.AreaTitle).Distinct()
            .ShouldBe(["Files", "Processes", "Registry · Windows", "Services · Windows"]);
        editor.AddOperationOfKindCommand.Execute(OperationKind.FromType(StopServiceOperation.TypeName));
        editor.Operations.Single().Value = "svc";
        editor.Summary.ShouldBe("2 platforms · 1 file · 1 operation · English changelog required");
        editor.Validate().ShouldBeNull();

        editor.SelectedPlatform = editor.Platforms[0];
        editor.SelectedOperation.ShouldBeNull();
        editor.Files.Single().RelativePath.ShouldBe("app.dll");

        var request = editor.BuildRequest();
        request.Package.Platforms.Select(p => (p.Platform, p.Files.Count, p.Operations.Count))
            .ShouldBe([("any", 1, 0), ("win-x64", 0, 1)]);

        editor.SelectedPlatform = editor.Platforms[1];
        editor.RemovePlatformCommand.Execute(editor.Platforms[1]); // the selected one: the first takes over
        editor.Platforms.Single().Platform.ShouldBe("any");
        editor.SelectedPlatform.Platform.ShouldBe("any");
        editor.RemovePlatformCommand.Execute(editor.Platforms[0]); // the last one stays
        editor.RemovePlatformCommand.Execute(null);
        editor.Platforms.Count.ShouldBe(1);
        editor.NewPlatform = new PlatformChoice("linux");
        editor.AddPlatformCommand.Execute(null);
        editor.SelectedPlatform = editor.Platforms[0];
        editor.RemovePlatformCommand.Execute(editor.Platforms[1]); // removing another one keeps the selection
        editor.SelectedPlatform.Platform.ShouldBe("any");
        editor.RemovePlatformCommand.Execute(new PlatformItemViewModel("osx")); // not in the list
        editor.Platforms.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData("any", "Any platform (any)")]
    [InlineData("win", "Windows (every architecture) (win)")]
    [InlineData("win-x64", "Windows x64 (win-x64)")]
    [InlineData("win-x86", "Windows x86 (win-x86)")]
    [InlineData("win-arm64", "Windows ARM64 (win-arm64)")]
    [InlineData("linux", "Linux (every architecture) (linux)")]
    [InlineData("linux-x64", "Linux x64 (linux-x64)")]
    [InlineData("linux-arm64", "Linux ARM64 (linux-arm64)")]
    [InlineData("osx", "macOS (every architecture) (osx)")]
    [InlineData("osx-x64", "macOS Intel (osx-x64)")]
    [InlineData("osx-arm64", "macOS Apple silicon (osx-arm64)")]
    public void PlatformChoice_NamesEveryPlatform(string platform, string display)
    {
        new PlatformChoice(platform).Display.ShouldBe(display);
        new PlatformItemViewModel(platform).Display.ShouldBe(display);
        PlatformChoice.All.Count.ShouldBe(11);
    }

    [Fact]
    public async Task PackageEditor_AddsAnAppBundleAsTheProgramRootOnMacOS()
    {
        var editor = Editor();
        _context.FileSystem.AddFile("/build/Demo.app/Contents/Info.plist", new MockFileData("<plist/>"));
        _context.FileSystem.AddFile("/build/Demo.app/Contents/MacOS/Demo", new MockFileData("binary"));
        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns("/build/Demo.app");

        await editor.AddFolderCommand.ExecuteAsync(null); // any platform: an ordinary folder
        editor.Files.Select(f => f.Display).OrderBy(d => d, StringComparer.Ordinal).ShouldBe([
            "Program/Demo.app/Contents/Info.plist", "Program/Demo.app/Contents/MacOS/Demo"
        ]);

        editor.NewPlatform = new PlatformChoice("osx-arm64");
        editor.AddPlatformCommand.Execute(null);
        editor.SelectedPlatform.IsMacOS.ShouldBeTrue();
        await editor.AddFolderCommand.ExecuteAsync(null);
        editor.Files.Select(f => f.Display).OrderBy(d => d, StringComparer.Ordinal)
            .ShouldBe(["Program/Contents/Info.plist", "Program/Contents/MacOS/Demo"]);

        editor.Files.Clear();
        editor.TargetFolder = "bundles";
        await editor.AddFolderCommand.ExecuteAsync(null); // with a sub folder it is just a folder again
        editor.Files.Select(f => f.Display).ShouldContain("Program/bundles/Demo.app/Contents/MacOS/Demo");
    }

    [Fact]
    public async Task PackageEditor_RefusesSymbolicLinks()
    {
        var editor = Editor();
        var fs = _context.FileSystem;
        fs.AddFile("/src/real.dll", new MockFileData("x"));
        fs.File.CreateSymbolicLink("/src/link.dll", "/src/real.dll");
        _context.Files.PickFilesAsync(Arg.Any<string>(), Arg.Any<FileTypeFilter[]>())
            .Returns(["/src/real.dll", "/src/link.dll"]);
        await editor.AddFilesCommand.ExecuteAsync(null);
        editor.Files.ShouldBeEmpty();
        editor.ErrorMessage.ShouldBe(
            "\"/src/link.dll\" is a symbolic link. Packages cannot contain links; add the file or folder it points to instead.");

        fs.AddFile("/tree/a.txt", new MockFileData("a"));
        fs.AddDirectory("/elsewhere");
        fs.Directory.CreateSymbolicLink("/tree/linked", "/elsewhere");
        _context.Files.PickFolderAsync(Arg.Any<string>()).Returns("/tree");
        editor.ErrorMessage = null;
        await editor.AddFolderCommand.ExecuteAsync(null);
        editor.Files.ShouldBeEmpty();
        editor.ErrorMessage!.ShouldContain("linked");
    }

    [Fact]
    public async Task PackageEditor_EditsTheFilesAndOperationsOfAnExistingPackage()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.0"), Released = true });
        _context.FileSystem.AddFile("/work/win-x64/Program/app.exe", new MockFileData(new byte[2048]));
        _context.FileSystem.AddFile("/work/win-x64/Program/old.dll", new MockFileData("old"));
        _context.FileSystem.AddFile("/build/app.exe", new MockFileData(new byte[3 * 1024 * 1024]));
        _context.FileSystem.AddFile("/build/new.dll", new MockFileData("new"));
        var content = new PackageDefinition(new UpdateVersion("2.0.0"));
        var windows = content.GetOrAddPlatform("win-x64");
        windows.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.exe", "/work/win-x64/Program/app.exe"));
        windows.Files.Add(new PackageFileEntry(PackageRoot.Program, "old.dll", "/work/win-x64/Program/old.dll"));
        windows.Operations.Add(new StartProcessOperation { Path = "%program%/app.exe", WaitForExit = true });
        content.GetOrAddPlatform("linux-x64").Files
            .Add(new PackageFileEntry(PackageRoot.Program, "app", "/work/linux-x64/Program/app") { UnixMode = 0x1ED });
        content.GetOrAddPlatform("osx-arm64").Files
            .Add(new PackageFileEntry(PackageRoot.Program, "app", "/work/osx-arm64/Program/app"));
        var entry = new PackageInfo { Version = new UpdateVersion("2.0.0"), Changelog = { ["en"] = "Two" } };
        var editor = Editor(project, entry, content);

        editor.Platforms.Select(p => p.Platform).ShouldBe(["win-x64", "linux-x64", "osx-arm64"]);
        editor.Files.Select(f => (f.Display, f.Change, f.SizeText))
            .ShouldBe([
                ("Program/app.exe", FileChange.Unchanged, nUpdate.Ui.ByteSizeFormatter.Format(2048, CultureInfo.CurrentCulture)),
                ("Program/old.dll", FileChange.Unchanged, "3 bytes")
            ]);
        editor.Files[0].RootPlaceholder.ShouldBe("%program%");
        editor.Platforms[1].Files.Single().SizeText.ShouldBeEmpty(); // not on disk
        editor.SelectedOperation.ShouldBe(editor.Operations.Single());
        editor.Operations.Single().Summary.ShouldBe("%program%/app.exe · waits");
        editor.ChangesContent.ShouldBeFalse();
        editor.Summary.ShouldBe("Files and operations unchanged");
        editor.Sections.Single(s => s.Key == "files").Hint.ShouldBeEmpty();

        // Replacing, adding and removing files marks them; a removed file stays listed until it is kept after all.
        editor.AddFile(PackageRoot.Program, "APP.EXE", "/build/app.exe");
        editor.AddFile(PackageRoot.Program, "new.dll", "/build/new.dll");
        editor.RemoveFileCommand.Execute(editor.Files[1]);
        editor.Files.Select(f => (f.Display, f.Change)).ShouldBe([
            ("Program/app.exe", FileChange.Changed), ("Program/old.dll", FileChange.Removed),
            ("Program/new.dll", FileChange.Added)
        ]);
        editor.Files[0].SizeText.ShouldBe(nUpdate.Ui.ByteSizeFormatter.Format(3 * 1024 * 1024, CultureInfo.CurrentCulture));
        editor.Summary.ShouldBe("Windows x64: 1 file added, 1 file changed, 1 file removed");
        editor.Sections.Single(s => s.Key == "files").Hint.ShouldBe("3 changes");
        editor.KeepFileCommand.Execute(editor.Files[1]);
        editor.Files[1].Change.ShouldBe(FileChange.Unchanged);
        editor.KeepFileCommand.Execute(null);
        editor.RemoveFileCommand.Execute(editor.Files[1]);
        editor.RemoveFileCommand.Execute(editor.Files[2]); // a file added in the editor goes at once
        editor.Files.Count.ShouldBe(2);
        editor.AddFile(PackageRoot.Program, "old.dll", "/work/win-x64/Program/old.dll"); // the same file again
        editor.Files[1].Change.ShouldBe(FileChange.Unchanged);
        editor.RemoveFileCommand.Execute(editor.Files[1]);
        editor.Sections.Single(s => s.Key == "files").Hint.ShouldBe("2 changes");

        // Changing an operation, removing a platform and adding one: only the changed and new platforms are built.
        editor.Operations[0].FailOnError = true;
        editor.Summary.ShouldBe("Windows x64: 1 file changed, 1 file removed, operations changed");
        editor.RemovePlatformCommand.Execute(editor.Platforms[2]);
        editor.NewPlatform = new PlatformChoice("win-arm64");
        editor.AddPlatformCommand.Execute(null);
        editor.Validate().ShouldBe("Add at least one file or operation for Windows ARM64 (win-arm64).");
        editor.AddFile(PackageRoot.Program, "app.exe", "/build/app.exe");
        editor.Summary.ShouldBe(
            "Windows x64: 1 file changed, 1 file removed, operations changed · Windows ARM64: new · macOS Apple silicon: removed");
        editor.ChangesContent.ShouldBeTrue();

        _context.Publisher.RebuildPackageAsync(Arg.Any<PublishRequest>(), Arg.Any<IReadOnlyCollection<string>>(),
            Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>()).Returns(new UpdatePackage());
        var closed = new List<bool>();
        editor.CloseRequested += (_, a) => closed.Add(a);
        await editor.SaveCommand.ExecuteAsync(null);
        closed.ShouldBe([true]);
        await _context.Publisher.Received().RebuildPackageAsync(
            Arg.Is<PublishRequest>(r =>
                r.Package.Platforms.Select(p => p.Platform).SequenceEqual(new[] { "win-x64", "linux-x64", "win-arm64" }) &&
                r.Package.Platforms[0].Files.Single().SourcePath == "/build/app.exe" &&
                r.Package.Platforms[0].Files.Single().UnixMode == null &&
                r.Package.Platforms[1].Files.Single().UnixMode == 0x1ED &&
                r.Package.Platforms[0].Operations.Cast<StartProcessOperation>().Single().FailOnError),
            Arg.Is<IReadOnlyCollection<string>>(c => c.SequenceEqual(new[] { "win-x64", "win-arm64" })),
            Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>());
        await _context.Publisher.DidNotReceiveWithAnyArgs().UpdateEntryAsync(null!, null!, null!);
    }

    [Fact]
    public async Task PackageEditor_RebuildsALocalPackageWhenOnlyAPlatformIsRemoved()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.0") });
        var entry = new PackageInfo
        {
            Version = new UpdateVersion("2.0.0"),
            Changelog = { ["en"] = "Two" },
            Files = [new PackageFile { Platform = "win-x64" }, new PackageFile { Platform = "linux-x64" }],
        };
        var editor = Editor(project, entry);
        editor.RemovePlatformCommand.Execute(editor.Platforms[1]);
        editor.Summary.ShouldBe("Linux x64: removed");
        editor.SaveText.ShouldBe("Save package");
        await editor.SaveCommand.ExecuteAsync(null);
        await _context.Publisher.Received().RebuildPackageAsync(
            Arg.Is<PublishRequest>(r => r.Package.Platforms.Single().Platform == "win-x64"),
            Arg.Is<IReadOnlyCollection<string>>(c => c.Count == 0), Arg.Any<IProgress<PipelineProgress>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void PackageEditor_ExplainsThePlaceholdersOfThePlatform()
    {
        var editor = Editor();
        editor.HasPlaceholders.ShouldBeFalse();
        editor.PlaceholdersTitle.ShouldBe("Placeholders on Any platform");
        editor.Placeholders.Select(p => p.Name).ShouldBe(["%program%", "%appdata%", "%temp%", "%desktop%"]);
        editor.Placeholders.ShouldAllBe(p => p.Example.Length == 0);

        editor.AddOperationOfKindCommand.Execute(OperationKind.FromType(TerminateProcessOperation.TypeName));
        editor.HasPlaceholders.ShouldBeFalse(); // a process name is no path
        editor.InsertPlaceholderCommand.Execute(editor.Placeholders[0]);
        editor.SelectedOperation!.Value.ShouldBe("%program%/"); // offered by the window only for paths
        editor.AddOperationOfKindCommand.Execute(OperationKind.FromType(StartProcessOperation.TypeName));
        editor.HasPlaceholders.ShouldBeTrue();
        var start = editor.SelectedOperation!;
        editor.InsertPlaceholderCommand.Execute(editor.Placeholders[0]);
        start.Value.ShouldBe("%program%/");
        start.Value = "%appdata%/tool.exe";
        editor.InsertPlaceholderCommand.Execute(editor.Placeholders[0]);
        start.Value.ShouldBe("%program%/tool.exe");
        start.Value = "\\tools\\run.exe";
        editor.InsertPlaceholderCommand.Execute(editor.Placeholders[2]);
        start.Value.ShouldBe("%temp%/tools\\run.exe");
        start.Value = "%broken";
        editor.InsertPlaceholderCommand.Execute(editor.Placeholders[3]);
        start.Value.ShouldBe("%desktop%/%broken");
        editor.InsertPlaceholderCommand.Execute(null);
        start.Value.ShouldBe("%desktop%/%broken");
        editor.SelectedOperation = null;
        editor.InsertPlaceholderCommand.Execute(editor.Placeholders[0]);
        editor.HasPlaceholders.ShouldBeFalse();

        foreach (var (platform, title, program, appData) in new[]
                 {
                     ("win-x64", "Windows x64", @"C:\Program Files\Demo", @"C:\Users\‹user›\AppData\Roaming"),
                     ("linux", "Linux (every architecture)", "/opt/Demo", "~/.config"),
                     ("osx-arm64", "macOS Apple silicon", "/Applications/Demo.app", "~/.config"),
                 })
        {
            editor.NewPlatform = new PlatformChoice(platform);
            editor.AddPlatformCommand.Execute(null);
            editor.PlaceholdersTitle.ShouldBe($"Placeholders on {title}");
            editor.Placeholders[0].Example.ShouldBe(program);
            editor.Placeholders[1].Example.ShouldBe(appData);
            editor.Placeholders.ShouldAllBe(p => p.Meaning.Length > 0 && p.Example.Length > 0);
        }
    }

    [Fact]
    public async Task PackageEditor_ChangesOnlyTheFeedEntryWithoutThePackageFiles()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.0"), Released = true });
        var entry = new PackageInfo
        {
            Version = new UpdateVersion("2.0.0"),
            Changelog = { ["en"] = "Two" },
            Files = [new PackageFile { Platform = "win-x64" }, new PackageFile { Platform = "linux-x64" }],
        };
        var editor = _context.Factory.Create<PackageEditorViewModel>(project, AppTestContext.NewSecrets(),
            new ExistingPackage(entry, null));

        editor.CanChangeContent.ShouldBeFalse();
        editor.MissingContentNotice.ShouldStartWith("The package files of 2.0.0 are not on this computer");
        editor.Sections.Select(s => s.Key).ShouldBe(["general", "changelog", "availability", "conditions"]);
        editor.Sections.Select(s => s.Hint).ShouldBe(["", "English", "", "Everyone"]);
        editor.Platforms.Select(p => p.Platform).ShouldBe(["win-x64", "linux-x64"]);
        editor.ChangesContent.ShouldBeFalse();
        editor.Changelogs[0].Text = "Two, fixed";
        await editor.SaveCommand.ExecuteAsync(null);
        await _context.Publisher.Received().UpdateEntryAsync(project, Arg.Any<ProjectSecrets>(),
            Arg.Is<PackageInfo>(e => e.Changelog["en"] == "Two, fixed"), Arg.Any<IProgress<PipelineProgress>>(),
            Arg.Any<CancellationToken>());
        Editor().CanChangeContent.ShouldBeTrue();

        // An entry without any package file still opens with a platform to show.
        var empty = Editor(project, new PackageInfo { Version = new UpdateVersion("2.0.0"), Changelog = { ["en"] = "x" } });
        empty.Platforms.Single().Platform.ShouldBe("any");
        empty.ChangesContent.ShouldBeFalse();
    }

    [Fact]
    public void PackageEditor_ChecksOnlyTheOperationsOfPlatformsItBuilds()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.0") });
        _context.FileSystem.AddFile("/work/app", new MockFileData("app"));
        var content = new PackageDefinition(new UpdateVersion("2.0.0"));
        var any = content.GetOrAddPlatform("any");
        any.Files.Add(new PackageFileEntry(PackageRoot.Program, "app", "/work/app"));
        any.Operations.Add(new DeleteFilesOperation { Directory = "%program%", Files = [] }); // as nUpdate 4 left it
        var entry = new PackageInfo
        { Version = new UpdateVersion("2.0.0"), Changelog = { ["en"] = "x" }, Files = [new PackageFile()] };
        var editor = Editor(project, entry, content);

        editor.Validate().ShouldBeNull(); // the operation is not built again
        editor.AddFile(PackageRoot.Program, "readme.txt", "/work/app");
        editor.Validate()!.ShouldContain("at least one entry");
    }

    [Fact]
    public void PackageEditor_NoticesChangedRegistryValues()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.0") });
        var content = new PackageDefinition(new UpdateVersion("2.0.0"));
        content.GetOrAddPlatform("win-x64").Operations.Add(new SetRegistryValuesOperation
        { Key = @"HKEY_CURRENT_USER\Software\Aurora", Values = [RegistryValue.String("Theme", "dark")] });
        var entry = new PackageInfo
        {
            Version = new UpdateVersion("2.0.0"),
            Changelog = { ["en"] = "x" },
            Files = [new PackageFile { Platform = "win-x64" }],
        };
        var editor = Editor(project, entry, content);
        var changed = new List<string?>();
        editor.PropertyChanged += (_, e) => changed.Add(e.PropertyName);
        var set = editor.Operations.Single();

        set.RegistryValues.Single().Value = "light";
        changed.ShouldContain(nameof(PackageEditorViewModel.Summary));
        editor.Summary.ShouldBe("Windows x64: operations changed");
        set.RegistryValues.Single().Value = "dark";
        editor.Summary.ShouldBe("Files and operations unchanged");
        set.AddRegistryValueCommand.Execute(null);
        editor.Summary.ShouldBe("Windows x64: operations changed");
        set.RemoveRegistryValueCommand.Execute(set.RegistryValues[1]);
        editor.Summary.ShouldBe("Files and operations unchanged");
    }
}
