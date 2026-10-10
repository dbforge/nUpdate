using System.Text.Json;
using System.Text.Json.Serialization;
using nUpdate.Updating;

namespace Aurora;

/// <summary>
///     Where Aurora looks for updates. The settings live in the user's application data, outside the program folder, so
///     an update does not replace them and every installed version reads the same ones.
/// </summary>
public sealed class AuroraSettings
{
    private static readonly JsonSerializerOptions Json =
        new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };

    /// <summary>The feed nUpdate Administration uploads: the project's update URL followed by <c>nupdate.json</c>.</summary>
    public string FeedUrl { get; set; } = "http://localhost/aurora/nupdate.json";

    /// <summary>The PEM public key of the project (nUpdate Administration, Overview, Copy public key).</summary>
    public string PublicKey { get; set; } = string.Empty;

    public Stability MinimumStability { get; set; } = Stability.Release;

    public bool CheckOnStart { get; set; }

    public static string FilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Aurora", "settings.json");

    public static AuroraSettings Load()
    {
        try
        {
            return JsonSerializer.Deserialize<AuroraSettings>(File.ReadAllText(FilePath), Json) ?? new AuroraSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return new AuroraSettings();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, Json));
    }
}
