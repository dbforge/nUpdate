using System.Globalization;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.TransferInterface;
using nUpdate.Security;

namespace nUpdate.Administration.Core.Projects;

/// <summary>
///     Converts project files written by every earlier nUpdate Administration to format 6: the files before 1.0 Beta 2
///     (no <c>ConfigVersion</c>, package versions stored as objects), <c>1b2</c>, <c>3b2</c>, <c>v3</c> (one field layout
///     that only grew over time) and <c>v5</c> (the 5.0 pre-releases with secrets protected per user and machine).
///     Keys are converted from XML to PEM. The recovered secrets are returned in memory; they are written back once the
///     user chooses a project password.
/// </summary>
public sealed class ProjectMigrator(ICredentialProtector protector)
{
    private static readonly string[] V3Versions = ["1b2", "3b2", "v3"];

    /// <summary>Whether the <c>ConfigVersion</c> is one of the 3.x/4.x layouts; <c>null</c> marks the oldest files.</summary>
    public static bool IsV3(string? configVersion) =>
        configVersion is null || V3Versions.Contains(configVersion, StringComparer.OrdinalIgnoreCase);

    public static bool IsV5(string? configVersion) =>
        string.Equals(configVersion, "v5", StringComparison.OrdinalIgnoreCase);

    private readonly ICredentialProtector _protector = protector ?? throw new ArgumentNullException(nameof(protector));

