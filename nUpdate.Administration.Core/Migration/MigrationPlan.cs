using nUpdate.Operations;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Migration;

/// <summary>
///     What a migration would do, prepared before anything is changed: every package of the legacy feed with its new
///     version, where its old zip comes from, what it contains and what cannot be carried over. Packages that were
///     downloaded for the review are kept in a temporary folder until the plan is disposed, so the migration does not
///     download them again.
/// </summary>
public sealed class MigrationPlan : IDisposable
{
    private readonly Action? _cleanup;
    private bool _disposed;

    public MigrationPlan(Guid projectId, bool legacyFeedPresent, UpdateFeed? existingFeed, IReadOnlyList<MigrationPackage> packages, Action? cleanup = null, IReadOnlyList<string>? unreadableVersions = null)
    {
        ProjectId = projectId;
        LegacyFeedPresent = legacyFeedPresent;
        ExistingFeed = existingFeed;
        Packages = packages ?? throw new ArgumentNullException(nameof(packages));
        _cleanup = cleanup;
        UnreadableVersions = unreadableVersions ?? [];
    }

    /// <summary>Entries of <c>updates.json</c> whose version cannot be read; they are left out.</summary>
    public IReadOnlyList<string> UnreadableVersions { get; }

    public Guid ProjectId { get; }

    /// <summary>The server has the <c>updates.json</c> of nUpdate 3 or 4.</summary>
    public bool LegacyFeedPresent { get; }

    /// <summary>The <c>nupdate.json</c> on the server, or <c>null</c> when there is none yet.</summary>
    public UpdateFeed? ExistingFeed { get; }

    /// <summary>Every package of the legacy feed, in version order.</summary>
    public IReadOnlyList<MigrationPackage> Packages { get; }

    /// <summary>The packages of the legacy feed that are not in <c>nupdate.json</c> yet.</summary>
    public IEnumerable<MigrationPackage> Pending => Packages.Where(p => !p.AlreadyMigrated);

    /// <summary>The packages the migration will repack and upload.</summary>
    public IEnumerable<MigrationPackage> Included => Packages.Where(p => p.Include);

    /// <summary>Packages of the legacy feed report their downloads to the <c>statistics.php</c> of nUpdate 3 and 4.</summary>
    public bool LegacyStatisticsUsed => Packages.Any(p => p.Legacy.UseStatistics);

    /// <summary>Nothing is left to do: <c>nupdate.json</c> exists and lists every package of the legacy feed.</summary>
    public bool IsComplete => ExistingFeed is not null && Packages.All(p => p.AlreadyMigrated);

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _cleanup?.Invoke();
    }
}

/// <summary>One package of the legacy feed as the migration would carry it over.</summary>
public sealed class MigrationPackage
{
    private bool _include;

    private MigrationPackage(LegacyFeedEntry legacy)
    {
        Legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));
    }

    /// <summary>A package that is in <c>nupdate.json</c> already.</summary>
    public static MigrationPackage Migrated(LegacyFeedEntry legacy) => new(legacy) { AlreadyMigrated = true };

    /// <summary>A package whose old zip could not be found or read.</summary>
    public static MigrationPackage Failed(LegacyFeedEntry legacy, string? source, string problem)
    {
        ArgumentException.ThrowIfNullOrEmpty(problem);
        return new MigrationPackage(legacy) { Source = source, Problem = problem };
    }

    /// <summary>A package that can be migrated; it is included unless the user deselects it.</summary>
    public static MigrationPackage Ready(LegacyFeedEntry legacy, string source, string sourcePath, long size, int fileCount, IReadOnlyList<string> skippedEntries, LegacyOperationConversion operations)
    {
        ArgumentException.ThrowIfNullOrEmpty(source);
        ArgumentException.ThrowIfNullOrEmpty(sourcePath);
        ArgumentNullException.ThrowIfNull(skippedEntries);
        ArgumentNullException.ThrowIfNull(operations);
        return new MigrationPackage(legacy)
        {
            Source = source,
            SourcePath = sourcePath,
            Size = size,
            FileCount = fileCount,
            SkippedEntries = skippedEntries,
            Operations = operations.Operations,
            Warnings = operations.Warnings,
            _include = true,
        };
    }

    public LegacyFeedEntry Legacy { get; }

    /// <summary>The canonical version the package gets.</summary>
    public UpdateVersion Version => Legacy.Version;

    /// <summary>The version as nUpdate 3 or 4 wrote it.</summary>
    public string LiteralVersion => Legacy.LiteralVersion;

    public bool AlreadyMigrated { get; private init; }

    /// <summary>Whether the migration carries the package over; only packages that are ready can be included.</summary>
    public bool Include
    {
        get => _include;
        set => _include = value && CanInclude;
    }

    public bool CanInclude => !AlreadyMigrated && Problem is null;

    /// <summary>Where the old zip comes from: the local copy or the URL it was downloaded from.</summary>
    public string? Source { get; private init; }

    /// <summary>The old zip on this computer: the local copy or the downloaded file.</summary>
    public string? SourcePath { get; private init; }

    /// <summary>The size of the old zip in bytes.</summary>
    public long Size { get; private init; }

    /// <summary>The files below the known root folders, which go into the new package.</summary>
    public int FileCount { get; private init; }

    /// <summary>Entries of the old zip outside the known root folders, which the installer never used and the new package leaves out.</summary>
    public IReadOnlyList<string> SkippedEntries { get; private init; } = [];

    public IReadOnlyList<Operation> Operations { get; private init; } = [];

    /// <summary>Operations or registry values of the old package that cannot be carried over.</summary>
    public IReadOnlyList<string> Warnings { get; private init; } = [];

    /// <summary>Why the package cannot be migrated, or <c>null</c>.</summary>
    public string? Problem { get; private init; }
}

/// <summary>What nUpdate 3 and 4 left for a project on the server and on this computer.</summary>
public sealed class LegacyFiles
{
    public LegacyFiles(IReadOnlyList<string> serverFiles, IReadOnlyList<string> serverDirectories, IReadOnlyList<string> localDirectories)
    {
        ServerFiles = serverFiles ?? throw new ArgumentNullException(nameof(serverFiles));
        ServerDirectories = serverDirectories ?? throw new ArgumentNullException(nameof(serverDirectories));
        LocalDirectories = localDirectories ?? throw new ArgumentNullException(nameof(localDirectories));
    }

    /// <summary><c>updates.json</c> and <c>statistics.php</c>, relative to the transfer directory.</summary>
    public IReadOnlyList<string> ServerFiles { get; }

    /// <summary>The package folders named after the old version spelling.</summary>
    public IReadOnlyList<string> ServerDirectories { get; }

    /// <summary>The local package copies of nUpdate Administration 4 and of the 5.0 pre-releases.</summary>
    public IReadOnlyList<string> LocalDirectories { get; }

    public bool IsEmpty => ServerFiles.Count == 0 && ServerDirectories.Count == 0 && LocalDirectories.Count == 0;

    /// <summary>The same files without the local ones.</summary>
    public LegacyFiles ServerOnly() => new(ServerFiles, ServerDirectories, []);
}
