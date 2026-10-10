using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.Core.Transfer;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Administration;

/// <summary>Wires the Core services and the view models.</summary>
[ExcludeFromCodeCoverage] // Composition root; CompositionTests resolve every view model from it.
public static class AppServices
{
    /// <param name="paths">The data folder; defaults to the user's roaming application data.</param>
    /// <param name="configure">Runs after the default registrations, so tests can replace the platform services.</param>
    public static ServiceProvider Build(AdministrationPaths? paths = null, Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        var fileSystem = new FileSystem();
        paths ??= AdministrationPaths.Default(fileSystem);
        fileSystem.Directory.CreateDirectory(paths.Root);

        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Information));
        services.AddSingleton<IFileSystem>(fileSystem);
        services.AddSingleton(paths);
        services.AddSingleton<ICredentialProtector>(_ =>
            DataProtectionCredentialProtector.CreateForDirectory(paths.KeyRingDirectory));
        services.AddSingleton<IProjectHttpClientFactory, ProjectHttpClientFactory>();
        services.AddSingleton<IProjectStore, ProjectStore>();
        services.AddSingleton<IProjectPasswordStore, ProjectPasswordStore>();
        services.AddSingleton<IProjectLogger, ProjectLogger>();
        services.AddSingleton<ITransferProviderFactory, TransferProviderFactory>();
        services.AddSingleton<IStatisticsApi, StatisticsApiClient>();
        services.AddSingleton<IPackageBuilder, PackageBuilder>();
        services.AddSingleton<IPackageSigner, PackageSigner>();
        services.AddSingleton<IPackageContentReader, PackageContentReader>();
        services.AddSingleton<IFeedStore, FeedStore>();
        services.AddSingleton<ILegacyFeedMigrator, LegacyFeedMigrator>();
        services.AddSingleton<IFeedChecker, FeedChecker>();
        services.AddSingleton<IPublishService, PublishService>();
        services.AddSingleton<IProjectService, ProjectService>();

        services.AddSingleton<DialogService>();
        services.AddSingleton<IDialogService>(provider => provider.GetRequiredService<DialogService>());
        services.AddSingleton<IFilePickerService>(provider => provider.GetRequiredService<DialogService>());
        services.AddSingleton<IClipboardService>(provider => provider.GetRequiredService<DialogService>());
        services.AddSingleton<ViewModelFactory>();
        services.AddSingleton<MainWindowViewModel>();
        // Editors are composed into the dialogs that edit project settings, one fresh instance per dialog.
        services.AddTransient<TransferSettingsEditorViewModel>();
        services.AddTransient<StatisticsSettingsEditorViewModel>();
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }
}