    /// <param name="legacy">The parsed 3.x/4.x project file.</param>
    /// <param name="masterPassword">
    ///     When the old project did not save its credentials, the password the user entered then (the old tool
    ///     derived the key from the password and the FTP user name). <c>null</c> for projects with saved credentials.
    /// </param>
    public static ProjectLoadResult FromV3(JObject legacy, string? masterPassword)
    {
        ArgumentNullException.ThrowIfNull(legacy);
        var failed = false;

        var saveCredentials = legacy.Value<bool?>("SaveCredentials") ?? true;
        var ftpUsername = legacy.Value<string>("FtpUsername") ?? string.Empty;
        var keyPassword = saveCredentials ? LegacyAesCredentialDecryptor.BuiltInKeyPassword : masterPassword;
        var ivPassword = saveCredentials ? LegacyAesCredentialDecryptor.BuiltInIvPassword : ftpUsername;
        var canDecrypt = !string.IsNullOrEmpty(keyPassword) && !string.IsNullOrEmpty(ivPassword);

        string? Decrypt(string? cipher)
        {
            if (string.IsNullOrEmpty(cipher) || !canDecrypt)
                return null;
            try
            {
                return LegacyAesCredentialDecryptor.Decrypt(cipher, keyPassword!, ivPassword);
            }
            catch (Exception ex) when (ex is CryptographicException or FormatException)
            {
                failed = true;
                return null;
            }
        }

        var updateUrl = legacy.Value<string>("UpdateUrl") ?? string.Empty;
        var useStatistics = legacy.Value<bool?>("UseStatistics") ?? false;
        var proxyAddress = legacy["Proxy"]?.Type == JTokenType.Object
            ? legacy["Proxy"]!.Value<string>("Address")
            : null;

        var project = new UpdateProject
        {
            Id = Guid.TryParse(legacy.Value<string>("Guid"), out var id) ? id : Guid.NewGuid(),
            Name = legacy.Value<string>("Name") ?? string.Empty,
            UpdateUrl = NormalizeUrl(updateUrl),
            AssemblyVersionPath = legacy.Value<string>("AssemblyVersionPath"),
            PublicKey = ConvertKey(legacy.Value<string>("PublicKey"), isPrivate: false) ?? string.Empty,
            Transfer = new TransferSettings
            {
                Protocol = MapProtocol(legacy.Value<int?>("FtpProtocol") ?? 0),
                Host = legacy.Value<string>("FtpHost") ?? string.Empty,
                Port = legacy.Value<int?>("FtpPort") ?? 21,
                Directory = legacy.Value<string>("FtpDirectory") ?? "/",
                Username = ftpUsername,
                UsePassiveMode = legacy.Value<bool?>("FtpUsePassiveMode") ?? true,
                PluginAssemblyPath = NullIfEmpty(legacy.Value<string>("FtpTransferAssemblyFilePath")),
                Proxy = string.IsNullOrEmpty(proxyAddress)
                    ? null
                    : new ProxySettings
                    { Address = proxyAddress, Username = NullIfEmpty(legacy.Value<string>("ProxyUsername")) },
            },
            Statistics = new StatisticsSettings
            {
                Enabled = useStatistics,
                // statistics.php next to the feed stays with nUpdate 4's clients; the default endpoint is the script of this version.
                EndpointUrl = null,
                Database = useStatistics
                    ? new StatisticsDatabaseSettings
                    {
                        Host = legacy.Value<string>("SqlWebUrl") ?? "localhost",
                        Name = legacy.Value<string>("SqlDatabaseName") ?? string.Empty,
                        Username = legacy.Value<string>("SqlUsername") ?? string.Empty,
                    }
                    : null,
            },
        };
        if (!string.IsNullOrEmpty(project.Transfer.PluginAssemblyPath))
            project.Transfer.Protocol = TransferProtocol.Plugin;

        var credentials = legacy["HttpAuthenticationCredentials"];
        if (credentials?.Type == JTokenType.Object && !string.IsNullOrEmpty(credentials.Value<string>("UserName")))
            project.HttpAuthentication = new HttpAuthenticationSettings
            { Username = credentials.Value<string>("UserName")! };

        foreach (var package in legacy["Packages"]?.OfType<JObject>() ?? [])
        {
            if (!LegacyVersion.TryParse(ReadPackageVersion(package["Version"]), out var version))
                continue;
            project.Packages.Add(new UpdatePackage
            {
                Version = version,
                Description = package.Value<string>("Description") ?? string.Empty,
                Released = package.Value<bool?>("IsReleased") ?? false,
                CreatedAt = DateTimeOffset.MinValue,
            });
        }

        foreach (var entry in legacy["Log"]?.OfType<JObject>() ?? [])
        {
            project.Log.Add(new LogEntry
            {
                Kind = ParseKind(entry["Entry"]),
                At = DateTimeOffset.TryParse(entry.Value<string>("EntryTime"), CultureInfo.CurrentCulture,
                    DateTimeStyles.AssumeLocal, out var time)
                    ? time
                    : DateTimeOffset.MinValue,
                Version = LegacyVersion.TryParse(entry.Value<string>("PackageVersion"), out var logVersion)
                    ? logVersion
                    : null,
                User = entry.Value<string>("Username") ?? string.Empty,
            });
        }

        var secrets = new ProjectSecrets
        {
            TransferPassword = Decrypt(legacy.Value<string>("FtpPassword")),
            ProxyPassword = Decrypt(legacy.Value<string>("ProxyPassword")),
            HttpAuthenticationPassword = credentials?.Type == JTokenType.Object
                ? NullIfEmpty(credentials.Value<string>("Password"))
                : null,
            StatisticsDatabasePassword = useStatistics ? Decrypt(legacy.Value<string>("SqlPassword")) : null,
            // The statistics API of 3.x and 4.x had no admin secret; the migration deploys the new script with this one.
            StatisticsAdminSecret = useStatistics ? SecretGenerator.CreateSecret() : null,
            PrivateKey = ConvertKey(legacy.Value<string>("PrivateKey"), isPrivate: true),
        };
        return new ProjectLoadResult(project, secrets, migrated: true,
            failed ? SecretsState.Unreadable : SecretsState.Loaded);
    }

