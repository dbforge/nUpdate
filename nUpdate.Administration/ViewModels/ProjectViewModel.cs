using System.Collections.ObjectModel;
using System.IO.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.Services;
using nUpdate.Exceptions;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Administration.ViewModels;

/// <summary>A package row in the project window.</summary>
public sealed class PackageItemViewModel(UpdatePackage package)
{
    public UpdatePackage Package { get; } = package ?? throw new ArgumentNullException(nameof(package));

    public string Version => Package.Version.ToString();

    public string Description => Package.Description;

    public string State => Package.Released ? "Released" : "Local only";

    public string Created => Package.CreatedAt == DateTimeOffset.MinValue
        ? "-"
        : Package.CreatedAt.LocalDateTime.ToString("d", System.Globalization.CultureInfo.CurrentCulture);
}

/// <summary>
///     The selected package as the details panel shows it. Platforms, size, rollout and changelog come from the local
///     feed entry; without one they read "-".
/// </summary>
public sealed class PackageDetailsViewModel(UpdatePackage package, PackageInfo? entry)
{
    public string Version => package.Version.ToString();

    public string Description => package.Description;

    public bool Released => package.Released;

    public IReadOnlyList<string> Platforms { get; } = entry?.Files.Select(f => f.Platform).ToList() ?? [];

    public string Size => entry is null
        ? "-"
        : ByteSizeFormatter.Format(entry.Files.Sum(f => f.Size), System.Globalization.CultureInfo.CurrentCulture);

    public string Rollout
    {
        get
        {
            if (entry is null)
                return "-";
            var conditions = entry.Rollout.Conditions.Count;
            var audience = conditions == 0 ? "Everyone" : conditions == 1 ? "1 condition" : $"{conditions} conditions";
            return entry.Necessary ? audience + " · necessary" : audience;
        }
    }

    public string Created => package.CreatedAt == DateTimeOffset.MinValue
        ? "-"
        : package.CreatedAt.LocalDateTime.ToString("g", System.Globalization.CultureInfo.CurrentCulture);

    public string Changelog => entry?.GetChangelog(System.Globalization.CultureInfo.CurrentUICulture) ?? string.Empty;

    public bool HasChangelog => Changelog.Length > 0;
}

/// <summary>A history row in the project window.</summary>
public sealed class LogItemViewModel(LogEntry entry)
{
    public LogEntry Entry { get; } = entry ?? throw new ArgumentNullException(nameof(entry));

    public string Time => Entry.At == DateTimeOffset.MinValue
        ? "-"
        : Entry.At.LocalDateTime.ToString("g", System.Globalization.CultureInfo.CurrentCulture);

    public string Kind => Entry.Kind.ToString();

    public string Version => Entry.Version?.ToString() ?? "-";

    public string User => Entry.User;
}

