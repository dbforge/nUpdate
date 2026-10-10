using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.Services;
using nUpdate.Administration.ViewModels;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.App;

/// <summary>Substituted services for view model tests, plus a factory that resolves view models from them.</summary>
public sealed class AppTestContext
{
    public AppTestContext()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Dialogs);
        services.AddSingleton(Files);
        services.AddSingleton(Clipboard);
        services.AddSingleton(Projects);
        services.AddSingleton(Publisher);
        services.AddSingleton(Store);
        services.AddSingleton(Passwords);
        services.AddSingleton(Statistics);
        services.AddSingleton(Feeds);
        services.AddSingleton(Migrator);
        services.AddSingleton(FeedChecker);
        services.AddSingleton<System.IO.Abstractions.IFileSystem>(FileSystem);
        services.AddSingleton(Paths);
        services.AddSingleton<ViewModelFactory>();
        services.AddTransient<TransferSettingsEditorViewModel>();
        services.AddTransient<StatisticsSettingsEditorViewModel>();
        services.AddTransient<MainWindowViewModel>();
        Provider = services.BuildServiceProvider();
        Factory = Provider.GetRequiredService<ViewModelFactory>();
        Store.ListAsync(Arg.Any<CancellationToken>()).Returns([]);
        Statistics.GetStatisticsAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new ProjectStatistics());
        Migrator.CheckAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>())
            .Returns(new MigrationStatus(false, true));
        Migrator.RunAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(), Arg.Any<MigrationPlan>(),
            Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns([]);
        Passwords.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((string?)null);
    }

    public IDialogService Dialogs { get; } = Substitute.For<IDialogService>();

    public IFilePickerService Files { get; } = Substitute.For<IFilePickerService>();

    public IClipboardService Clipboard { get; } = Substitute.For<IClipboardService>();

    public IProjectService Projects { get; } = Substitute.For<IProjectService>();

    public IPublishService Publisher { get; } = Substitute.For<IPublishService>();

    public IProjectStore Store { get; } = Substitute.For<IProjectStore>();

    public IProjectPasswordStore Passwords { get; } = Substitute.For<IProjectPasswordStore>();

    public IStatisticsApi Statistics { get; } = Substitute.For<IStatisticsApi>();

    public IFeedStore Feeds { get; } = Substitute.For<IFeedStore>();

    public ILegacyFeedMigrator Migrator { get; } = Substitute.For<ILegacyFeedMigrator>();

    public IFeedChecker FeedChecker { get; } = Substitute.For<IFeedChecker>();


    public MockFileSystem FileSystem { get; } = new();

    public AdministrationPaths Paths => new(FileSystem,
        FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), "nupdate-app"),
        FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), "nupdate-app-projects"));

    public ServiceProvider Provider { get; }

    public ViewModelFactory Factory { get; }

    public static UpdateProject NewProject(bool statistics = false) =>
        new AdminTestContext().NewProject(statistics: statistics);

    public static ProjectSecrets NewSecrets(bool statistics = false) => AdminTestContext.NewSecrets(statistics);

    public static ProjectLoadResult Loaded(UpdateProject project, ProjectSecrets secrets, bool migrated = false) =>
        new(project, secrets, migrated, SecretsState.Loaded);
}
