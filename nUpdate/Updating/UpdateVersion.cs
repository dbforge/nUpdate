using System.Globalization;
using System.Text.RegularExpressions;

namespace nUpdate.Updating;

/// <summary>
///     An immutable version as in SemVer 2.0 with an optional fourth number: <c>major.minor.patch</c>, then the
///     revision when it is not zero, an optional pre-release label after a dash and optional build metadata after a
///     plus sign (<c>2.1.0</c>, <c>2.1.0.4</c>, <c>2.1.0-beta.3</c>, <c>2.0.0-rc.1+build.7</c>). There is exactly one
///     string form, <see cref="ToString" />; it is the only form that parses and what every file, folder and feed
///     carries. The spellings of nUpdate 3 and 4 (<c>1.2.0.0b3</c>) are converted by the migration of nUpdate
///     Administration, not here.
/// </summary>
public sealed class UpdateVersion : IComparable<UpdateVersion>, IComparable, IEquatable<UpdateVersion>
{
    /// <summary>What the parser accepts, as a sentence for error messages.</summary>
    public const string FormatDescription =
        "Write it as major.minor.patch, optionally followed by a fourth number other than 0, a pre-release label after a dash and build metadata after a plus sign, for example 2.1.0, 2.1.0.4, 2.1.0-beta.3 or 2.1.0-rc.1+build.7.";

    private const string Number = "(0|[1-9][0-9]*)";

    /// <summary>A pre-release identifier: a number without leading zeros, or letters, digits and hyphens with at least one non-digit.</summary>
    private const string PreReleaseIdentifier = "(0|[1-9][0-9]*|[0-9]*[A-Za-z-][0-9A-Za-z-]*)";

    private const string MetadataIdentifier = "[0-9A-Za-z-]+";

    private static readonly Regex VersionPattern = new(
        "^(?<Major>" + Number + @")\.(?<Minor>" + Number + @")\.(?<Build>" + Number + @")(\.(?<Revision>[1-9][0-9]*))?" +
        "(-(?<Pre>" + PreReleaseIdentifier + @"(\." + PreReleaseIdentifier + ")*))?" +
        @"(\+(?<Meta>" + MetadataIdentifier + @"(\." + MetadataIdentifier + ")*))?$",
        RegexOptions.CultureInvariant);

    private static readonly Regex PreReleasePattern = new("^" + PreReleaseIdentifier + @"(\." + PreReleaseIdentifier + ")*$", RegexOptions.CultureInvariant);

    private static readonly Regex MetadataPattern = new("^" + MetadataIdentifier + @"(\." + MetadataIdentifier + ")*$", RegexOptions.CultureInvariant);

    /// <summary>Initializes the version <c>0.0.0</c>.</summary>
    public UpdateVersion()
        : this(0, 0, 0, 0, null, null)
    {
    }

    /// <summary>Parses the canonical form of a version, such as <c>1.2.0</c>, <c>1.2.0.3</c>, <c>1.2.0-rc.1</c> or <c>1.2.0-beta.4+exp</c>.</summary>
    /// <exception cref="ArgumentException">The string is not a version in the canonical form.</exception>
    public UpdateVersion(string version)
    {
        if (version is null)
            throw new ArgumentNullException(nameof(version));
        if (!TryParseCore(version, out var parsed))
            throw new ArgumentException($"\"{version}\" is not a valid version. {FormatDescription}", nameof(version));

        Major = parsed.Major;
        Minor = parsed.Minor;
        Build = parsed.Build;
        Revision = parsed.Revision;
        PreRelease = parsed.PreRelease;
        BuildMetadata = parsed.BuildMetadata;
    }

    public UpdateVersion(int major, int minor, int build, int revision = 0)
        : this(major, minor, build, revision, null, null)
    {
    }