    /// <summary>Converts a file written by the 5.0 pre-releases: PascalCase names, literal versions, secrets protected per user and machine.</summary>
    public ProjectLoadResult FromV5(JObject legacy)
    {
        ArgumentNullException.ThrowIfNull(legacy);
        var failed = false;

        string? Unprotect(JToken? token)
        {
            var value = token?.Type == JTokenType.String ? token.ToString() : null;
            if (string.IsNullOrEmpty(value))
                return null;
            try
            {
                return _protector.Unprotect(value);
            }
            catch (Exception ex) when (ex is CryptographicException or FormatException)
            {
                failed = true;
                return null;
            }
        }

        var transfer = legacy["Transfer"] as JObject ?? [];
        var proxy = transfer["Proxy"] as JObject;
        var statistics = legacy["Statistics"] as JObject ?? [];
        var database = statistics["Database"] as JObject;
        var authentication = legacy["HttpAuthentication"] as JObject;

        var project = new UpdateProject
        {
            Id = Guid.TryParse(legacy.Value<string>("Id"), out var id) ? id : Guid.NewGuid(),
            Name = Text(legacy["Name"]),
            UpdateUrl = NormalizeUrl(Text(legacy["UpdateUrl"])),
            AssemblyVersionPath = legacy.Value<string>("AssemblyVersionPath"),
            PublicKey = ConvertKey(legacy.Value<string>("PublicKey"), isPrivate: false) ?? string.Empty,
            Transfer = new TransferSettings
            {
                Protocol = ParseEnum(transfer["Protocol"], TransferProtocol.Sftp),
                Host = Text(transfer["Host"]),
                Port = transfer.Value<int?>("Port") ?? 22,
                Directory = transfer.Value<string>("Directory") ?? "/",
                Username = Text(transfer["Username"]),
                UsePassiveMode = transfer.Value<bool?>("UsePassiveMode") ?? true,
                TrustedCertificateFingerprint = transfer.Value<string>("TrustedCertificateFingerprint"),
                SftpPrivateKeyPath = transfer.Value<string>("SftpPrivateKeyPath"),
                TrustedHostKeyFingerprint = transfer.Value<string>("TrustedHostKeyFingerprint"),
                PluginAssemblyPath = transfer.Value<string>("PluginAssemblyPath"),
                Proxy = proxy is null
                    ? null
                    : new ProxySettings
                    { Address = Text(proxy["Address"]), Username = proxy.Value<string>("Username") },
            },
            HttpAuthentication = authentication is null
                ? null
                : new HttpAuthenticationSettings { Username = Text(authentication["Username"]) },
            Statistics = new StatisticsSettings
            {
                Enabled = statistics.Value<bool?>("Enabled") ?? false,
                EndpointUrl = CustomEndpoint(statistics.Value<string>("EndpointUrl"), Text(legacy["UpdateUrl"])),
                Database = database is null
                    ? null
                    : new StatisticsDatabaseSettings
                    {
                        Host = database.Value<string>("Host") ?? "localhost",
                        Name = Text(database["Name"]),
                        Username = Text(database["Username"]),
                    },
            },
        };

        foreach (var package in legacy["Packages"]?.OfType<JObject>() ?? [])
        {
            if (!LegacyVersion.TryParse(package.Value<string>("LiteralVersion"), out var version))
                continue;
            project.Packages.Add(new UpdatePackage
            {
                Version = version,
                Description = Text(package["Description"]),
                Released = package.Value<bool?>("IsReleased") ?? false,
                CreatedAt = ParseTime(package.Value<string>("Created")),
            });
        }

        foreach (var entry in legacy["Log"]?.OfType<JObject>() ?? [])
        {
            project.Log.Add(new LogEntry
            {
                Kind = ParseEnum(entry["Kind"], LogEntryKind.Edit),
                At = ParseTime(entry.Value<string>("Time")),
                Version = LegacyVersion.TryParse(entry.Value<string>("PackageVersion"), out var logVersion)
                    ? logVersion
                    : null,
                User = Text(entry["Username"]),
            });
        }

        var saved = legacy.Value<bool?>("SaveCredentials") ?? true;
        var secrets = new ProjectSecrets
        {
            TransferPassword = Unprotect(transfer["ProtectedPassword"]),
            SftpKeyPassphrase = Unprotect(transfer["ProtectedSftpKeyPassphrase"]),
            ProxyPassword = Unprotect(proxy?["ProtectedPassword"]),
            HttpAuthenticationPassword = Unprotect(authentication?["ProtectedPassword"]),
            StatisticsAdminSecret = Unprotect(statistics["ProtectedAdminSecret"]),
            StatisticsDatabasePassword = Unprotect(database?["ProtectedPassword"]),
            PrivateKey = ConvertKey(Unprotect(legacy["ProtectedPrivateKey"]), isPrivate: true),
        };
        return new ProjectLoadResult(project, secrets, migrated: true,
            failed ? SecretsState.Unreadable : saved ? SecretsState.Loaded : SecretsState.NotSaved);
    }