/// <summary>The window of an open project: packages, statistics, history, settings and the migration of legacy projects.</summary>
public partial class ProjectViewModel : DialogViewModel
{
    private readonly IPublishService _publisher;
    private readonly IFeedStore _feeds;
    private readonly IStatisticsApi _statistics;
    private readonly ILegacyFeedMigrator _migrator;
    private readonly IDialogService _dialogs;
    private readonly IClipboardService _clipboard;
    private readonly IFileSystem _fileSystem;
    private readonly ViewModelFactory _factory;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditPackageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PublishPackageCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeletePackageCommand))]
    private PackageItemViewModel? _selectedPackage;

    /// <summary>The details of the selected package, or <c>null</c> without a selection.</summary>
    [ObservableProperty] private PackageDetailsViewModel? _selectedDetails;

    [ObservableProperty] private string _statisticsStatus = string.Empty;

    [ObservableProperty] private long _totalDownloads;

    /// <summary>Filters the package list by version or description.</summary>
    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] private string _selectedSourceLanguage = "C#";

    [ObservableProperty] private string _feedStatus = "Not checked yet.";

    [ObservableProperty] private bool _feedReachable;

    [ObservableProperty] private string _statisticsUpdatedText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NeedsMigration))]
    [NotifyPropertyChangedFor(nameof(LegacyFeedPresent))]
    [NotifyPropertyChangedFor(nameof(MigrationText))]
    private MigrationStatus? _migration;

    public ProjectViewModel(IPublishService publisher, IFeedStore feeds, IStatisticsApi statistics,
        ILegacyFeedMigrator migrator,
        IDialogService dialogs, IClipboardService clipboard, IFileSystem fileSystem, ViewModelFactory factory,
        UpdateProject project, ProjectSecrets secrets)
    {
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
        _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        _migrator = migrator ?? throw new ArgumentNullException(nameof(migrator));
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
        _clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        Project = project ?? throw new ArgumentNullException(nameof(project));
        Secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));
        StatisticsStatus = project.Statistics.Enabled
            ? "Statistics have not been loaded yet."
            : "Statistics are disabled for this project.";
        Refresh();
    }

    /// <summary>Called by the window once it is open: loads the statistics and checks for a legacy feed, opening the migration assistant when it is needed.</summary>
    public async Task OnOpenedAsync()
    {
        await RefreshStatisticsAsync();
        await CheckMigrationAsync();
        if (NeedsMigration)
            await MigrateAsync();
    }

    public UpdateProject Project { get; }

    public ProjectSecrets Secrets { get; }

    public ObservableCollection<PackageItemViewModel> Packages { get; } = [];

    public ObservableCollection<LogItemViewModel> History { get; } = [];

    public ObservableCollection<VersionStatistics> VersionStatistics { get; } = [];

    public IReadOnlyList<string> SourceLanguages { get; } = ["C#", "Visual Basic"];

    /// <summary>The snippet for the selected language, copied to the clipboard.</summary>
    public string SourceSnippet => SelectedSourceLanguage == "Visual Basic"
        ? ClientSourceSnippet.VisualBasic(Project)
        : ClientSourceSnippet.CSharp(Project);

    /// <summary>The snippet as shown on the overview, with the long public key shortened.</summary>
    public string SourceSnippetPreview
    {
        get
        {
            var key = Project.PublicKey;
            if (key.Length <= 40)
                return SourceSnippet;
            return SourceSnippet.Replace(key, string.Concat(key.AsSpan(0, 24), "…", key.AsSpan(key.Length - 16)),
                StringComparison.Ordinal);
        }
    }

    public string PackagesSummary
    {
        get
        {
            var released = Project.Packages.Count(p => p.Released);
            var newest = Project.Packages.Where(p => p.Released).OrderByDescending(p => p.Version).FirstOrDefault();
            var count = released == 1 ? "1 package released" : $"{released} packages released";
            return newest is null
                ? count
                : $"{count} · newest {newest.Version} on {newest.CreatedAt.LocalDateTime.ToString("d", System.Globalization.CultureInfo.CurrentCulture)}";
        }
    }

    public string PackageCountText => Project.Packages.Count == 1 ? "1 package" : $"{Project.Packages.Count} packages";

    public int PackageCount => Project.Packages.Count;

    public string Name => Project.Name;

    public string Initials => ProjectCardViewModel.InitialsOf(Project.Name);

    public string UpdateUrl => Project.UpdateUrl;

    public string FeedUrl => Project.FeedUri.ToString();

    public string Folder => Project.Folder;

    public string PublicKey => Project.PublicKey;

    public string ProjectId => Project.Id.ToString();

    public string TransferSummary =>
        $"{Project.Transfer.Protocol} {Project.Transfer.Username}@{Project.Transfer.Host}:{Project.Transfer.Port}{Project.Transfer.Directory}";

    public bool StatisticsEnabled => Project.Statistics.Enabled;

    public bool HasSelectedPackage => SelectedPackage is not null;

    public bool CanPublishSelected => SelectedPackage is { Package.Released: false };

    /// <summary>The server has only the legacy feed; publishing is refused until the packages are migrated.</summary>
    public bool NeedsMigration => Migration?.NeedsMigration == true;

    /// <summary>The server still has the legacy feed and package folders, which old clients read.</summary>
    public bool LegacyFeedPresent => Migration?.LegacyFeedPresent == true;

    public string MigrationText => Migration switch
    {
        null => "Not checked yet.",
        { NeedsMigration: true } =>
            "Only the updates.json of nUpdate 3 or 4 is on the server. The migration assistant moves the packages to nUpdate 5 next to it; nothing can be published before.",
        { LegacyFeedPresent: true } =>
            "nUpdate 3 and 4 still have their updates.json on the server for the copies installed today. The migration assistant explains how to move them over; retire the old setup once every copy has moved.",
        _ => "No updates.json of nUpdate 3 or 4 on the server.",
    };

    /// <summary>Rebuilds the lists from the project model.</summary>
    public void Refresh()
    {
        Title = $"{Project.Name} - nUpdate Administration";
        var selected = SelectedPackage?.Package.Version;
        Packages.Clear();
        var filter = SearchText.Trim();
        foreach (var package in Project.Packages.OrderByDescending(p => p.Version))
        {
            if (filter.Length == 0 || package.Version.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                package.Description.Contains(filter, StringComparison.OrdinalIgnoreCase))
                Packages.Add(new PackageItemViewModel(package));
        }

        SelectedPackage = Packages.FirstOrDefault(p => p.Package.Version == selected);
        History.Clear();
        foreach (var entry in Project.Log.OrderByDescending(e => e.At))
            History.Add(new LogItemViewModel(entry));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Initials));
        OnPropertyChanged(nameof(UpdateUrl));
        OnPropertyChanged(nameof(FeedUrl));
        OnPropertyChanged(nameof(Folder));
        OnPropertyChanged(nameof(TransferSummary));
        OnPropertyChanged(nameof(StatisticsEnabled));
        OnPropertyChanged(nameof(SourceSnippet));
        OnPropertyChanged(nameof(SourceSnippetPreview));
        OnPropertyChanged(nameof(PackagesSummary));
        OnPropertyChanged(nameof(PackageCountText));
        OnPropertyChanged(nameof(PackageCount));
    }

    partial void OnSearchTextChanged(string value) => Refresh();

    partial void OnSelectedSourceLanguageChanged(string value)
    {
        OnPropertyChanged(nameof(SourceSnippet));
        OnPropertyChanged(nameof(SourceSnippetPreview));
    }

    [RelayCommand]
    private Task CopySourceAsync() => _clipboard.SetTextAsync(SourceSnippet);

    /// <summary>Downloads the feed from the update URL to show whether clients can reach it.</summary>
    [RelayCommand]
    private async Task CheckFeedAsync()
    {
        FeedStatus = "Checking...";
        FeedReachable = false;
        try
        {
            var feed = await _feeds.LoadRemoteAsync(Project, Secrets);
            if (feed is null)
            {
                FeedStatus = "No nupdate.json on the server yet.";
                return;
            }

            FeedStatus = feed.Packages.Count == 1
                ? "Reachable, 1 package"
                : $"Reachable, {feed.Packages.Count} packages";
            FeedReachable = true;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or IOException
                                       or InvalidOperationException or InvalidFeedException
                                       or UnsupportedFormatException)
        {
            FeedStatus = ex.Message;
        }
    }

    /// <summary>Looks for the legacy and the current feed on the server; failures leave the status unknown.</summary>
    [RelayCommand]
    private async Task CheckMigrationAsync()
    {
        try
        {
            Migration = await _migrator.CheckAsync(Project, Secrets);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or IOException
                                       or InvalidOperationException or InvalidFeedException
                                       or UnsupportedFormatException)
        {
            Migration = null;
            FeedStatus = ex.Message;
        }
    }

    /// <summary>Opens the migration assistant; afterwards the packages and the migration status are refreshed.</summary>
    [RelayCommand]
    private async Task MigrateAsync()
    {
        var assistant = _factory.Create<MigrationViewModel>(Project, Secrets);
        await _dialogs.ShowDialogAsync(assistant);
        Refresh();
        await CheckMigrationAsync();
    }

    /// <summary>Lists what nUpdate 3 and 4 left for the project and deletes it once the user confirmed the list.</summary>
    [RelayCommand]
    private async Task RetireLegacySetupAsync()
    {
        LegacyFiles? files = null;
        if (!await RunWithTrustAsync("Looking for the files of nUpdate 3 and 4...",
                async _ => files = await _migrator.FindLegacyFilesAsync(Project, Secrets)))
        {
            await ShowErrorAsync("Error while looking for the files of nUpdate 3 and 4");
            return;
        }

        if (files!.IsEmpty)
        {
            await _dialogs.ShowInfoAsync("Retire the nUpdate 4 setup",
                "Neither the server nor this computer has files of nUpdate 3 or 4 for this project.");
            await CheckMigrationAsync();
            return;
        }

        if (!await _dialogs.ConfirmAsync("Retire the nUpdate 4 setup", DescribeRetirement(files), "Delete"))
            return;
        if (!await RunWithTrustAsync("Deleting the files of nUpdate 3 and 4...",
                _ => _migrator.DeleteLegacyFilesAsync(Project, Secrets, files)))
        {
            await ShowErrorAsync("Error while deleting the files of nUpdate 3 and 4");
            return;
        }

        await CheckMigrationAsync();
    }

    /// <summary>The confirmation text: the consequence, then every file and folder that goes.</summary>
    public static string DescribeRetirement(LegacyFiles files)
    {
        ArgumentNullException.ThrowIfNull(files);
        var lines = new List<string>
        {
            "Installed copies of your application that still run nUpdate 3 or 4 will no longer find updates. This deletes:"
        };
        var server = files.ServerFiles.Concat(files.ServerDirectories.Select(d => d + "/")).ToList();
        if (server.Count > 0)
            lines.Add("On the server: " + string.Join(", ", server));
        if (files.LocalDirectories.Count > 0)
            lines.Add(
                "On this computer, the package copies nUpdate Administration 4 needs to edit or upload these packages again: " +
                string.Join(", ", files.LocalDirectories));
        return string.Join(Environment.NewLine + Environment.NewLine, lines);
    }

    partial void OnSelectedPackageChanged(PackageItemViewModel? value)
    {
        OnPropertyChanged(nameof(HasSelectedPackage));
        OnPropertyChanged(nameof(CanPublishSelected));
        SelectedDetails = value is null ? null : new PackageDetailsViewModel(value.Package, null);
        if (value is not null)
            _ = LoadDetailsAsync(value);
    }

    /// <summary>Reads the local feed entry of the package for the details panel, unless the selection moved on meanwhile.</summary>
    private async Task LoadDetailsAsync(PackageItemViewModel item)
    {
        PackageInfo? entry;
        try
        {
            entry = await _feeds.LoadEntryAsync(Project, item.Package.Version);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or Newtonsoft.Json.JsonException)
        {
            return;
        }

        if (SelectedPackage == item)
            SelectedDetails = new PackageDetailsViewModel(item.Package, entry);
    }

    [RelayCommand]
    private async Task AddPackageAsync()
    {
        var editor = _factory.Create<PackageEditorViewModel>(Project, Secrets);
        if (await _dialogs.ShowDialogAsync(editor))
            Refresh();
    }

    /// <summary>
    ///     Extracts the package into a temporary folder and opens it in the editor; the folder holds the files the editor
    ///     builds the package from again and is deleted once the editor is closed.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private async Task EditPackageAsync()
    {
        var version = SelectedPackage!.Package.Version;
        var workingDirectory = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), $"nupdate-edit-{Guid.NewGuid():N}");
        try
        {
            ExistingPackage? existing = null;
            var ok = await RunBusyAsync("Opening the package...", async _ =>
            {
                var entry = await _feeds.LoadEntryAsync(Project, version)
                            ?? throw new InvalidOperationException(
                                $"The local feed entry of {version} is missing. Delete and recreate the package.");
                PackageDefinition? content;
                try
                {
                    content = await _publisher.OpenPackageAsync(Project, version, workingDirectory);
                }
                catch (FileNotFoundException)
                {
                    content = null; // the package files are elsewhere: the feed entry can still change
                }

                existing = new ExistingPackage(entry, content);
            });
            if (!ok)
            {
                await ShowErrorAsync("Error while opening the package");
                return;
            }

            var editor = _factory.Create<PackageEditorViewModel>(Project, Secrets, existing!);
            if (await _dialogs.ShowDialogAsync(editor))
                Refresh();
        }
        finally
        {
            DeleteWorkingDirectory(workingDirectory);
        }
    }

    /// <summary>Deletes the extracted package; a file that is still in use leaves it to the system's temp cleanup.</summary>
    private void DeleteWorkingDirectory(string path)
    {
        try
        {
            if (_fileSystem.Directory.Exists(path))
                _fileSystem.Directory.Delete(path, recursive: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
        }
    }

    [RelayCommand(CanExecute = nameof(CanPublishSelected))]
    private async Task PublishPackageAsync()
    {
        var version = SelectedPackage!.Package.Version;
        if (!await _dialogs.ConfirmAsync("Publish package",
                $"Upload the package {version} to the server and make it available to all clients?", "Publish"))
            return;
        var ok = await RunWithTrustAsync("Publishing...",
            progress => _publisher.PublishExistingAsync(Project, Secrets, version, progress));
        Refresh();
        if (!ok)
            await ShowErrorAsync("Error while publishing the package");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private async Task DeletePackageAsync()
    {
        var package = SelectedPackage!.Package;
        var text = package.Released
            ? $"Delete the package {package.Version} from the server and locally? Clients will no longer receive it."
            : $"Delete the local package {package.Version}?";
        if (!await _dialogs.ConfirmAsync("Delete package", text, "Delete"))
            return;
        var ok = await RunWithTrustAsync("Deleting...",
            progress => _publisher.DeletePackageAsync(Project, Secrets, package.Version, progress));
        Refresh();
        if (!ok)
            await ShowErrorAsync("Error while deleting the package");
    }

    /// <summary>
    ///     Runs a server operation; when the server's certificate or host key is unknown, offers to trust its fingerprint
    ///     and retries once. The fingerprint is saved with the project by the operation itself.
    /// </summary>
    private Task<bool> RunWithTrustAsync(string busyText, Func<IProgress<PipelineProgress>, Task> action) =>
        RunTrustingAsync(_dialogs, Project.Transfer, busyText, action);

    [RelayCommand]
    private Task CopyPublicKeyAsync() => _clipboard.SetTextAsync(Project.PublicKey);

    [RelayCommand]
    private Task CopyCSharpSourceAsync() => _clipboard.SetTextAsync(ClientSourceSnippet.CSharp(Project));

    [RelayCommand]
    private Task CopyVisualBasicSourceAsync() => _clipboard.SetTextAsync(ClientSourceSnippet.VisualBasic(Project));

    [RelayCommand]
    private Task CopyFeedUrlAsync() => _clipboard.SetTextAsync(FeedUrl);

    [RelayCommand]
    private async Task RefreshStatisticsAsync()
    {
        if (!Project.Statistics.Enabled)
            return;
        if (string.IsNullOrEmpty(Secrets.StatisticsAdminSecret))
        {
            StatisticsStatus = "The statistics admin secret is missing. Enter it in the credentials.";
            return;
        }

        StatisticsStatus = "Loading...";
        try
        {
            var result = await _statistics.GetStatisticsAsync(PublishService.Endpoint(Project, Secrets), Project.Id);
            VersionStatistics.Clear();
            foreach (var version in result.Versions.OrderBy(v => v.Version))
                VersionStatistics.Add(version);
            TotalDownloads = result.Total;
            StatisticsStatus = result.Versions.Count == 1
                ? "across 1 version"
                : $"across {result.Versions.Count} versions";
            StatisticsUpdatedText = "Last updated " +
                                    DateTime.Now.ToString("g", System.Globalization.CultureInfo.CurrentCulture);
        }
        catch (StatisticsException ex)
        {
            StatisticsStatus = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            StatisticsStatus = ex.Message;
        }
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        var settings = _factory.Create<ProjectSettingsViewModel>(Project, Secrets);
        var accepted = await _dialogs.ShowDialogAsync(settings);
        if (settings.Deleted)
        {
            Close(true);
            return;
        }

        if (accepted || settings.Migrated)
            Refresh();
        if (settings.Migrated)
            await CheckMigrationAsync();
    }

    private async Task ShowErrorAsync(string title)
    {
        if (ErrorMessage is not null)
            await _dialogs.ShowErrorAsync(title, ErrorMessage);
    }
}