    /// <summary>Creates a version with a pre-release label (<c>beta.1</c>) and build metadata (<c>build.7</c>), both as SemVer 2.0 defines them.</summary>
    /// <exception cref="ArgumentException">
    ///     A label is not made of dot-separated identifiers of letters, digits and hyphens, or a numeric pre-release
    ///     identifier has a leading zero.
    /// </exception>
    public UpdateVersion(int major, int minor, int build, int revision, string? preRelease, string? buildMetadata)
    {
        Major = NonNegative(major, nameof(major));
        Minor = NonNegative(minor, nameof(minor));
        Build = NonNegative(build, nameof(build));
        Revision = NonNegative(revision, nameof(revision));
        PreRelease = Validate(preRelease, PreReleasePattern, "pre-release label: use dot-separated identifiers of letters, digits and hyphens, numbers without leading zeros", nameof(preRelease));
        BuildMetadata = Validate(buildMetadata, MetadataPattern, "build metadata: use dot-separated identifiers of letters, digits and hyphens", nameof(buildMetadata));
    }

    public int Major { get; }

    public int Minor { get; }

    public int Build { get; }

    public int Revision { get; }

    /// <summary>The pre-release identifiers joined with dots (<c>beta.3</c>, <c>rc</c>, <c>nightly.20</c>), or <c>null</c> for a release.</summary>
    public string? PreRelease { get; }

    /// <summary>The build metadata after the plus sign, or <c>null</c>. It takes no part in ordering or equality.</summary>
    public string? BuildMetadata { get; }

    public bool IsPreRelease => PreRelease is null ? false : true;

    /// <summary>The version without its pre-release label and metadata: the release a pre-release leads up to.</summary>
    public UpdateVersion Release => PreRelease is null && BuildMetadata is null ? this : new UpdateVersion(Major, Minor, Build, Revision);

    /// <summary>The stage nUpdate recognises in the first identifier of the label.</summary>
    internal PreReleaseStage Stage
    {
        get
        {
            if (PreRelease is null)
                return PreReleaseStage.None;
            var first = FirstIdentifier(PreRelease);
            if (first.Equals("alpha", StringComparison.OrdinalIgnoreCase))
                return PreReleaseStage.Alpha;
            if (first.Equals("beta", StringComparison.OrdinalIgnoreCase))
                return PreReleaseStage.Beta;
            if (first.Equals("rc", StringComparison.OrdinalIgnoreCase))
                return PreReleaseStage.ReleaseCandidate;
            return PreReleaseStage.Other;
        }
    }

    /// <summary>Parses the canonical form of a version. Returns <c>false</c> instead of throwing for anything else.</summary>
    public static bool TryParse(string? version, out UpdateVersion? result)
    {
        result = null;
        if (version is null || !TryParseCore(version, out var parsed))
            return false;
        result = new UpdateVersion(parsed.Major, parsed.Minor, parsed.Build, parsed.Revision, parsed.PreRelease, parsed.BuildMetadata);
        return true;
    }

    public static bool IsValid(string? version) => version is not null && TryParseCore(version, out _);

    /// <summary>Returns the highest version, or <c>0.0.0</c> for an empty sequence.</summary>
    public static UpdateVersion Max(IEnumerable<UpdateVersion> versions)
    {
        if (versions is null)
            throw new ArgumentNullException(nameof(versions));
        var highest = new UpdateVersion();
        foreach (var version in versions)
        {
            if (version > highest)
                highest = version;
        }

        return highest;
    }

    /// <summary>Returns the lowest version, or <c>0.0.0</c> for an empty sequence.</summary>
    public static UpdateVersion Min(IEnumerable<UpdateVersion> versions)
    {
        if (versions is null)
            throw new ArgumentNullException(nameof(versions));
        UpdateVersion? lowest = null;
        foreach (var version in versions)
        {
            if (lowest is null || version < lowest)
                lowest = version;
        }

        return lowest ?? new UpdateVersion();
    }

