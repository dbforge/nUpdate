using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Services;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Administration.ViewModels;

/// <summary>How a file of an edited package differs from the package as it was built.</summary>
public enum FileChange
{
    Unchanged,
    Added,
    Changed,
    Removed,
}

/// <summary>
///     A file of the package. A file of an existing package remembers where it was extracted to, so the editor can tell
///     whether it was replaced; a removed one stays listed until the package is saved, so the removal can be undone.
/// </summary>
public sealed partial class PackageFileItem : ObservableObject
{
    private readonly string? _originalSourcePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Change))]
    private string _sourcePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Change))]
    private bool _isRemoved;

    /// <param name="root">The folder on the client the file goes to.</param>
    /// <param name="relativePath">The path below <paramref name="root" />.</param>
    /// <param name="sourcePath">The file on this computer.</param>
    /// <param name="size">The size of the file in bytes, 0 when unknown.</param>
    /// <param name="isOriginal">Whether the file belongs to the existing package being edited.</param>
    /// <param name="unixMode">The Unix permissions the existing package stores for the file, if any.</param>
    public PackageFileItem(PackageRoot root, string relativePath, string sourcePath, long size, bool isOriginal,
        int? unixMode = null)
    {
        Root = root;
        RelativePath = relativePath;
        _sourcePath = sourcePath;
        Size = size;
        _originalSourcePath = isOriginal ? sourcePath : null;
        UnixMode = unixMode;
    }

    public PackageRoot Root { get; }

    public string RelativePath { get; }

    public long Size { get; private set; }

    /// <summary>The Unix permissions the existing package stores for the file; they stay while the file is unchanged.</summary>
    public int? UnixMode { get; }

    public string Display => $"{Root}/{RelativePath}";

    /// <summary>The placeholder an operation uses for the root, such as <c>%program%</c>.</summary>
    public string RootPlaceholder => $"%{Root.ToString().ToLowerInvariant()}%";

    public string SizeText => Size <= 0 ? string.Empty : ByteSizeFormatter.Format(Size, CultureInfo.CurrentCulture);

    public bool IsOriginal => _originalSourcePath is not null;

    public FileChange Change => IsRemoved ? FileChange.Removed
        : !IsOriginal ? FileChange.Added
        : SourcePath == _originalSourcePath ? FileChange.Unchanged
        : FileChange.Changed;

    /// <summary>Takes another file for the same target path.</summary>
    public void Replace(string sourcePath, long size)
    {
        SourcePath = sourcePath;
        Size = size;
        IsRemoved = false;
        OnPropertyChanged(nameof(SizeText));
    }
}

/// <summary>The changelog for one culture.</summary>
public partial class ChangelogItemViewModel(CultureInfo culture) : ViewModelBase
{
    [ObservableProperty] private string _text = string.Empty;

    public CultureInfo Culture { get; } = culture ?? throw new ArgumentNullException(nameof(culture));

    public string Display => Culture.Name == "en" ? "English (required)" : $"{Culture.EnglishName} ({Culture.Name})";

    /// <summary>Every language but the required English can be removed.</summary>
    public bool IsRemovable => Culture.Name != "en";
}

/// <summary>A platform a package file can be built for, with the name the editor shows.</summary>
public sealed record PlatformChoice(string Platform)
{
    public static IReadOnlyList<PlatformChoice> All { get; } =
        PackagePlatform.All.Select(p => new PlatformChoice(p)).ToList();

    private static readonly Dictionary<string, string> Names = new(StringComparer.Ordinal)
    {
        [PackagePlatform.Any] = "Any platform",
        [PackagePlatform.Windows] = "Windows (every architecture)",
        ["win-x64"] = "Windows x64",
        ["win-x86"] = "Windows x86",
        ["win-arm64"] = "Windows ARM64",
        [PackagePlatform.Linux] = "Linux (every architecture)",
        ["linux-x64"] = "Linux x64",
        ["linux-arm64"] = "Linux ARM64",
        [PackagePlatform.MacOS] = "macOS (every architecture)",
        ["osx-x64"] = "macOS Intel",
        ["osx-arm64"] = "macOS Apple silicon",
    };

    /// <summary>The name of the platform, or the platform itself when nUpdate does not know it.</summary>
    public string Name => Names.TryGetValue(Platform, out var name) ? name : Platform;

    public string Display => $"{Name} ({Platform})";
}

/// <summary>What a package asks for after the update, with the name the editor shows.</summary>
public sealed record AfterInstallChoice(AfterInstall? Value, string Display)
{
    public static IReadOnlyList<AfterInstallChoice> All { get; } =
    [
        new(null, "As the application decides"),
        new(AfterInstall.Restart, "Restart the application"),
        new(AfterInstall.Close, "Leave the application closed"),
    ];

    /// <summary>The choice for a value read from the feed, which only allows these three.</summary>
    public static AfterInstallChoice For(AfterInstall? value) => All.Single(choice => choice.Value == value);
}

