namespace nUpdate.Updating;

/// <summary>What a client is and accepts, for <see cref="UpdateFilter" />.</summary>
internal sealed class UpdateFilterOptions
{
    public UpdateFilterOptions(UpdateVersion currentVersion)
    {
        CurrentVersion = currentVersion ?? throw new ArgumentNullException(nameof(currentVersion));
    }

    public UpdateVersion CurrentVersion { get; }

    public Stability MinimumStability { get; set; }

    public IReadOnlyCollection<string> AcceptedPreReleaseLabels { get; set; } = [];

    public IReadOnlyDictionary<string, string> RolloutConditions { get; set; } = new Dictionary<string, string>();

    /// <summary>The runtime identifier of the client; packages without a file for it are left out.</summary>
    public string Platform { get; set; } = PackagePlatform.Any;
}

/// <summary>Picks the packages a client installs: the newest acceptable one plus every necessary one in between, oldest first.</summary>
internal static class UpdateFilter
{
    public static IReadOnlyList<PackageInfo> Select(IEnumerable<PackageInfo> packages, UpdateFilterOptions options)
    {
        if (packages is null)
            throw new ArgumentNullException(nameof(packages));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        var current = options.CurrentVersion;
        var candidates = packages.Where(package =>
            package.Version > current
            && IsAccepted(package.Version, options)
            && !package.UnsupportedVersions.Any(unsupported => unsupported.Release == current.Release)
            && package.FindFile(options.Platform) is not null
            && RolloutConditionEvaluator.Matches(package.Rollout, options.RolloutConditions)).ToList();
        if (candidates.Count == 0)
            return [];

        var highest = UpdateVersion.Max(candidates.Select(c => c.Version));
        return candidates.Where(c => c.Version == highest || c.Necessary).OrderBy(c => c.Version).ToList();
    }

    private static bool IsAccepted(UpdateVersion version, UpdateFilterOptions options)
    {
        var stage = version.Stage;
        if (stage == PreReleaseStage.None)
            return true;
        var first = version.PreRelease!.Split('.')[0];
        if (options.AcceptedPreReleaseLabels.Contains(first, StringComparer.OrdinalIgnoreCase))
            return true;
        return stage switch
        {
            PreReleaseStage.ReleaseCandidate => options.MinimumStability >= Stability.ReleaseCandidate,
            PreReleaseStage.Beta => options.MinimumStability >= Stability.Beta,
            _ => options.MinimumStability == Stability.Any,
        };
    }
}
