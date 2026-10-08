using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.Services;
using nUpdate.Packaging;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Administration.ViewModels;

/// <summary>
///     The migration assistant: shows what is on the server and what the migration will add and keep, lets the user
///     review the packages and the statistics, runs the migration and then explains, step by step, how to run nUpdate 4
///     and nUpdate 5 side by side until every installation has moved.
/// </summary>
public sealed partial class MigrationViewModel : DialogViewModel, IDisposable
{
    public const int OverviewStep = 0;
    public const int PackagesStep = 1;
    public const int StatisticsStep = 2;
    public const int MigrateStep = 3;
    public const int SideBySideStep = 4;

    private readonly ILegacyFeedMigrator _migrator;
    private readonly IFeedChecker _checker;
    private readonly IDialogService _dialogs;
    private readonly IClipboardService _clipboard;
    private MigrationPlan? _plan;
    private bool _disposed;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverviewStep))]
    [NotifyPropertyChangedFor(nameof(IsPackagesStep))]
    [NotifyPropertyChangedFor(nameof(IsStatisticsStep))]
    [NotifyPropertyChangedFor(nameof(IsMigrateStep))]
    [NotifyPropertyChangedFor(nameof(IsSideBySideStep))]
    [NotifyPropertyChangedFor(nameof(ContinueText))]
    [NotifyPropertyChangedFor(nameof(CanReload))]
    [NotifyCanExecuteChangedFor(nameof(BackCommand))]
    private int _step;

    /// <summary>The migration ran in this dialog.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SideBySideIntroduction))]
    [NotifyPropertyChangedFor(nameof(CanReload))]
    private bool _migrated;

    [ObservableProperty]
    private string _checkStatus = "Not checked yet.";

    [ObservableProperty]
    private bool? _checkSucceeded;

    public MigrationViewModel(ILegacyFeedMigrator migrator, IFeedChecker checker, IDialogService dialogs, IClipboardService clipboard, UpdateProject project, ProjectSecrets secrets)
    {
        _migrator = migrator ?? throw new ArgumentNullException(nameof(migrator));
        _checker = checker ?? throw new ArgumentNullException(nameof(checker));
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
        _clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));
        Project = project ?? throw new ArgumentNullException(nameof(project));
        Secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));
        Title = $"Move {project.Name} to nUpdate 5";
        Steps = [new WizardStep(1, "What changes"), new WizardStep(2, "Packages"), new WizardStep(3, "Statistics"), new WizardStep(4, "Migrate"), new WizardStep(5, "Side by side")];
        Steps[0].IsCurrent = true;
        CloseRequested += (_, _) => Dispose();
    }

    public UpdateProject Project { get; }

    public ProjectSecrets Secrets { get; }

    public IReadOnlyList<WizardStep> Steps { get; }

    public bool IsOverviewStep => Step == OverviewStep;

    public bool IsPackagesStep => Step == PackagesStep;

    public bool IsStatisticsStep => Step == StatisticsStep;

    public bool IsMigrateStep => Step == MigrateStep;

    public bool IsSideBySideStep => Step == SideBySideStep;

    public string ContinueText => Step switch
    {
        MigrateStep => NothingToMigrate ? "Continue" : "Start migration",
        SideBySideStep => "Done",
        _ => "Continue",
    };

    /// <summary>The server has been read; until then (or when reading it failed) the assistant cannot go on.</summary>
    public bool IsPrepared => _plan is not null;

    /// <summary>The server can be read again (to retry a download, or after changing files there) until the migration ran.</summary>
    public bool CanReload => !Migrated && Step < SideBySideStep;

    public ObservableCollection<MigrationPackageItemViewModel> Packages { get; } = [];

    /// <summary>The lines of the last feed check: one per package, then the statistics.</summary>
    public ObservableCollection<string> CheckResults { get; } = [];

    /// <summary>The migration is running; the window cannot be closed until it finished or was rolled back.</summary>
    public bool IsMigrating { get; private set; }

    /// <summary>The versions the migration carried over, once it ran.</summary>
    public IReadOnlyList<UpdateVersion> MigratedVersions { get; private set; } = [];

    // --- What changes ---------------------------------------------------------------------------------------------------

    public IReadOnlyList<string> ServerState
    {
        get
        {
            if (_plan is null)
                return [];
            var lines = new List<string>
            {
                _plan.LegacyFeedPresent
                    ? $"{LegacyFeed.FileName} with {Count(_plan.Packages.Count, "package")}, which applications built with nUpdate 3 or 4 read."
                    : $"No {LegacyFeed.FileName}: nUpdate 3 or 4 never published to this server, or its files are gone already.",
                _plan.ExistingFeed is null
                    ? $"No {UpdateFeed.FileName} yet."
                    : $"{UpdateFeed.FileName} with {Count(_plan.ExistingFeed.Packages.Count, "package")}, which applications built with nUpdate 5 read.",
            };
            if (_plan.LegacyStatisticsUsed)
                lines.Add($"Packages of {LegacyFeed.FileName} report their downloads to {StatisticsScript.LegacyScriptFileName}.");
            if (_plan.UnreadableVersions.Count > 0)
                lines.Add($"{LegacyFeed.FileName} has entries whose version nUpdate 5 cannot read; they are left out: {string.Join(", ", _plan.UnreadableVersions)}.");
            return lines;
        }
    }

    public IReadOnlyList<string> AddedItems
    {
        get
        {
            var lines = new List<string>
            {
                $"packages/<version>/<platform>.zip for every package you select (win-x86, win-x64 or win, as nUpdate 4 only ran on Windows): repacked with a {PackageLayout.ManifestFileName} and signed with RSA-PSS. The key pair stays the same; nUpdate 5 uses it in PEM form.",
                $"{UpdateFeed.FileName}: the feed applications built with nUpdate 5 read, at {Project.FeedUri}.",
            };
            if (Project.Statistics.Enabled)
                lines.Add($"{StatisticsScript.ScriptFileName} and {StatisticsScript.ConfigFileName}: the statistics of nUpdate 5, with their own tables in the same database.");
            lines.Add($"On this computer: the new packages in {Project.PackagesDirectory}.");
            return lines;
        }
    }

    public IReadOnlyList<string> KeptItems
    {
        get
        {
            var folders = _plan?.Packages.Select(p => p.LiteralVersion + "/").ToList() ?? [];
            var lines = new List<string>
            {
                folders.Count == 0
                    ? $"{LegacyFeed.FileName} and the package folders of nUpdate 3 and 4, if there are any."
                    : $"{LegacyFeed.FileName} and the package folders {string.Join(", ", folders)}: installed copies of your application that still run nUpdate 3 or 4 keep updating from them.",
            };
            if (_plan?.LegacyStatisticsUsed == true)
                lines.Add($"{StatisticsScript.LegacyScriptFileName} and its tables: those copies keep reporting their downloads.");
            lines.Add(Project.LegacyProjectFile is { } file
                ? $"Your project file of nUpdate Administration 4 ({file}) and its package copies, so you can keep publishing to {LegacyFeed.FileName} with it in the meantime."
                : $"The project and the package copies of nUpdate Administration 4, so you can keep publishing to {LegacyFeed.FileName} with it in the meantime.");
            return lines;
        }
    }

    // --- Packages -------------------------------------------------------------------------------------------------------

    public string SelectionSummary
    {
        get
        {
            var pending = Packages.Count(p => !p.AlreadyMigrated);
            var selected = Packages.Count(p => p.Include);
            if (Packages.Count == 0)
                return $"{LegacyFeed.FileName} lists no packages.";
            if (pending == 0)
                return $"Every package of {LegacyFeed.FileName} is in {UpdateFeed.FileName} already.";
            return $"{selected} of {Count(pending, "package")} to migrate selected.";
        }
    }

    // --- Statistics -----------------------------------------------------------------------------------------------------

    public bool StatisticsEnabled => Project.Statistics.Enabled;

    public IReadOnlyList<string> StatisticsDetails
    {
        get
        {
            if (!StatisticsEnabled)
                return [];
            var database = Project.Statistics.Database;
            return
            [
                $"Endpoint: {PublishService.StatisticsUri(Project)}",
                database is null ? "Database: not set" : $"Database: {database.Name} on {database.Host}, user {database.Username}",
                string.IsNullOrEmpty(Secrets.StatisticsDatabasePassword) ? "Database password: missing" : "Database password: entered",
                string.IsNullOrEmpty(Secrets.StatisticsAdminSecret) ? "Admin secret: missing" : "Admin secret: present",
            ];
        }
    }

    /// <summary>Why the statistics cannot be set up, or <c>null</c>.</summary>
    public string? StatisticsProblem
    {
        get
        {
            if (!StatisticsEnabled)
                return null;
            const string Fix = " Close the assistant, enter it in the project settings, save them and open the assistant again from the project window.";
            if (Project.Statistics.Database is null || string.IsNullOrWhiteSpace(Project.Statistics.Database.Name))
                return "The statistics need database settings." + Fix.Replace("enter it", "enter them", StringComparison.Ordinal);
            if (string.IsNullOrEmpty(Secrets.StatisticsDatabasePassword))
                return "The database password of the statistics is missing." + Fix;
            return string.IsNullOrEmpty(Secrets.StatisticsAdminSecret)
                ? "The statistics admin secret is missing. Close the assistant, enter it in the project credentials and open the assistant again from the project window."
                : null;
        }
    }

    // --- Migrate --------------------------------------------------------------------------------------------------------

    /// <summary>Nothing is selected and the server has <c>nupdate.json</c> already, so the migration has nothing to do.</summary>
    public bool NothingToMigrate => _plan is not null && _plan.ExistingFeed is not null && !_plan.Included.Any();

    public IReadOnlyList<string> MigrationSummary
    {
        get
        {
            if (_plan is null)
                return [];
            if (NothingToMigrate)
                return [$"No package is selected and {UpdateFeed.FileName} exists already, so there is nothing to do. Continue to see how to run both versions side by side."];
            var included = _plan.Included.ToList();
            var lines = new List<string>
            {
                included.Count == 0
                    ? $"No package is selected: {UpdateFeed.FileName} starts empty, and you publish your first nUpdate 5 version from the project window."
                    : $"{Count(included.Count, "package")} ({string.Join(", ", included.Select(p => p.Version))}) {(included.Count == 1 ? "is" : "are")} repacked, signed and uploaded to packages/.",
                $"{UpdateFeed.FileName} is written with {Count((_plan.ExistingFeed?.Packages.Count ?? 0) + included.Count, "package")}.",
            };
            if (StatisticsEnabled)
                lines.Add($"{StatisticsScript.ScriptFileName} is uploaded" + (included.Count > 0 ? " and the versions are registered in it." : "."));
            if (included.Count > 0)
                lines.Add("The project is saved with the packages marked as released.");
            lines.Add("Nothing of nUpdate 3 and 4 is changed. If a step fails, what this migration uploaded is removed again.");
            return lines;
        }
    }

    // --- Side by side ---------------------------------------------------------------------------------------------------

    public string SideBySideIntroduction
    {
        get
        {
            var leftOut = _plan?.Pending.Count(p => !(Migrated && p.Include)) ?? 0;
            var start = Migrated ? "The migration is done." : leftOut == 0 ? $"Every package of {LegacyFeed.FileName} is in {UpdateFeed.FileName}." : "Nothing was migrated in this run.";
            var missing = leftOut == 0
                ? string.Empty
                : $" {Count(leftOut, "package")} of {LegacyFeed.FileName} {(leftOut == 1 ? "is" : "are")} not in {UpdateFeed.FileName}, so applications built with nUpdate 5 do not see {(leftOut == 1 ? "it" : "them")}; that only matters for versions newer than your first nUpdate 5 release.";
            return start + missing + " Follow these steps to move your users to nUpdate 5 while nUpdate 4 keeps serving the copies that are installed today.";
        }
    }

    public string ClientSnippet => ClientSourceSnippet.CSharp(Project);

    /// <summary>The newest package of the legacy feed, which the examples refer to.</summary>
    private MigrationPackage? NewestPackage => _plan is { Packages.Count: > 0 } plan ? plan.Packages[^1] : null;

    /// <summary>
    ///     The version the build of step 2 declares: higher than everything in <c>updates.json</c>, since step 3 ships it
    ///     through the old feed. A version the installed copies already have would offer itself as an update forever.
    /// </summary>
    public string VersionExample
    {
        get
        {
            var (bridge, bridgeLegacy) = BridgeVersion;
            var newest = NewestPackage;
            var higher = newest is null ? string.Empty : $" It has to be higher than {newest.LiteralVersion}, the newest package in {LegacyFeed.FileName}, because step 3 ships it through the old feed.";
            return $"Declare the version of that build in the new form, [assembly: ApplicationVersion(\"{bridge}\")] for what nUpdate Administration 4 calls {bridgeLegacy}, instead of [assembly: nUpdateVersion(...)].{higher} The UpdateManager reads the version from that attribute; nUpdate 5 no longer reads spellings like {bridgeLegacy} or 1.2b1.";
        }
    }

    /// <summary>An example version for the build that moves the users over: the next minor version after the newest package.</summary>
    private (UpdateVersion Version, string Legacy) BridgeVersion
    {
        get
        {
            var newest = NewestPackage?.Version;
            var bridge = newest is null ? new UpdateVersion(1, 2, 0) : new UpdateVersion(newest.Major, newest.Minor + 1, 0);
            return (bridge, $"{bridge.Major}.{bridge.Minor}.0.0");
        }
    }

    public string BridgeRelease
    {
        get
        {
            var tool = Project.LegacyProjectFile is { } file ? $"nUpdate Administration 4 and the project file {file}" : "nUpdate Administration 4";
            var (bridge, bridgeLegacy) = BridgeVersion;
            return $"Installed copies still run nUpdate 4 and only read {LegacyFeed.FileName}. Publish the build from step 2 once with {tool}, under the version it declares written the old way ({bridgeLegacy} for {bridge}). " +
                   $"When your users install it, your application reads {UpdateFeed.FileName} from then on. Publish the same version here as well, and every later version only here.";
        }
    }

    public string KeepOldFiles =>
        $"Leave {LegacyFeed.FileName}, the package folders and {StatisticsScript.LegacyScriptFileName} on the server as long as copies with nUpdate 3 or 4 are in use; the statistics of nUpdate Administration 4 show whether they still download. " +
        "nUpdate Administration 4 keeps working next to this one: it has its own project file and project list, and this administration changes neither. " +
        "Open the projects of nUpdate 5 from this administration: double-clicking a .nupdproj file starts whichever version registered the extension, usually nUpdate Administration 4.";

    public string Retirement =>
        $"When nobody downloads from {LegacyFeed.FileName} any more, choose \"Retire the nUpdate 4 setup…\" on the Overview tab of the project. It lists, then deletes {LegacyFeed.FileName}, the package folders and {StatisticsScript.LegacyScriptFileName} on the server and the package copies of nUpdate Administration 4 on this computer.";

    /// <summary>Called when the window opens: reads the server and prepares the plan. Opens at the last step when nothing is left to migrate.</summary>
    public async Task InitializeAsync()
    {
        if (_plan is not null || _disposed)
            return;
        MigrationPlan? plan = null;
        if (!await RunBusyAsync("Looking at the server...", async progress => plan = await _migrator.PrepareAsync(Project, Secrets, progress)))
        {
            ErrorMessage = "The server could not be read: " + ErrorMessage;
            return;
        }

        if (_disposed)
        {
            // The window was closed while the server was read.
            plan!.Dispose();
            return;
        }

        _plan = plan!;
        Packages.Clear();
        foreach (var package in _plan.Packages)
        {
            var item = new MigrationPackageItemViewModel(package);
            item.PropertyChanged += (_, _) => OnSelectionChanged();
            Packages.Add(item);
        }

        OnPropertyChanged(string.Empty);
        if (_plan.IsComplete)
            Step = SideBySideStep;
    }

    partial void OnStepChanged(int value)
    {
        ErrorMessage = null;
        for (var i = 0; i < Steps.Count; i++)
        {
            Steps[i].IsCurrent = i == value;
            Steps[i].IsDone = i < value;
        }
    }

    private void OnSelectionChanged()
    {
        OnPropertyChanged(nameof(SelectionSummary));
        OnPropertyChanged(nameof(NothingToMigrate));
        OnPropertyChanged(nameof(MigrationSummary));
        OnPropertyChanged(nameof(ContinueText));
    }

    /// <summary>Reads the server again, for example to retry a package that could not be downloaded; the selection starts over.</summary>
    [RelayCommand]
    private Task ReloadAsync()
    {
        _plan?.Dispose();
        _plan = null;
        Packages.Clear();
        OnPropertyChanged(string.Empty);
        return InitializeAsync();
    }

    private bool CanGoBack() => Step is > OverviewStep and < SideBySideStep;

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void Back() => Step--;

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (_plan is null)
        {
            ErrorMessage = "The server has not been read yet. Try again with Reload.";
            return;
        }

        switch (Step)
        {
            case StatisticsStep when StatisticsProblem is { } problem:
                ErrorMessage = problem;
                return;
            case MigrateStep when NothingToMigrate:
                Step = SideBySideStep;
                return;
            case MigrateStep:
                await MigrateAsync();
                return;
            case SideBySideStep:
                Close(true);
                return;
            default:
                Step++;
                return;
        }
    }

    private async Task MigrateAsync()
    {
        IReadOnlyList<UpdateVersion> migrated = [];
        IsMigrating = true;
        try
        {
            if (!await RunTrustingAsync(_dialogs, Project.Transfer, "Migrating...", async progress => migrated = await _migrator.RunAsync(Project, Secrets, _plan!, progress)))
                return;
        }
        finally
        {
            IsMigrating = false;
        }

        MigratedVersions = migrated;
        Migrated = true;
        Step = SideBySideStep;
    }

    /// <summary>Checks the new feed like an nUpdate 5 client and lists what it found.</summary>
    [RelayCommand]
    private async Task CheckFeedAsync()
    {
        FeedCheckResult? result = null;
        CheckSucceeded = null;
        CheckResults.Clear();
        if (!await RunBusyAsync("Checking the new feed...", async progress => result = await _checker.CheckAsync(Project, Secrets, progress)))
        {
            CheckStatus = ErrorMessage!;
            CheckSucceeded = false;
            return;
        }

        if (result!.FeedProblem is { } feedProblem)
            CheckResults.Add("✗ " + feedProblem);
        foreach (var package in result.Packages)
            CheckResults.Add(package.Problem is null ? $"✓ {package.Version} ({package.Platform}): downloaded, size, hash, signature and manifest are fine." : $"✗ {package.Version} ({package.Platform}): {package.Problem}");
        if (result.StatisticsChecked)
            CheckResults.Add(result.StatisticsProblem is null ? $"✓ {StatisticsScript.ScriptFileName} answers." : $"✗ {StatisticsScript.ScriptFileName}: {result.StatisticsProblem}");
        CheckSucceeded = result.Succeeded;
        CheckStatus = result.Succeeded
            ? "An application built with nUpdate 5 can update from this server."
            : "An application built with nUpdate 5 would run into the problems listed below.";
    }

    [RelayCommand]
    private Task CopySnippetAsync() => _clipboard.SetTextAsync(ClientSnippet);

    /// <summary>Deletes the packages downloaded for the review.</summary>
    public void Dispose()
    {
        _disposed = true;
        _plan?.Dispose();
    }

    private static string Count(int count, string noun) => count == 1 ? $"1 {noun}" : $"{count} {noun}s";
}