/// <summary>
///     The files and operations of the package file of one platform. For an existing package it remembers its operations
///     as they were, so the editor knows which platforms to build again.
/// </summary>
public sealed class PlatformItemViewModel(string platform, bool isNew = true)
{
    private string _originalOperations = string.Empty;

    public string Platform { get; } = platform;

    public string Name => new PlatformChoice(Platform).Name;

    public string Display => new PlatformChoice(Platform).Display;

    /// <summary>Whether the platform was added in the editor rather than read from the existing package.</summary>
    public bool IsNew { get; } = isNew;

    /// <summary>Whether the platform's package file has to be built (again): it is new, or its files or operations changed.</summary>
    public bool NeedsBuild => IsNew || Files.Any(f => f.Change != FileChange.Unchanged) || OperationsChanged;

    public bool OperationsChanged => OperationsSignature() != _originalOperations;

    /// <summary>Takes the current operations as the ones the existing package has.</summary>
    public void KeepOperationsAsOriginal() => _originalOperations = OperationsSignature();

    private string OperationsSignature() => string.Join("\n", Operations.Select(o => o.Signature));

    public bool IsWindows => PackagePlatform.IsWindows(Platform);

    public bool IsMacOS => PackagePlatform.OperatingSystemOf(Platform) == PackagePlatform.MacOS;

    public ObservableCollection<PackageFileItem> Files { get; } = [];

    public ObservableCollection<OperationEditorViewModel> Operations { get; } = [];
}

/// <summary>
///     A package to edit: its feed entry and its files and operations, extracted by
///     <see cref="IPublishService.OpenPackageAsync" />. Without the content (its package files are not on this computer)
///     only the feed entry can change.
/// </summary>
public sealed record ExistingPackage(PackageInfo Entry, PackageDefinition? Content);

/// <summary>A path placeholder of operations, with where it points on a platform.</summary>
public sealed record PathPlaceholder(string Name, string Meaning, string Example)
{
    /// <summary>
    ///     The placeholders the installer resolves, explained for a platform; <c>any</c> and other platforms without
    ///     one operating system get no example, since it depends on where the update is installed.
    /// </summary>
    public static IReadOnlyList<PathPlaceholder> For(string platform, string application)
    {
        var system = PackagePlatform.OperatingSystemOf(platform);
        if (system == PackagePlatform.Windows)
            return
            [
                new("%program%", $"The folder of {application}", $@"C:\Program Files\{application}"),
                new("%appdata%", "The roaming application data", @"C:\Users\‹user›\AppData\Roaming"),
                new("%temp%", "The temporary folder", @"C:\Users\‹user›\AppData\Local\Temp"),
                new("%desktop%", "The desktop of the user", @"C:\Users\‹user›\Desktop"),
            ];
        if (system == PackagePlatform.Linux)
            return
            [
                new("%program%", $"The folder of {application}", $"/opt/{application}"),
                new("%appdata%", "The configuration folder of the user ($XDG_CONFIG_HOME)", "~/.config"),
                new("%temp%", "The temporary folder ($TMPDIR)", "/tmp"),
                new("%desktop%", "The desktop of the user", "~/Desktop"),
            ];
        if (system == PackagePlatform.MacOS)
            return
            [
                new("%program%", $"The whole {application}.app bundle", $"/Applications/{application}.app"),
                new("%appdata%", "The configuration folder of the user ($XDG_CONFIG_HOME)", "~/.config"),
                new("%temp%", "The temporary folder of the user", "$TMPDIR"),
                new("%desktop%", "The desktop of the user", "~/Desktop"),
            ];
        return
        [
            new("%program%", $"The folder of {application}; on macOS the whole .app bundle", string.Empty),
            new("%appdata%", "The application data of the user", string.Empty),
            new("%temp%", "The temporary folder", string.Empty),
            new("%desktop%", "The desktop of the user", string.Empty),
        ];
    }
}

/// <summary>A rollout condition row.</summary>
public partial class RolloutConditionItemViewModel : ViewModelBase
{
    [ObservableProperty] private string _key = string.Empty;

    [ObservableProperty] private string _value = string.Empty;

    [ObservableProperty] private bool _isNegative;

    public RolloutCondition ToCondition() => new(Key.Trim(), Value.Trim(), IsNegative);
}

/// <summary>
///     Creates a new package, or edits an existing one. Changing only the feed entry (changelog, conditions and so on)
///     saves the entry; changing files, operations or platforms builds the affected package files again.
/// </summary>
public partial class PackageEditorViewModel : DialogViewModel
{
    private readonly IPublishService _publisher;
    private readonly IFilePickerService _files;
    private readonly IFileSystem _fileSystem;
    private readonly PackageInfo? _existingEntry;
    /// <summary>The platforms of the existing package, to tell which ones were removed.</summary>
    private readonly IReadOnlyList<string> _existingPlatforms = [];
    private PlatformItemViewModel _selectedPlatform;

    [ObservableProperty] private string _version = string.Empty;

    [ObservableProperty] private string _description = string.Empty;

    [ObservableProperty] private bool _necessary;

