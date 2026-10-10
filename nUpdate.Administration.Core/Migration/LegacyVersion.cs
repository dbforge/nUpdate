using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Migration;

/// <summary>
///     Reads the version spellings of nUpdate 3 and 4 and of the 5.0 pre-releases, which <see cref="UpdateVersion" />
///     no longer accepts, and turns them into the canonical version: one to four numbers (<c>1.2</c>,
///     <c>1.2.0.0</c>), a stage glued to them or after a dash or a space (<c>1.2.0.0b3</c>, <c>1.2-rc.1</c>,
///     <c>1.2 a</c>), the long form nUpdate 4 showed (<c>1.2.0.0 Beta 3</c>) and any SemVer label
///     (<c>1.2.0.0-beta.3</c>). All of these examples become <c>1.2.0-beta.3</c> or <c>1.2.0-alpha</c>.
/// </summary>
public static class LegacyVersion
{
    private const string Identifier = "[0-9A-Za-z-]+";

    private static readonly Regex ShortForm = new(
        @"^(?<Numbers>[0-9]+(\.[0-9]+){0,3})" +
        @"([- ](?<Label>" + Identifier + @"(\." + Identifier + @")*)|(?<Label>[A-Za-z][0-9A-Za-z-]*(\." + Identifier +
        @")*))?" +
        @"(\+(?<Meta>" + Identifier + @"(\." + Identifier + @")*))?$",
        RegexOptions.CultureInvariant);

    /// <summary>The <c>FullText</c> of nUpdate 4: <c>1.2.0.0 Beta 3</c>, or without a number when it was 0 (<c>1.2.0.0 ReleaseCandidate</c>).</summary>
    private static readonly Regex LongForm =
        new(@"^(?<Numbers>[0-9]+(\.[0-9]+){0,3}) (?<Stage>Alpha|Beta|ReleaseCandidate)( (?<Build>[0-9]+))?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>The stage shortcuts of nUpdate 3 and 4 with an optional number: <c>a</c>, <c>b3</c>, <c>rc.1</c>.</summary>
    private static readonly Regex ClassicStage = new(@"^(?<Stage>a|b|rc)\.?(?<Build>[0-9]+)?$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>Converts a version written by an earlier nUpdate; canonical versions are returned as they are.</summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out UpdateVersion? version)
    {
        version = null;
        if (text is null)
            return false;
        text = text.Trim();

        var longForm = LongForm.Match(text);
        if (longForm.Success)
        {
            var stage = longForm.Groups["Stage"].Value.ToLowerInvariant() switch
            {
                "alpha" => "a",
                "beta" => "b",
                _ => "rc",
            };
            var build = longForm.Groups["Build"].Success ? longForm.Groups["Build"].Value : string.Empty;
            return TryCreate(longForm.Groups["Numbers"].Value, stage + build, null, out version);
        }

        var shortForm = ShortForm.Match(text);
        return shortForm.Success
               && TryCreate(shortForm.Groups["Numbers"].Value,
                   shortForm.Groups["Label"].Success ? shortForm.Groups["Label"].Value : null,
                   shortForm.Groups["Meta"].Success ? shortForm.Groups["Meta"].Value : null, out version);
    }

    /// <exception cref="InvalidDataException">The text is not a version of any nUpdate.</exception>
    public static UpdateVersion Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return TryParse(text, out var version)
            ? version
            : throw new InvalidDataException($"\"{text}\" is not a version of nUpdate 3, nUpdate 4 or nUpdate 5.");
    }

    private static bool TryCreate(string numbers, string? label, string? metadata,
        [NotNullWhen(true)] out UpdateVersion? version)
    {
        version = null;
        var parts = new int[4];
        var texts = numbers.Split('.');
        for (var i = 0; i < texts.Length; i++)
        {
            if (!int.TryParse(texts[i], NumberStyles.None, CultureInfo.InvariantCulture, out parts[i]))
                return false;
        }

        string? preRelease = null;
        if (label is not null && !TryNormalizeLabel(label, out preRelease))
            return false;
        version = new UpdateVersion(parts[0], parts[1], parts[2], parts[3], preRelease, metadata);
        return true;
    }

    /// <summary>
    ///     <c>a</c>, <c>b3</c>, <c>rc.1</c> become <c>alpha</c>, <c>beta.3</c>, <c>rc.1</c>; other labels keep their
    ///     identifiers with leading zeros removed from numbers, since SemVer forbids them.
    /// </summary>
    private static bool TryNormalizeLabel(string label, out string? normalized)
    {
        normalized = null;
        var classic = ClassicStage.Match(label);
        if (classic.Success)
        {
            var stage = classic.Groups["Stage"].Value.ToLowerInvariant() switch
            {
                "a" => "alpha",
                "b" => "beta",
                _ => "rc",
            };
            if (!classic.Groups["Build"].Success)
            {
                normalized = stage;
                return true;
            }

            if (!int.TryParse(classic.Groups["Build"].Value, NumberStyles.None, CultureInfo.InvariantCulture,
                    out var build))
                return false;
            normalized = build == 0 ? stage : stage + "." + build.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        var identifiers = label.Split('.');
        for (var i = 0; i < identifiers.Length; i++)
        {
            if (identifiers[i].All(char.IsAsciiDigit))
                identifiers[i] = identifiers[i].TrimStart('0') is { Length: > 0 } trimmed ? trimmed : "0";
        }

        normalized = string.Join(".", identifiers);
        return true;
    }
}