    /// <summary>Converts an RSA key from the XML form earlier versions stored to PEM. Text that is not XML is returned unchanged.</summary>
    public static string? ConvertKey(string? key, bool isPrivate)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;
        if (!key.TrimStart().StartsWith('<'))
            return key;
        using var signing = PackageSigning.FromXml(key);
        return isPrivate ? signing.PrivateKeyPem : signing.PublicKeyPem;
    }

    /// <summary>
    ///     Package versions are strings since 1.0 Beta 2; before that they were serialized <c>UpdateVersion</c> objects
    ///     whose <c>DevelopmentalStage</c> counted 0 = Release, 1 = Beta, 2 = Alpha.
    /// </summary>
    private static string ReadPackageVersion(JToken? token)
    {
        if (token is not JObject version)
            return token?.Type == JTokenType.String ? token.ToString() : string.Empty;

        var numbers =
            $"{version.Value<int?>("Major") ?? 0}.{version.Value<int?>("Minor") ?? 0}.{version.Value<int?>("Build") ?? 0}.{version.Value<int?>("Revision") ?? 0}";
        var stage = version.Value<int?>("DevelopmentalStage") ?? 0;
        var developmentBuild = version.Value<int?>("DevelopmentBuild") ?? 0;
        return stage switch
        {
            1 => $"{numbers}b{developmentBuild}",
            2 => $"{numbers}a{developmentBuild}",
            _ => numbers,
        };
    }

    /// <summary>
    ///     The endpoint of a 5.0 pre-release project unless it is the pre-release default, <c>statistics.php</c> next to the
    ///     feed: that name belongs to the script of nUpdate 3 and 4, so such projects follow the default of this version.
    /// </summary>
    private static string? CustomEndpoint(string? endpoint, string updateUrl)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            return null;
        if (UpdateProject.IsValidUpdateUrl(updateUrl) && string.Equals(endpoint,
                new Uri(new Uri(NormalizeUrl(updateUrl)), Statistics.StatisticsScript.LegacyScriptFileName).ToString(),
                StringComparison.OrdinalIgnoreCase))
            return null;
        return endpoint;
    }

    /// <summary>The text of a token; missing and null read as empty.</summary>
    private static string Text(JToken? token) =>
        token is null || token.Type == JTokenType.Null ? string.Empty : token.ToString();

    private static string NormalizeUrl(string url) =>
        string.IsNullOrWhiteSpace(url) ? url : UpdateProject.NormalizeUpdateUrl(url);

    private static TransferProtocol MapProtocol(int legacyProtocol) => legacyProtocol switch
    {
        // Starksoft.Aspen.Ftps.FtpsSecurityProtocol: None=0, Tls1Explicit=1, Ssl3Explicit=2, Tls1OrSsl3Explicit=3, Tls1Implicit=4, Ssl3Implicit=5, Tls1OrSsl3Implicit=6
        0 => TransferProtocol.Ftp,
        1 or 2 or 3 => TransferProtocol.FtpsExplicit,
        _ => TransferProtocol.FtpsImplicit,
    };

    private static LogEntryKind ParseKind(JToken? token)
    {
        if (token is { Type: JTokenType.Integer })
            return (LogEntryKind)token.Value<int>();
        return Enum.TryParse<LogEntryKind>(token?.ToString(), ignoreCase: true, out var kind)
            ? kind
            : LogEntryKind.Edit;
    }

    private static T ParseEnum<T>(JToken? token, T fallback)
        where T : struct, Enum
    {
        if (token is { Type: JTokenType.Integer })
            return (T)Enum.ToObject(typeof(T), token.Value<int>());
        return Enum.TryParse<T>(token?.ToString(), ignoreCase: true, out var value) ? value : fallback;
    }

    private static DateTimeOffset ParseTime(string? text) =>
        DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var time)
            ? time
            : DateTimeOffset.MinValue;

    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;
}
