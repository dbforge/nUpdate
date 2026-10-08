using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using nUpdate.Administration;
using nUpdate.Administration.Core;
using nUpdate.Administration.ViewModels;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.App;

/// <summary>How the view models are built: the real container must resolve every one of them, and every constructor rejects null.</summary>
public sealed class CompositionTests : IDisposable
{
    private readonly AppTestContext _context = new();
    private readonly string _root = Path.Combine(Path.GetTempPath(), "nupdate-composition-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    [Fact]
    public void RealContainer_ResolvesEveryViewModel()
    {
        using var provider = AppServices.Build(new AdministrationPaths(new FileSystem(), _root, Path.Combine(_root, "projects")));
        var factory = provider.GetRequiredService<ViewModelFactory>();
        var project = AppTestContext.NewProject(statistics: true);
        var secrets = AppTestContext.NewSecrets(statistics: true);
        var content = new nUpdate.Administration.Core.Packages.PackageDefinition(new UpdateVersion("1.0.0"));
        content.GetOrAddPlatform("any");
        var existing = new ExistingPackage(new PackageInfo { Version = new UpdateVersion("1.0.0") }, content);

        provider.GetRequiredService<MainWindowViewModel>().ShouldNotBeNull();
        factory.Create<NewProjectViewModel>().ShouldNotBeNull();
        factory.Create<ProjectPasswordViewModel>().ShouldNotBeNull();
        factory.Create<CredentialsViewModel>(project, secrets, CredentialsMode.Secrets).ShouldNotBeNull();
        factory.Create<ProjectSettingsViewModel>(project, secrets).ShouldNotBeNull();
        factory.Create<PackageEditorViewModel>(project, secrets).ShouldNotBeNull();
        factory.Create<PackageEditorViewModel>(project, secrets, existing).ShouldNotBeNull();
        factory.Create<ProjectViewModel>(project, secrets).ShouldNotBeNull();
    }

    [Fact]
    public void Constructors_RejectNullDependencies()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var transfer = _context.Factory.Create<TransferSettingsEditorViewModel>();
        var statistics = _context.Factory.Create<StatisticsSettingsEditorViewModel>();
        var c = _context;

        Should.Throw<ArgumentNullException>(() => new MainWindowViewModel(null!, c.Projects, c.Passwords, c.Dialogs, c.Files, c.Factory));
        Should.Throw<ArgumentNullException>(() => new MainWindowViewModel(c.Store, null!, c.Passwords, c.Dialogs, c.Files, c.Factory));
        Should.Throw<ArgumentNullException>(() => new MainWindowViewModel(c.Store, c.Projects, null!, c.Dialogs, c.Files, c.Factory));
        Should.Throw<ArgumentNullException>(() => new MainWindowViewModel(c.Store, c.Projects, c.Passwords, null!, c.Files, c.Factory));
        Should.Throw<ArgumentNullException>(() => new MainWindowViewModel(c.Store, c.Projects, c.Passwords, c.Dialogs, null!, c.Factory));
        Should.Throw<ArgumentNullException>(() => new MainWindowViewModel(c.Store, c.Projects, c.Passwords, c.Dialogs, c.Files, null!));

        Should.Throw<ArgumentNullException>(() => new NewProjectViewModel(null!, c.Paths, c.Files, transfer, statistics));
        Should.Throw<ArgumentNullException>(() => new NewProjectViewModel(c.Projects, null!, c.Files, transfer, statistics));
        Should.Throw<ArgumentNullException>(() => new NewProjectViewModel(c.Projects, c.Paths, null!, transfer, statistics));
        Should.Throw<ArgumentNullException>(() => new NewProjectViewModel(c.Projects, c.Paths, c.Files, null!, statistics));
        Should.Throw<ArgumentNullException>(() => new NewProjectViewModel(c.Projects, c.Paths, c.Files, transfer, null!));

        Should.Throw<ArgumentNullException>(() => new TransferSettingsEditorViewModel(null!, c.Dialogs, c.Files));
        Should.Throw<ArgumentNullException>(() => new TransferSettingsEditorViewModel(c.Projects, null!, c.Files));
        Should.Throw<ArgumentNullException>(() => new TransferSettingsEditorViewModel(c.Projects, c.Dialogs, null!));

        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(null!, c.Passwords, c.Factory, c.Dialogs, transfer, statistics, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(c.Projects, null!, c.Factory, c.Dialogs, transfer, statistics, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(c.Projects, c.Passwords, null!, c.Dialogs, transfer, statistics, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(c.Projects, c.Passwords, c.Factory, null!, transfer, statistics, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(c.Projects, c.Passwords, c.Factory, c.Dialogs, null!, statistics, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(c.Projects, c.Passwords, c.Factory, c.Dialogs, transfer, null!, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(c.Projects, c.Passwords, c.Factory, c.Dialogs, transfer, statistics, null!, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectSettingsViewModel(c.Projects, c.Passwords, c.Factory, c.Dialogs, transfer, statistics, project, null!));

        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(null!, c.Feeds, c.Statistics, c.Migrator, c.Dialogs, c.Clipboard, c.FileSystem, c.Factory, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, null!, c.Statistics, c.Migrator, c.Dialogs, c.Clipboard, c.FileSystem, c.Factory, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, null!, c.Migrator, c.Dialogs, c.Clipboard, c.FileSystem, c.Factory, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, c.Statistics, null!, c.Dialogs, c.Clipboard, c.FileSystem, c.Factory, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, c.Statistics, c.Migrator, null!, c.Clipboard, c.FileSystem, c.Factory, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, c.Statistics, c.Migrator, c.Dialogs, null!, c.FileSystem, c.Factory, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, c.Statistics, c.Migrator, c.Dialogs, c.Clipboard, null!, c.Factory, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, c.Statistics, c.Migrator, c.Dialogs, c.Clipboard, c.FileSystem, null!, project, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, c.Statistics, c.Migrator, c.Dialogs, c.Clipboard, c.FileSystem, c.Factory, null!, secrets));
        Should.Throw<ArgumentNullException>(() => new ProjectViewModel(c.Publisher, c.Feeds, c.Statistics, c.Migrator, c.Dialogs, c.Clipboard, c.FileSystem, c.Factory, project, null!));

        Should.Throw<ArgumentNullException>(() => new PackageEditorViewModel(null!, c.Files, c.FileSystem, project, secrets));
        Should.Throw<ArgumentNullException>(() => new PackageEditorViewModel(c.Publisher, null!, c.FileSystem, project, secrets));
        Should.Throw<ArgumentNullException>(() => new PackageEditorViewModel(c.Publisher, c.Files, null!, project, secrets));
        Should.Throw<ArgumentNullException>(() => new PackageEditorViewModel(c.Publisher, c.Files, c.FileSystem, null!, secrets));
        Should.Throw<ArgumentNullException>(() => new PackageEditorViewModel(c.Publisher, c.Files, c.FileSystem, project, null!));

        Should.Throw<ArgumentNullException>(() => new PackageItemViewModel(null!));
        Should.Throw<ArgumentNullException>(() => new LogItemViewModel(null!));
        Should.Throw<ArgumentNullException>(() => new ChangelogItemViewModel(null!));
    }
}