/// <summary>One package of the migration plan as the assistant lists it.</summary>
public sealed partial class MigrationPackageItemViewModel : ObservableObject
{
    public MigrationPackageItemViewModel(MigrationPackage package)
    {
        Package = package ?? throw new ArgumentNullException(nameof(package));
    }

    public MigrationPackage Package { get; }

    public string Title => $"{Package.LiteralVersion} → {Package.Version}";

    public bool AlreadyMigrated => Package.AlreadyMigrated;

    public bool CanInclude => Package.CanInclude;

    public bool Include
    {
        get => Package.Include;
        set
        {
            if (Package.Include == value)
                return;
            Package.Include = value;
            OnPropertyChanged();
        }
    }

    public string Details
    {
        get
        {
            if (Package.AlreadyMigrated)
                return $"Already in {UpdateFeed.FileName}.";
            if (Package.Problem is not null)
                return Package.Source is null ? "Cannot be migrated." : $"Cannot be migrated from {Package.Source}.";
            var origin = Package.Source!.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? $"downloaded from {Package.Source}" : $"from this computer ({Package.Source})";
            return $"{Count(Package.FileCount, "file")}, {Count(Package.Operations.Count, "operation")}, {ByteSizeFormatter.Format(Package.Size)}, {origin}.";
        }
    }

    /// <summary>The problem, what is left out and why; empty when the package is carried over completely.</summary>
    public IReadOnlyList<string> Notes
    {
        get
        {
            var notes = new List<string>();
            if (Package.Problem is not null)
                notes.Add(Package.Problem);
            notes.AddRange(Package.Warnings);
            if (Package.SkippedEntries.Count > 0)
                notes.Add($"Left out because they are outside the folders the installer knows: {string.Join(", ", Package.SkippedEntries)}.");
            return notes;
        }
    }

    public bool HasNotes => Notes.Count > 0;

    private static string Count(int count, string noun) => count == 1 ? $"1 {noun}" : $"{count} {noun}s";
}
