using nUpdate.Operations;
using nUpdate.Updating;

namespace nUpdate.Packaging;

/// <summary>The <c>manifest.json</c> at the root of every package zip: what the package is and what it does.</summary>
public sealed class PackageManifest
{
    public const int CurrentFormat = 1;

    public int Format { get; set; } = CurrentFormat;

    public Guid ProjectId { get; set; }

    public UpdateVersion Version { get; set; } = new();

    /// <summary>The platform of the package file, see <see cref="PackagePlatform" />.</summary>
    public string Platform { get; set; } = PackagePlatform.Any;

    public DateTimeOffset CreatedAt { get; set; }

    public List<Operation> Operations { get; set; } = [];

    /// <summary>
    ///     The macOS code signature attributes of package files, by entry name (<c>Program/Contents/MacOS/App.dll</c>)
    ///     and attribute name, as Base64; see <see cref="Platform.ICodeSignatureAttributes" />. The installer sets them on
    ///     the installed files.
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> CodeSignatures { get; set; } = new(StringComparer.Ordinal);

    /// <summary>The areas the operations touch, in a stable order, for the feed's <c>touches</c>.</summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyList<OperationArea> Touches => Operations.Select(o => o.Area).Distinct().OrderBy(a => a).ToList();
}

/// <summary>The folders a package is copied into on the client.</summary>
public enum PackageRoot
{
    /// <summary>The application directory.</summary>
    Program,

    /// <summary>The user's roaming application data.</summary>
    AppData,

    /// <summary>The temp folder.</summary>
    Temp,

    /// <summary>The user's desktop.</summary>
    Desktop,
}

/// <summary>The names inside a package zip, shared by the Administration that builds packages and the installer that reads them.</summary>
internal static class PackageLayout
{
    public const string ManifestFileName = "manifest.json";

    public static IReadOnlyList<PackageRoot> Roots { get; } =
        [PackageRoot.Program, PackageRoot.AppData, PackageRoot.Temp, PackageRoot.Desktop];

    /// <summary>The folder name of a root inside the zip, for example <c>Program</c>.</summary>
    public static string FolderName(PackageRoot root) => Enum.IsDefined(typeof(PackageRoot), root)
        ? root.ToString()
        : throw new ArgumentOutOfRangeException(nameof(root));

    /// <summary>The folder below the feed that holds the package files.</summary>
    public const string PackagesFolderName = "packages";

    /// <summary>The file name of the package file of a platform, on the server and locally: <c>win-x64.zip</c>.</summary>
    public static string PackageFileName(string platform) =>
        platform is null ? throw new ArgumentNullException(nameof(platform)) : $"{platform}.zip";

    /// <summary>The folder of a version relative to the feed on the server: <c>packages/2.1.0</c>.</summary>
    public static string RemoteVersionDirectory(UpdateVersion version) =>
        version is null ? throw new ArgumentNullException(nameof(version)) : $"{PackagesFolderName}/{version}";

    /// <summary>The path of a package file relative to the feed on the server: <c>packages/2.1.0/win-x64.zip</c>.</summary>
    public static string RemotePackagePath(UpdateVersion version, string platform) =>
        $"{RemoteVersionDirectory(version)}/{PackageFileName(platform)}";
}