    /// <summary>Whether the application starts again after this update; see <see cref="UpdateManager.DefaultAfterInstall" />.</summary>
    [ObservableProperty] private AfterInstallChoice _afterUpdate = AfterInstallChoice.All[0];

    [ObservableProperty] private bool _publish = true;

    [ObservableProperty] private bool _includeInStatistics = true;

    [ObservableProperty] private bool _restrictVersions;

    [ObservableProperty] private string _unsupportedVersionsText = string.Empty;

    [ObservableProperty] private RolloutConditionMode _rolloutConditionMode = RolloutConditionMode.Any;

    [ObservableProperty] private string _newCultureName = string.Empty;

    [ObservableProperty] private OperationKind _selectedOperationKind = OperationKind.All[0];

    [ObservableProperty] private OperationEditorViewModel? _selectedOperation;


    /// <summary>The platform "Add platform" adds.</summary>
    [ObservableProperty] private PlatformChoice? _newPlatform;

    [ObservableProperty] private PackageRoot _selectedRoot = PackageRoot.Program;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGeneralSection))]
    [NotifyPropertyChangedFor(nameof(IsChangelogSection))]
    [NotifyPropertyChangedFor(nameof(IsFilesSection))]
    [NotifyPropertyChangedFor(nameof(IsAvailabilitySection))]
    [NotifyPropertyChangedFor(nameof(IsConditionsSection))]
    [NotifyPropertyChangedFor(nameof(IsOperationsSection))]
    private EditorSection _selectedSection;

    /// <summary>A sub folder below the selected root that picked files and folders are placed in.</summary>
    [ObservableProperty] private string _targetFolder = string.Empty;

    public PackageEditorViewModel(IPublishService publisher, IFilePickerService files, IFileSystem fileSystem,
        UpdateProject project, ProjectSecrets secrets, ExistingPackage? existing = null)
    {
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _files = files ?? throw new ArgumentNullException(nameof(files));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        Project = project ?? throw new ArgumentNullException(nameof(project));
        Secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));
        _existingEntry = existing?.Entry;
        Sections = EditorSection.CreateAll(existing is null || existing.Content is not null);
        _selectedSection = Sections[0];
        Changelogs.Add(new ChangelogItemViewModel(new CultureInfo("en")));
        Platforms.CollectionChanged += (_, _) => OnPlatformsChanged();
        Changelogs.CollectionChanged += (_, _) => UpdateHints();
        Conditions.CollectionChanged += (_, _) => UpdateHints();

        if (existing is null)
        {
            Title = $"New package for {project.Name}";
            Version = SuggestVersion(project).ToString();
            Platforms.Add(_selectedPlatform = new PlatformItemViewModel(PackagePlatform.Any));
            UpdateHints();
            return;
        }

        var entry = existing.Entry;
        Title = $"Edit package {entry.Version} of {project.Name}";
        Version = entry.Version.ToString();
        Description = project.FindPackage(entry.Version)?.Description ?? string.Empty;
        Necessary = entry.Necessary;
        AfterUpdate = AfterInstallChoice.For(entry.AfterInstall);
        CanChangeContent = existing.Content is not null;
        if (existing.Content is null)
        {
            foreach (var file in entry.Files)
                Platforms.Add(new PlatformItemViewModel(file.Platform, isNew: false));
        }
        else
        {
            foreach (var content in existing.Content.Platforms)
            {
                var platform = new PlatformItemViewModel(content.Platform, isNew: false);
                foreach (var file in content.Files)
                    platform.Files.Add(new PackageFileItem(file.Root, file.RelativePath, file.SourcePath,
                        SizeOf(file.SourcePath), isOriginal: true, file.UnixMode));
                foreach (var operation in content.Operations)
                    platform.Operations.Add(Track(OperationEditorViewModel.FromOperation(operation)));
                platform.KeepOperationsAsOriginal();
                Platforms.Add(platform);
            }
        }

        // An entry always has a package file; should one have none, the editor still needs a platform to show.
        if (Platforms.Count == 0)
            Platforms.Add(new PlatformItemViewModel(PackagePlatform.Any, isNew: false));
        _existingPlatforms = Platforms.Select(p => p.Platform).ToList();
        _selectedPlatform = Platforms[0];
        SelectedOperation = _selectedPlatform.Operations.FirstOrDefault();
        IncludeInStatistics = entry.Statistics?.Enabled ?? true;
        RestrictVersions = entry.UnsupportedVersions.Count > 0;
        UnsupportedVersionsText = string.Join(Environment.NewLine, entry.UnsupportedVersions);
        RolloutConditionMode = entry.Rollout.Mode;
        foreach (var pair in entry.Changelog)
        {
            var item = Changelogs.FirstOrDefault(c =>
                string.Equals(c.Culture.Name, pair.Key, StringComparison.OrdinalIgnoreCase));
            if (item is null)
                Changelogs.Add(item = new ChangelogItemViewModel(CultureInfo.GetCultureInfo(pair.Key)));
            item.Text = pair.Value;
        }

        foreach (var condition in entry.Rollout.Conditions)
            Conditions.Add(new RolloutConditionItemViewModel
            { Key = condition.Key, Value = condition.Value, IsNegative = condition.Negated });
        UpdateHints();
    }

    public UpdateProject Project { get; }

    public ProjectSecrets Secrets { get; }

    public bool IsEditMode => _existingEntry is not null;

    public bool IsCreateMode => !IsEditMode;

    /// <summary>
    ///     Whether files, operations and platforms can change: always for a new package, for an existing one only when
    ///     its package files are on this computer.
    /// </summary>
    public bool CanChangeContent { get; } = true;

    /// <summary>Why an existing package can only change its feed entry.</summary>
    public string MissingContentNotice =>
        $"The package files of {Version} are not on this computer, so its files, operations and platforms cannot change here. Copy the project's packages folder from the computer that created the package to change them.";

    /// <summary>Whether the edited package is on the server, so saving publishes the changes.</summary>
    public bool IsReleased => _existingEntry is not null && Project.FindPackage(_existingEntry.Version)?.Released == true;

    /// <summary>The state the rail shows under the version.</summary>
    public string State => IsCreateMode ? "New" : IsReleased ? "Released" : "Local only";

    /// <summary>The primary button.</summary>
    public string SaveText => IsCreateMode ? "Create package" : IsReleased ? "Save and publish" : "Save package";

    /// <summary>What saving does to a published package whose files or operations change.</summary>
    public string RebuildNotice =>
        $"{Version} is published. Saving builds the package files you changed again, signs them and replaces them on the server. Clients that already run {Version} keep what they installed.";

    public bool StatisticsAvailable => Project.Statistics.Enabled;

    public ObservableCollection<ChangelogItemViewModel> Changelogs { get; } = [];

    /// <summary>
    ///     The platform whose files and operations the editor shows. A selector writes <c>null</c> back when its selected
    ///     platform is removed; that is ignored, so there is always one.
    /// </summary>
    public PlatformItemViewModel SelectedPlatform
    {
        get => _selectedPlatform;
        set
        {
            if (value is null || !SetProperty(ref _selectedPlatform, value))
                return;
            OnPropertyChanged(nameof(Files));
            OnPropertyChanged(nameof(Operations));
            OnPropertyChanged(nameof(OperationPalette));
            OnPropertyChanged(nameof(Placeholders));
            OnPropertyChanged(nameof(PlaceholdersTitle));
            SelectedOperation = value.Operations.FirstOrDefault();
        }
    }

    /// <summary>The platforms the package has a file for; new packages start with <c>any</c>.</summary>
    public ObservableCollection<PlatformItemViewModel> Platforms { get; } = [];

    /// <summary>The platforms that can still be added.</summary>
    public IReadOnlyList<PlatformChoice> AvailablePlatforms =>
        PlatformChoice.All.Where(c => Platforms.All(p => p.Platform != c.Platform)).ToList();

    /// <summary>Whether the package has more than one platform, so the Files and Operations pages show which one they edit.</summary>
    public bool HasSeveralPlatforms => Platforms.Count > 1;

    /// <summary>The files of the selected platform.</summary>
    public ObservableCollection<PackageFileItem> Files => SelectedPlatform.Files;

    /// <summary>The operations of the selected platform.</summary>
    public ObservableCollection<OperationEditorViewModel> Operations => SelectedPlatform.Operations;

    public ObservableCollection<RolloutConditionItemViewModel> Conditions { get; } = [];

    public IReadOnlyList<RolloutConditionMode> RolloutConditionModes { get; } = Enum.GetValues<RolloutConditionMode>();

    public IReadOnlyList<AfterInstallChoice> AfterInstallChoices => AfterInstallChoice.All;

    public IReadOnlyList<PackageRoot> Roots { get; } = Enum.GetValues<PackageRoot>();

    public IReadOnlyList<OperationKind> OperationKinds => OperationKind.All;

    /// <summary>The operations that can be added; registry and service operations only for Windows platforms.</summary>
    public IReadOnlyList<OperationKind> OperationPalette =>
        OperationKind.All.Where(k => SelectedPlatform.IsWindows || !k.RequiresWindows).ToList();

    /// <summary>The placeholders operations can start their paths with, and where they point on the selected platform.</summary>
    public IReadOnlyList<PathPlaceholder> Placeholders => PathPlaceholder.For(SelectedPlatform.Platform, Project.Name);

    public string PlaceholdersTitle => $"Placeholders on {SelectedPlatform.Name}";

    /// <summary>Whether the selected operation takes a path, so the placeholders apply to it.</summary>
    public bool HasPlaceholders => SelectedOperation?.UsesPaths == true;

    /// <summary>The pages of the editor, listed in its left rail with a short state each.</summary>
    public IReadOnlyList<EditorSection> Sections { get; }

    public bool IsGeneralSection => SelectedSection.Key == "general";

    public bool IsChangelogSection => SelectedSection.Key == "changelog";

    public bool IsFilesSection => SelectedSection.Key == "files";

    public bool IsAvailabilitySection => SelectedSection.Key == "availability";

    public bool IsConditionsSection => SelectedSection.Key == "conditions";

    public bool IsOperationsSection => SelectedSection.Key == "operations";

    /// <summary>
    ///     What the bottom bar summarises: for a new package the platforms, files and operations; for an existing one what
    ///     changes about its package files.
    /// </summary>
    public string Summary
    {
        get
        {
            if (IsEditMode)
                return DescribeChanges();
            var fileCount = Platforms.Sum(p => p.Files.Count);
            var operationCount = Platforms.Sum(p => p.Operations.Count);
            var files = fileCount == 1 ? "1 file" : $"{fileCount} files";
            var operations = operationCount == 1 ? "1 operation" : $"{operationCount} operations";
            var platforms = HasSeveralPlatforms ? $"{Platforms.Count} platforms · " : string.Empty;
            return $"{platforms}{files} · {operations} · English changelog required";
        }
    }

    /// <summary>The platforms the existing package had and the editor no longer has.</summary>
    private IEnumerable<string> RemovedPlatforms => _existingPlatforms.Where(p => Platforms.All(i => i.Platform != p));

    /// <summary>Whether saving an existing package builds package files, rather than only replacing its feed entry.</summary>
    public bool ChangesContent => Platforms.Any(p => p.NeedsBuild) || RemovedPlatforms.Any();

    private string DescribeChanges()
    {
        var parts = new List<string>();
        foreach (var platform in Platforms.Where(p => p.NeedsBuild))
        {
            if (platform.IsNew)
            {
                parts.Add($"{platform.Name}: new");
                continue;
            }

            var changes = new List<string>();
            AddCount(changes, platform.Files.Count(f => f.Change == FileChange.Added), "added");
            AddCount(changes, platform.Files.Count(f => f.Change == FileChange.Changed), "changed");
            AddCount(changes, platform.Files.Count(f => f.Change == FileChange.Removed), "removed");
            if (platform.OperationsChanged)
                changes.Add("operations changed");
            parts.Add($"{platform.Name}: {string.Join(", ", changes)}");
        }

        parts.AddRange(RemovedPlatforms.Select(p => $"{new PlatformChoice(p).Name}: removed"));
        return parts.Count == 0 ? "Files and operations unchanged" : string.Join(" · ", parts);
    }

    private static void AddCount(List<string> changes, int count, string what)
    {
        if (count > 0)
            changes.Add(count == 1 ? $"1 file {what}" : $"{count} files {what}");
    }

    /// <summary>The package that was created, set when the dialog was accepted in create mode.</summary>
    public UpdatePackage? CreatedPackage { get; private set; }

    public string? Validate()
    {
        if (!UpdateVersion.TryParse(Version.Trim(), out var version))
            return
                "Enter a valid version such as 2.1.0, 2.1.0.4 or 2.1.0-beta.1 (major.minor.patch, a fourth number only when it is not 0).";
        if (version!.Release == new UpdateVersion(0, 0, 0))
            return "The version 0.0.0 is reserved.";
        if (IsCreateMode && Project.Packages.Any(p => p.Version == version))
            return $"The project already has a package {version}.";
        if (string.IsNullOrWhiteSpace(Changelogs.First(c => c.Culture.Name == "en").Text))
            return "Enter the English changelog.";
        foreach (var platform in Platforms.Where(p => IsCreateMode || p.NeedsBuild))
        {
            if (platform.Files.All(f => f.IsRemoved) && platform.Operations.Count == 0)
                return HasSeveralPlatforms
                    ? $"Add at least one file or operation for {platform.Display}."
                    : "Add at least one file or operation.";
        }

        if (RestrictVersions && UnsupportedVersions().Any(v => !UpdateVersion.IsValid(v)))
            return "Every unsupported version must be a valid version such as 2.1.0 or 2.1.0-beta.1.";
        if (Conditions.Any(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Value)))
            return "Every rollout condition needs a key and a value.";
        return Platforms.Where(p => IsCreateMode || p.NeedsBuild).SelectMany(p => p.Operations).Select(o => o.Validate())
            .FirstOrDefault(problem => problem is not null);
    }

    [RelayCommand]
    private void AddPlatform()
    {
        if (NewPlatform is null)
            return;
        var platform = new PlatformItemViewModel(NewPlatform.Platform);
        Platforms.Add(platform);
        SelectedPlatform = platform;
        NewPlatform = null;
    }

    /// <summary>Removes a platform with its files and operations; the last one stays.</summary>
    [RelayCommand]
    private void RemovePlatform(PlatformItemViewModel? platform)
    {
        if (platform is null || Platforms.Count == 1)
            return;
        // Select another one first: the platform switchers must never be left with nothing selected.
        if (SelectedPlatform == platform)
            SelectedPlatform = Platforms.First(p => p != platform);
        Platforms.Remove(platform);
    }

    private void OnPlatformsChanged()
    {
        OnPropertyChanged(nameof(AvailablePlatforms));
        OnPropertyChanged(nameof(HasSeveralPlatforms));
        OnPropertyChanged(nameof(Summary));
        UpdateHints();
    }

    partial void OnSelectedOperationChanged(OperationEditorViewModel? value) =>
        OnPropertyChanged(nameof(HasPlaceholders));

    partial void OnRestrictVersionsChanged(bool value) => UpdateHints();

    /// <summary>Refreshes the short state next to each page in the rail.</summary>
    private void UpdateHints()
    {
        SetHint("changelog", Changelogs.Count == 1 ? "English" : $"{Changelogs.Count} languages");
        if (IsEditMode)
        {
            var changes = Platforms.Sum(p => p.Files.Count(f => f.Change != FileChange.Unchanged));
            SetHint("files", changes == 0 ? string.Empty : changes == 1 ? "1 change" : $"{changes} changes");
        }
        else
        {
            var files = Platforms.Sum(p => p.Files.Count);
            SetHint("files", files == 0 ? string.Empty : files == 1 ? "1 file" : $"{files} files");
        }

        var operations = Platforms.Sum(p => p.Operations.Count);
        SetHint("operations", operations == 0 ? string.Empty : operations.ToString(CultureInfo.CurrentCulture));
        SetHint("availability", RestrictVersions ? "Restricted" : string.Empty);
        SetHint("conditions", Conditions.Count == 0 ? "Everyone"
            : Conditions.Count == 1 ? "1 condition" : $"{Conditions.Count} conditions");
    }

    /// <summary>Sets the hint of a page; an existing package without its package files has no Files and Operations pages.</summary>
    private void SetHint(string key, string hint)
    {
        if (Sections.FirstOrDefault(s => s.Key == key) is { } section)
            section.Hint = hint;
    }

    [RelayCommand]
    private void AddCulture()
    {
        var name = NewCultureName.Trim();
        if (name.Length == 0)
            return;
        CultureInfo culture;
        try
        {
            culture = CultureInfo.GetCultureInfo(name);
        }
        catch (CultureNotFoundException)
        {
            ErrorMessage = $"\"{name}\" is not a culture name. Use names like de-DE or fr.";
            return;
        }

        if (Changelogs.Any(c => string.Equals(c.Culture.Name, culture.Name, StringComparison.OrdinalIgnoreCase)))
            return;
        Changelogs.Add(new ChangelogItemViewModel(culture));
        NewCultureName = string.Empty;
        ErrorMessage = null;
    }

    [RelayCommand]
    private void RemoveCulture(ChangelogItemViewModel? item)
    {
        if (item is not null && item.Culture.Name != "en")
            Changelogs.Remove(item);
    }

    /// <summary>
    ///     The next version: the assembly version of the configured executable when it is newer than every package,
    ///     otherwise the highest package version with its revision increased (1.0 for the first package).
    /// </summary>
    public static UpdateVersion SuggestVersion(UpdateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        var highest = project.Packages.Count == 0 ? null : UpdateVersion.Max(project.Packages.Select(p => p.Version));
        var assembly = AssemblyVersionReader.TryRead(project.AssemblyVersionPath);
        var fromAssembly = assembly is null
            ? null
            : new UpdateVersion(assembly.Major, assembly.Minor, Math.Max(0, assembly.Build),
                Math.Max(0, assembly.Revision));
        if (fromAssembly is not null && (highest is null || fromAssembly > highest))
            return fromAssembly;
        return highest is null
            ? new UpdateVersion("1.0.0")
            : new UpdateVersion(highest.Major, highest.Minor, highest.Build, highest.Revision + 1);
    }

    [RelayCommand]
    private async Task AddFilesAsync()
    {
        var folder = TargetFolder.Trim().Trim('/', '\\').Replace('\\', '/');
        var paths = await _files.PickFilesAsync("Add files to " + SelectedRoot, FileTypeFilter.All);
        if (paths.FirstOrDefault(IsLink) is { } link)
        {
            ErrorMessage = LinkMessage(link);
            return;
        }

        foreach (var path in paths)
        {
            var name = _fileSystem.Path.GetFileName(path);
            AddFile(SelectedRoot, folder.Length == 0 ? name : $"{folder}/{name}", path);
        }
    }

    /// <summary>
    ///     Adds a folder with everything below it. On a macOS platform an <c>.app</c> folder added to <c>Program</c>
    ///     without a sub folder becomes the bundle itself, since <c>Program</c> is the bundle there.
    /// </summary>
    [RelayCommand]
    private async Task AddFolderAsync()
    {
        var folder = await _files.PickFolderAsync("Add a folder to " + SelectedRoot);
        if (folder is null)
            return;
        var name = _fileSystem.Path.GetFileName(folder.TrimEnd(_fileSystem.Path.DirectorySeparatorChar,
            _fileSystem.Path.AltDirectorySeparatorChar));
        var target = TargetFolder.Trim().Trim('/', '\\').Replace('\\', '/');
        var isBundle = SelectedPlatform.IsMacOS && SelectedRoot == PackageRoot.Program && target.Length == 0 &&
                       name.EndsWith(".app", StringComparison.OrdinalIgnoreCase);
        var prefix = isBundle ? string.Empty : target.Length > 0 ? $"{target}/{name}/" : $"{name}/";
        string[] entries;
        try
        {
            entries = await Task.Run(() =>
                _fileSystem.Directory.GetFileSystemEntries(folder, "*", SearchOption.AllDirectories));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ErrorMessage = ex.Message;
            return;
        }

        if (entries.Prepend(folder).FirstOrDefault(IsLink) is { } link)
        {
            ErrorMessage = LinkMessage(link);
            return;
        }

        foreach (var file in entries.Where(_fileSystem.File.Exists))
        {
            var relative = _fileSystem.Path.GetRelativePath(folder, file).Replace('\\', '/');
            AddFile(SelectedRoot, prefix + relative, file);
        }
    }

    private bool IsLink(string path) =>
        (_fileSystem.Directory.Exists(path)
            ? _fileSystem.DirectoryInfo.New(path).LinkTarget
            : _fileSystem.FileInfo.New(path).LinkTarget) is not null;

    private static string LinkMessage(string path) =>
        $"\"{path}\" is a symbolic link. Packages cannot contain links; add the file or folder it points to instead.";

    private long SizeOf(string path) => _fileSystem.File.Exists(path) ? _fileSystem.FileInfo.New(path).Length : 0;

    /// <summary>
    ///     Adds a file. A file with the same target path is replaced: a file of the existing package then counts as
    ///     changed, any other one is swapped for the new one.
    /// </summary>
    public void AddFile(PackageRoot root, string relativePath, string sourcePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);
        ArgumentNullException.ThrowIfNull(sourcePath);
        var normalized = relativePath.Replace('\\', '/').TrimStart('/');
        var existing = Files.FirstOrDefault(f =>
            f.Root == root && string.Equals(f.RelativePath, normalized, StringComparison.OrdinalIgnoreCase));
        if (existing is { IsOriginal: true })
        {
            existing.Replace(sourcePath, SizeOf(sourcePath));
        }
        else
        {
            if (existing is not null)
                Files.Remove(existing);
            Files.Add(new PackageFileItem(root, normalized, sourcePath, SizeOf(sourcePath), isOriginal: false));
        }

        OnFilesChanged();
    }

    /// <summary>Removes a file; a file of the existing package stays listed as removed until saved, so it can be kept after all.</summary>
    [RelayCommand]
    private void RemoveFile(PackageFileItem? item)
    {
        if (item is null)
            return;
        if (item.IsOriginal)
            item.IsRemoved = true;
        else
            Files.Remove(item);
        OnFilesChanged();
    }

    /// <summary>Undoes the removal of a file of the existing package.</summary>
    [RelayCommand]
    private void KeepFile(PackageFileItem? item)
    {
        if (item is null)
            return;
        item.IsRemoved = false;
        OnFilesChanged();
    }

    private void OnFilesChanged()
    {
        OnPropertyChanged(nameof(Summary));
        UpdateHints();
    }

    [RelayCommand]
    private void AddOperation() => AddOperationOfKind(SelectedOperationKind);

    /// <summary>Adds an operation from the palette and selects it.</summary>
    [RelayCommand]
    private void AddOperationOfKind(OperationKind? kind)
    {
        if (kind is null)
            return;
        var editor = Track(new OperationEditorViewModel(kind));
        Operations.Add(editor);
        SelectedOperation = editor;
        OnOperationsChanged();
    }

    /// <summary>Keeps the summary current while an operation is filled in.</summary>
    private OperationEditorViewModel Track(OperationEditorViewModel editor)
    {
        editor.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Summary));
        return editor;
    }

    [RelayCommand]
    private void RemoveOperation(OperationEditorViewModel? editor)
    {
        if (editor is null)
            return;
        Operations.Remove(editor);
        if (SelectedOperation == editor)
            SelectedOperation = Operations.LastOrDefault();
        OnOperationsChanged();
    }

    /// <summary>Operations run top to bottom; these move one up or down.</summary>
    [RelayCommand]
    private void MoveOperationUp(OperationEditorViewModel? editor) => MoveOperation(editor, -1);

    [RelayCommand]
    private void MoveOperationDown(OperationEditorViewModel? editor) => MoveOperation(editor, 1);

    private void MoveOperation(OperationEditorViewModel? editor, int offset)
    {
        if (editor is null)
            return;
        var index = Operations.IndexOf(editor);
        var target = index + offset;
        if (index < 0 || target < 0 || target >= Operations.Count)
            return;
        Operations.Move(index, target);
        SelectedOperation = editor;
        OnOperationsChanged();
    }

    private void OnOperationsChanged()
    {
        OnPropertyChanged(nameof(Summary));
        UpdateHints();
    }

    /// <summary>Puts a placeholder in front of the selected operation's path.</summary>
    [RelayCommand]
    private void InsertPlaceholder(PathPlaceholder? placeholder)
    {
        if (placeholder is not null)
            SelectedOperation?.InsertPlaceholder(placeholder.Name);
    }

    [RelayCommand]
    private void AddCondition() => Conditions.Add(new RolloutConditionItemViewModel());

    [RelayCommand]
    private void RemoveCondition(RolloutConditionItemViewModel? item)
    {
        if (item is not null)
            Conditions.Remove(item);
    }

    /// <summary>
    ///     Creates the package; for an existing one builds the changed package files again, or only replaces the feed
    ///     entry when no file, operation or platform changed.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = Validate();
        if (ErrorMessage is not null)
            return;

        bool ok;
        if (IsCreateMode)
            ok = await RunBusyAsync("Creating the package...",
                async progress => CreatedPackage = await _publisher.CreatePackageAsync(BuildRequest(), progress));
        else if (ChangesContent)
            ok = await RunBusyAsync(IsReleased ? "Building and publishing the package again..." : "Building the package again...",
                progress => _publisher.RebuildPackageAsync(BuildRequest(),
                    Platforms.Where(p => p.NeedsBuild).Select(p => p.Platform).ToList(), progress));
        else
            ok = await RunBusyAsync("Saving the package...",
                progress => _publisher.UpdateEntryAsync(Project, Secrets, BuildEditedEntry(), progress));
        if (ok)
            Close(true);
    }

    /// <summary>The request for the package, built from the editor's state; removed files are left out.</summary>
    public PublishRequest BuildRequest()
    {
        var definition = new PackageDefinition(new UpdateVersion(Version.Trim()));
        foreach (var platform in Platforms)
        {
            var package = definition.GetOrAddPlatform(platform.Platform);
            foreach (var file in platform.Files.Where(f => !f.IsRemoved))
                package.Files.Add(new PackageFileEntry(file.Root, file.RelativePath, file.SourcePath)
                { UnixMode = file.Change == FileChange.Unchanged ? file.UnixMode : null });
            foreach (var operation in platform.Operations)
                package.Operations.Add(operation.ToOperation());
        }

        var request = new PublishRequest(Project, Secrets, definition)
        {
            Description = Description.Trim(),
            Necessary = Necessary,
            AfterInstall = AfterUpdate.Value,
            Publish = Publish,
            IncludeInStatistics = IncludeInStatistics,
            RolloutConditionMode = RolloutConditionMode,
        };
        if (RestrictVersions)
            request.UnsupportedVersions.AddRange(UnsupportedVersions().Select(v => new UpdateVersion(v)));
        foreach (var changelog in Changelogs.Where(c => !string.IsNullOrWhiteSpace(c.Text)))
            request.Changelog[changelog.Culture] = changelog.Text.Trim();
        foreach (var condition in Conditions)
            request.RolloutConditions.Add(condition.ToCondition());
        return request;
    }

    /// <summary>The existing entry with the edited metadata applied.</summary>
    public PackageInfo BuildEditedEntry()
    {
        var entry = _existingEntry ?? throw new InvalidOperationException("The editor is not in edit mode.");
        entry.Necessary = Necessary;
        entry.AfterInstall = AfterUpdate.Value;
        entry.Statistics = Project.Statistics.Enabled
            ? new PackageStatistics { Url = PublishService.StatisticsUrl(Project), Enabled = IncludeInStatistics }
            : null;
        entry.UnsupportedVersions =
            RestrictVersions ? UnsupportedVersions().Select(v => new UpdateVersion(v)).ToList() : [];
        entry.Rollout = new RolloutSettings
        { Mode = RolloutConditionMode, Conditions = Conditions.Select(c => c.ToCondition()).ToList() };
        entry.Changelog = Changelogs.Where(c => !string.IsNullOrWhiteSpace(c.Text))
            .ToDictionary(c => c.Culture.Name, c => c.Text.Trim(), StringComparer.OrdinalIgnoreCase);
        var package = Project.FindPackage(entry.Version);
        if (package is not null)
            package.Description = Description.Trim();
        return entry;
    }

    private string[] UnsupportedVersions() => UnsupportedVersionsText.Split(['\r', '\n', ',', ';'],
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

/// <summary>A page of the package editor, listed in its left rail with a short state such as "2 languages".</summary>
public sealed partial class EditorSection(string key, string title, string icon) : ObservableObject
{
    [ObservableProperty] private string _hint = string.Empty;

    public string Key { get; } = key;

    public string Title { get; } = title;

    /// <summary>The key of the icon geometry in App.axaml.</summary>
    public string Icon { get; } = icon;

    /// <summary>The pages in the order of the rail; each editor has its own, since the hints differ.</summary>
    /// <param name="withContent">Whether to include Files and Operations, which need the package files.</param>
    public static IReadOnlyList<EditorSection> CreateAll(bool withContent = true) =>
    [
        new("general", "General", "IconSettings"),
        new("changelog", "Changelog", "IconChangelog"),
        .. withContent
            ? new EditorSection[] { new("files", "Files", "IconFile"), new("operations", "Operations", "IconOperations") }
            : [],
        new("availability", "Availability", "IconAvailability"),
        new("conditions", "Rollout", "IconGlobe"),
    ];
}