    public int CompareTo(UpdateVersion? other)
    {
        if (other is null)
            return 1;

        var result = Major.CompareTo(other.Major);
        if (result != 0)
            return result;
        result = Minor.CompareTo(other.Minor);
        if (result != 0)
            return result;
        result = Build.CompareTo(other.Build);
        if (result != 0)
            return result;
        result = Revision.CompareTo(other.Revision);
        if (result != 0)
            return result;

        // A release is newer than any of its pre-releases; two pre-releases compare identifier by identifier.
        if (PreRelease is null)
            return other.PreRelease is null ? 0 : 1;
        if (other.PreRelease is null)
            return -1;
        return ComparePreRelease(PreRelease, other.PreRelease);
    }

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        UpdateVersion other => CompareTo(other),
        _ => throw new ArgumentException($"Object must be of type {nameof(UpdateVersion)}.", nameof(obj)),
    };

    public bool Equals(UpdateVersion? other) => other is not null && CompareTo(other) == 0;

    public override bool Equals(object? obj) => Equals(obj as UpdateVersion);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + Major;
            hash = (hash * 31) + Minor;
            hash = (hash * 31) + Build;
            hash = (hash * 31) + Revision;
            hash = (hash * 31) + (PreRelease is null ? 0 : StringComparer.Ordinal.GetHashCode(PreRelease));
            return hash;
        }
    }

    /// <summary>
    ///     The canonical form: <c>major.minor.build</c>, the revision only when it is not zero, then <c>-label</c> and
    ///     <c>+metadata</c> as written. Parses back to an equal version.
    /// </summary>
    public override string ToString()
    {
        var numbers = Revision == 0
            ? string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Major, Minor, Build)
            : string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
        var label = PreRelease is null ? string.Empty : "-" + PreRelease;
        var metadata = BuildMetadata is null ? string.Empty : "+" + BuildMetadata;
        return numbers + label + metadata;
    }

    public static bool operator ==(UpdateVersion? left, UpdateVersion? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(UpdateVersion? left, UpdateVersion? right) => !(left == right);

    public static bool operator <(UpdateVersion? left, UpdateVersion? right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(UpdateVersion? left, UpdateVersion? right) =>
        left is null || left.CompareTo(right) <= 0;

    public static bool operator >(UpdateVersion? left, UpdateVersion? right) =>
        left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(UpdateVersion? left, UpdateVersion? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    private static bool TryParseCore(string version, out (int Major, int Minor, int Build, int Revision, string? PreRelease, string? BuildMetadata) parsed)
    {
        parsed = default;
        var match = VersionPattern.Match(version);
        if (!match.Success)
            return false;

        if (!TryPart(match.Groups["Major"], out var major) || !TryPart(match.Groups["Minor"], out var minor)
            || !TryPart(match.Groups["Build"], out var build) || !TryPart(match.Groups["Revision"], out var revision))
            return false;

        var pre = match.Groups["Pre"].Success ? match.Groups["Pre"].Value : null;
        var meta = match.Groups["Meta"].Success ? match.Groups["Meta"].Value : null;
        parsed = (major, minor, build, revision, pre, meta);
        return true;
    }

    private static bool TryPart(Group group, out int value)
    {
        value = 0;
        return !group.Success || int.TryParse(group.Value, NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }

    private static string? Validate(string? label, Regex pattern, string what, string parameterName)
    {
        if (label is null || pattern.IsMatch(label))
            return label;
        throw new ArgumentException($"\"{label}\" is not a valid {what}.", parameterName);
    }

    private static string FirstIdentifier(string label)
    {
        var dot = label.IndexOf('.');
        return dot < 0 ? label : label.Substring(0, dot);
    }

    /// <summary>SemVer 2.0 precedence: numeric identifiers compare numerically and rank below alphanumeric ones; a shorter list ranks lower.</summary>
    private static int ComparePreRelease(string left, string right)
    {
        var a = left.Split('.');
        var b = right.Split('.');
        for (var i = 0; i < Math.Min(a.Length, b.Length); i++)
        {
            var aNumeric = long.TryParse(a[i], NumberStyles.None, CultureInfo.InvariantCulture, out var aValue);
            var bNumeric = long.TryParse(b[i], NumberStyles.None, CultureInfo.InvariantCulture, out var bValue);
            var result = aNumeric && bNumeric ? aValue.CompareTo(bValue)
                : aNumeric ? -1
                : bNumeric ? 1
                : string.CompareOrdinal(a[i], b[i]);
            if (result != 0)
                return Math.Sign(result);
        }

        return a.Length.CompareTo(b.Length);
    }

    private static int NonNegative(int value, string parameterName) =>
        value < 0 ? throw new ArgumentOutOfRangeException(parameterName, "The value must be 0 or higher.") : value;
}
