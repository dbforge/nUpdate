using Newtonsoft.Json.Linq;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Migration;

/// <summary>Reads the <c>updates.json</c> written by nUpdate Administration 3.x and 4.x: a JSON array of configuration entries with PascalCase names and numeric enums.</summary>
public static class LegacyFeed
{
    public const string FileName = "updates.json";

    /// <exception cref="InvalidDataException">The content is not a legacy feed, or an entry has no readable version.</exception>
    public static List<LegacyFeedEntry> Parse(string content) => Parse(content, null);

    /// <summary>Reads the feed; entries without a readable version are left out and their version text added to <paramref name="unreadable" />.</summary>
    /// <exception cref="InvalidDataException">The content is not a legacy feed, or (without <paramref name="unreadable" />) an entry has no readable version.</exception>
    public static List<LegacyFeedEntry> Parse(string content, ICollection<string>? unreadable)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (string.IsNullOrWhiteSpace(content))
            return [];
        JToken json;
        try
        {
            json = JToken.Parse(content);
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidDataException("The legacy feed is not valid JSON.", ex);
        }

        if (json is not JArray array)
            throw new InvalidDataException("The legacy feed is not a JSON array.");

        var entries = new List<LegacyFeedEntry>();
        foreach (var item in array.OfType<JObject>())
        {
            var literal = Text(item["LiteralVersion"]);
            if (!LegacyVersion.TryParse(literal, out var version))
            {
                if (unreadable is null)
                    throw new InvalidDataException($"\"{literal}\" in the legacy feed is not a valid version.");
                unreadable.Add(literal);
                continue;
            }

            entries.Add(new LegacyFeedEntry(version, literal)
            {
                Platform = ParsePlatform(item["Architecture"]),
                Necessary = Flag(item["NecessaryUpdate"]),
                Changelog = ReadChangelog(item["Changelog"]),
                UnsupportedVersions = (item["UnsupportedVersions"] as JArray ?? [])
                    .Select(t => LegacyVersion.TryParse(t.ToString(), out var unsupported) ? unsupported : null)
                    .Where(v => v is not null).Select(v => v!).ToList(),
                Rollout = new RolloutSettings
                {
                    Mode = ParseMode(item["RolloutConditionMode"]),
                    Conditions = (item["RolloutConditions"] as JArray ?? []).OfType<JObject>()
                        .Select(c =>
                            new RolloutCondition(Text(c["Key"]), Text(c["Value"]), Flag(c["IsNegativeCondition"])))
                        .Where(c => c.Key.Length > 0).ToList(),
                },
                PackageUri = Uri.TryCreate(item.Value<string>("UpdatePackageUri"), UriKind.Absolute, out var uri)
                    ? uri
                    : null,
                UseStatistics = Flag(item["UseStatistics"]),
                Operations = item["Operations"] as JArray,
                Signature = item["Signature"]?.Type == JTokenType.String && Text(item["Signature"]).Length > 0
                    ? Text(item["Signature"])
                    : null,
            });
        }

        return entries;
    }

    private static Dictionary<string, string> ReadChangelog(JToken? token)
    {
        var changelog = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (token is JObject entries)
        {
            foreach (var pair in entries)
                changelog[pair.Key] = Text(pair.Value);
        }

        return changelog;
    }

    /// <summary>nUpdate 3 and 4 only ran on Windows: X86 (0) is <c>win-x86</c>, X64 (1) <c>win-x64</c>, Independent (2) <c>win</c>.</summary>
    private static string ParsePlatform(JToken? token)
    {
        if (token is { Type: JTokenType.Integer })
            return token.Value<int>() switch { 0 => "win-x86", 1 => "win-x64", _ => PackagePlatform.Windows };
        return Text(token).ToUpperInvariant() switch
        {
            "X86" => "win-x86",
            "X64" => "win-x64",
            _ => PackagePlatform.Windows
        };
    }

    /// <summary><c>true</c> for JSON true or the text "true"; anything else, including garbage, is <c>false</c>.</summary>
    private static bool Flag(JToken? token) => string.Equals(Text(token), "true", StringComparison.OrdinalIgnoreCase);

    /// <summary>The text of a token; <c>null</c> and JSON null read as empty.</summary>
    private static string Text(JToken? token) =>
        token is null || token.Type == JTokenType.Null ? string.Empty : token.ToString();

    private static RolloutConditionMode ParseMode(JToken? token)
    {
        // nUpdate 4: AtLeastOne = 0, All = 1.
        if (token is { Type: JTokenType.Integer })
            return token.Value<int>() == 1 ? RolloutConditionMode.All : RolloutConditionMode.Any;
        return string.Equals(token?.ToString(), "All", StringComparison.OrdinalIgnoreCase)
            ? RolloutConditionMode.All
            : RolloutConditionMode.Any;
    }
}

/// <summary>One entry of a legacy feed.</summary>
public sealed class LegacyFeedEntry(UpdateVersion version, string literalVersion)
{
    public UpdateVersion Version { get; } = version ?? throw new ArgumentNullException(nameof(version));

    /// <summary>The version as the old file spelled it, which is also the name of the package folder on the server.</summary>
    public string LiteralVersion { get; } = literalVersion ?? throw new ArgumentNullException(nameof(literalVersion));

    /// <summary>The platform of the package: <c>win-x86</c>, <c>win-x64</c> or <c>win</c>, since nUpdate 3 and 4 only ran on Windows.</summary>
    public string Platform { get; set; } = PackagePlatform.Windows;

    public bool Necessary { get; set; }

    public Dictionary<string, string> Changelog { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public List<UpdateVersion> UnsupportedVersions { get; set; } = [];

    public RolloutSettings Rollout { get; set; } = new();

    public Uri? PackageUri { get; set; }

    public bool UseStatistics { get; set; }

    /// <summary>Operations stored in the feed by the oldest versions, in their legacy shape.</summary>
    public JArray? Operations { get; set; }

    /// <summary>The Base64 RSA PKCS#1 SHA-512 signature nUpdate 3 and 4 made over the zip, or <c>null</c>.</summary>
    public string? Signature { get; set; }

    /// <summary>The folder of the package on the server, relative to the update URL: the literal version.</summary>
    public string RemoteDirectory => LiteralVersion;
}
