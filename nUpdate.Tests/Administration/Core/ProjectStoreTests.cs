using System.IO.Abstractions.TestingHelpers;
using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.TransferInterface;
using nUpdate.Exceptions;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class ProjectStoreTests
{
    private readonly AdminTestContext _context = new();

    /// <summary>The XML form of the test key pair, as nUpdate 3.x and 4.x stored it.</summary>
    public static (string PublicXml, string PrivateXml) XmlKeys { get; } = CreateXmlKeys();

    private static (string PublicXml, string PrivateXml) CreateXmlKeys()
    {
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportParameters(nUpdate.Security.RsaKeyPem.DecodePrivate(TestKeys.PrivateKey));
        return (rsa.ToXmlString(false), rsa.ToXmlString(true));
    }

    public static string LegacyProjectJson(bool saveCredentials, string keyPassword, string ivPassword) => $$"""
        {
          "ApplicationId": 7, "AssemblyVersionPath": "C:\\app\\app.exe", "ConfigVersion": "v3",
          "FtpDirectory": "/updates", "FtpHost": "ftp.example.com", "FtpNetworkVersion": 0,
          "FtpPassword": "{{LegacyAesCredentialDecryptor.Encrypt("ftp-pw", keyPassword, ivPassword)}}",
          "FtpPort": 2121, "FtpProtocol": 1, "FtpTransferAssemblyFilePath": "", "FtpUsePassiveMode": false, "FtpUsername": "ftpuser",
          "Guid": "12345678-1234-1234-1234-123456789abc",
          "HttpAuthenticationCredentials": { "UserName": "web", "Password": "webpw", "Domain": "" },
          "Log": [ { "Entry": 0, "EntryTime": "2020-01-02 03:04:05", "PackageVersion": "1.0.0.0", "Project": null, "Username": "DOM\\user" },
                   { "Entry": "Upload", "EntryTime": "garbage", "PackageVersion": null, "Username": "u2" } ],
          "Name": "Legacy", "Packages": [ { "Version": "1.0.0.0", "Description": "first", "IsReleased": true, "LocalPackagePath": "x" }, { "Version": "1.1.0.0b2", "IsReleased": false } ],
          "Path": "C:\\projects\\Legacy.nupdproj",
          "PrivateKey": "{{XmlKeys.PrivateXml}}",
          "Proxy": { "Address": "http://proxy:8080", "BypassList": [], "BypassProxyOnLocal": false, "UseDefaultCredentials": false },
          "ProxyPassword": "{{LegacyAesCredentialDecryptor.Encrypt("proxy-pw", keyPassword, ivPassword)}}", "ProxyUsername": "proxyuser",
          "PublicKey": "{{XmlKeys.PublicXml}}",
          "SaveCredentials": {{(saveCredentials ? "true" : "false")}},
          "SqlDatabaseName": "stats", "SqlPassword": "{{LegacyAesCredentialDecryptor.Encrypt("sql-pw", keyPassword, ivPassword)}}",
          "SqlUsername": "sqluser", "SqlWebUrl": "db.example.com", "UpdateUrl": "https://updates.example.com/legacy", "UseStatistics": true
        }
        """;

    private string V5ProjectJson(bool saveCredentials) => $$"""
        {
          "ConfigVersion": "v5", "Id": "12345678-1234-1234-1234-123456789abc", "Name": "Five", "UpdateUrl": "https://updates.example.com/five/",
          "AssemblyVersionPath": null,
          "Transfer": { "Protocol": 3, "Host": "sftp.example.com", "Port": 2222, "Directory": "/five", "Username": "deploy",
                        "ProtectedPassword": "{{_context.Protector.Protect("sftp-pw")}}", "UsePassiveMode": true, "TrustedCertificateFingerprint": null,
                        "SftpPrivateKeyPath": "/keys/id", "ProtectedSftpKeyPassphrase": "{{_context.Protector.Protect("phrase")}}", "TrustedHostKeyFingerprint": "ab:cd",
                        "PluginAssemblyPath": null, "Proxy": { "Address": "http://proxy", "Username": "pu", "ProtectedPassword": "{{_context.Protector.Protect("proxy-pw")}}" } },
          "HttpAuthentication": { "Username": "web", "ProtectedPassword": "{{_context.Protector.Protect("web-pw")}}" },
          "Statistics": { "Enabled": true, "EndpointUrl": "https://updates.example.com/five/statistics.php", "ProtectedAdminSecret": "{{_context.Protector.Protect("admin")}}",
                          "Database": { "Host": "db", "Name": "stats", "Username": "dbu", "ProtectedPassword": "{{_context.Protector.Protect("db-pw")}}" } },
          "ProtectedPrivateKey": "{{_context.Protector.Protect(XmlKeys.PrivateXml)}}",
          "PublicKey": "{{XmlKeys.PublicXml}}",
          "SaveCredentials": {{(saveCredentials ? "true" : "false")}},
          "Packages": [ { "LiteralVersion": "1.0.0.0", "Description": "first", "IsReleased": true, "Created": "2026-01-02T03:04:05+00:00" }, { "LiteralVersion": "", "IsReleased": false } ],
          "Log": [ { "Kind": "Upload", "Time": "2026-01-02T03:04:05+00:00", "PackageVersion": "1.0.0.0", "Username": "u" }, { "Kind": 3, "Time": "x", "PackageVersion": null, "Username": "u" } ]
        }
        """;

    [Fact]
    public async Task List_IsEmptyWithoutFile_AndSaveRegisters()
    {
        (await _context.Store.ListAsync()).ShouldBeEmpty();
        var project = _context.NewProject();
        await _context.Store.SaveAsync(project);

        var entries = await _context.Store.ListAsync();
        entries.Single().Name.ShouldBe("Demo");
        entries.Single().Id.ShouldBe(project.Id);
        entries.Single().Path.ShouldBe(project.Path);
        _context.FileSystem.File.Exists(project.Path).ShouldBeTrue();
        _context.FileSystem.File.ReadAllText(project.Path).ShouldStartWith("{" + Environment.NewLine + "  \"format\": 6,");

        var loaded = await _context.Store.LoadAsync(project.Path);
        loaded.Migrated.ShouldBeFalse();
        loaded.SecretsState.ShouldBe(SecretsState.NotSaved);
        loaded.Project.Name.ShouldBe("Demo");
        loaded.Project.Path.ShouldBe(project.Path);
        loaded.Project.Id.ShouldBe(project.Id);
        loaded.Project.Folder.ShouldBe(_context.ProjectFolder("Demo"));
    }

    [Fact]
    public async Task Register_ReplacesByIdOrPath_AndUnregisterRemoves()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        await _context.Store.RegisterAsync(new ProjectRegistration(a, "A", "/a"));
        await _context.Store.RegisterAsync(new ProjectRegistration(a, "a", "/a2"));
        await _context.Store.RegisterAsync(new ProjectRegistration(b, "B", "/b"));
        await _context.Store.RegisterAsync(new ProjectRegistration(Guid.NewGuid(), "C", "/b"));
        (await _context.Store.ListAsync()).Select(e => $"{e.Name}={e.Path}").ShouldBe(["a=/a2", "C=/b"]);

        await _context.Store.UnregisterAsync(a);
        await _context.Store.UnregisterAsync(Guid.NewGuid());
        (await _context.Store.ListAsync()).Select(e => e.Name).ShouldBe(["C"]);
        await _context.Store.UnregisterPathAsync("/nope");
        (await _context.Store.ListAsync()).Select(e => e.Name).ShouldBe(["C"]);
        await _context.Store.UnregisterPathAsync("/b");
        (await _context.Store.ListAsync()).ShouldBeEmpty();
        await Should.ThrowAsync<ArgumentException>(() => _context.Store.UnregisterPathAsync(" "));

        _context.FileSystem.File.WriteAllText(_context.Paths.ProjectsConfigFile, "");
        (await _context.Store.ListAsync()).ShouldBeEmpty();
        _context.FileSystem.File.WriteAllText(_context.Paths.ProjectsConfigFile, "null");
        (await _context.Store.ListAsync()).ShouldBeEmpty();
        _context.FileSystem.File.WriteAllText(_context.Paths.ProjectsConfigFile, """{"format":1}""");
        (await _context.Store.ListAsync()).ShouldBeEmpty();
        _context.FileSystem.File.WriteAllText(_context.Paths.ProjectsConfigFile, """{"format":9,"projects":[]}""");
        await Should.ThrowAsync<UnsupportedFormatException>(() => _context.Store.ListAsync());
        _context.FileSystem.File.WriteAllText(_context.Paths.ProjectsConfigFile, "{broken");
        await Should.ThrowAsync<Newtonsoft.Json.JsonException>(() => _context.Store.ListAsync());

        await Should.ThrowAsync<ArgumentNullException>(() => _context.Store.RegisterAsync(null!));
        Should.Throw<ArgumentNullException>(() => new ProjectRegistration(Guid.Empty, null!, "/p"));
        Should.Throw<ArgumentNullException>(() => new ProjectRegistration(Guid.Empty, "n", null!));
        new ProjectRegistration().Path.ShouldBe("");
    }

    [Theory]
    [InlineData("https://stats.example.com/api.php", "https://updates.example.com/five/", "https://stats.example.com/api.php")]
    [InlineData("https://updates.example.com/five/statistics.php", "not a url", "https://updates.example.com/five/statistics.php")]
    [InlineData("  ", "https://updates.example.com/five/", null)]
    public async Task Load_KeepsACustomStatisticsEndpointOfA50PreRelease(string endpoint, string updateUrl, string? expected)
    {
        var json = V5ProjectJson(true)
            .Replace("\"EndpointUrl\": \"https://updates.example.com/five/statistics.php\"", $"\"EndpointUrl\": \"{endpoint}\"", StringComparison.Ordinal)
            .Replace("\"UpdateUrl\": \"https://updates.example.com/five/\"", $"\"UpdateUrl\": \"{updateUrl}\"", StringComparison.Ordinal);
        _context.FileSystem.AddFile("/old/Custom/Custom.nupdproj", new MockFileData(json));
        (await _context.Store.LoadAsync("/old/Custom/Custom.nupdproj")).Project.Statistics.EndpointUrl.ShouldBe(expected);
    }

    [Fact]
    public async Task List_TakesOverTheProjectListOfAdministration4WithoutChangingIt()
    {
        const string legacyList = """[{"Name":"Old","Path":"/old/Old.nupdproj"},{"Name":"Pathless"},{"Name":null,"Path":"/old/Nameless.nupdproj"},"junk"]""";
        _context.FileSystem.AddFile(_context.Paths.LegacyProjectsConfigFile, new MockFileData(legacyList));

        var entries = await _context.Store.ListAsync();

        entries.Select(e => $"{e.Name}={e.Path}").ShouldBe(["Old=/old/Old.nupdproj", "=/old/Nameless.nupdproj"]);
        entries.ShouldAllBe(e => e.Id == Guid.Empty);

        // The first change writes the list of this version; nUpdate Administration 4 keeps its own file as it was.
        await _context.Store.UnregisterPathAsync("/old/Nameless.nupdproj");
        _context.FileSystem.File.Exists(_context.Paths.ProjectsConfigFile).ShouldBeTrue();
        _context.FileSystem.File.ReadAllText(_context.Paths.LegacyProjectsConfigFile).ShouldBe(legacyList);
        (await _context.Store.ListAsync()).Select(e => e.Name).ShouldBe(["Old"]);

        // From then on only this version's list counts, so projects added in nUpdate Administration 4 later are not picked up.
        _context.FileSystem.File.WriteAllText(_context.Paths.LegacyProjectsConfigFile, """[{"Name":"Later","Path":"/old/Later.nupdproj"}]""");
        (await _context.Store.ListAsync()).Select(e => e.Name).ShouldBe(["Old"]);
        _context.FileSystem.File.WriteAllText(_context.Paths.ProjectsConfigFile, """{"format":1,"projects":null}""");
        (await _context.Store.ListAsync()).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("{broken")]
    [InlineData("""{"format":1,"projects":[]}""")]
    [InlineData("null")]
    public async Task List_IgnoresALegacyListItCannotUse(string content)
    {
        _context.FileSystem.AddFile(_context.Paths.LegacyProjectsConfigFile, new MockFileData(content));
        (await _context.Store.ListAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Load_RoundTripsSecretsUnderTheProjectPassword()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = new ProjectSecrets
        {
            TransferPassword = "t",
            SftpKeyPassphrase = "s",
            ProxyPassword = "p",
            HttpAuthenticationPassword = "h",
            StatisticsAdminSecret = "a",
            StatisticsDatabasePassword = "d",
            PrivateKey = "k",
        };
        project.Secrets = ProjectSecretsProtection.Protect(secrets, "pw");
        await _context.Store.SaveAsync(project);
        _context.FileSystem.File.ReadAllText(project.Path).ShouldNotContain("\"t\"");

        var withoutPassword = await _context.Store.LoadAsync(project.Path);
        withoutPassword.SecretsState.ShouldBe(SecretsState.PasswordRequired);
        withoutPassword.Secrets.PrivateKey.ShouldBeNull();

        var wrongPassword = await _context.Store.LoadAsync(project.Path, "wrong");
        wrongPassword.SecretsState.ShouldBe(SecretsState.Unreadable);

        var loaded = await _context.Store.LoadAsync(project.Path, "pw");
        loaded.SecretsState.ShouldBe(SecretsState.Loaded);
        var restored = loaded.Secrets;
        restored.TransferPassword.ShouldBe("t");
        restored.SftpKeyPassphrase.ShouldBe("s");
        restored.ProxyPassword.ShouldBe("p");
        restored.HttpAuthenticationPassword.ShouldBe("h");
        restored.StatisticsAdminSecret.ShouldBe("a");
        restored.StatisticsDatabasePassword.ShouldBe("d");
        restored.PrivateKey.ShouldBe("k");
        ProjectSecretsProtection.IsComplete(loaded.Project, restored).ShouldBeTrue();

        project.Secrets = "not base64!";
        await _context.Store.SaveAsync(project);
        (await _context.Store.LoadAsync(project.Path, "pw")).SecretsState.ShouldBe(SecretsState.Unreadable);
    }

    [Fact]
    public async Task Load_MigratesV3WithSavedCredentials()
    {
        _context.FileSystem.AddFile("/old/Legacy.nupdproj", new MockFileData(LegacyProjectJson(true, LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword)));

        var result = await _context.Store.LoadAsync("/old/Legacy.nupdproj");

        result.Migrated.ShouldBeTrue();
        result.SecretsState.ShouldBe(SecretsState.Loaded);
        var project = result.Project;
        project.Format.ShouldBe(6);
        project.Secrets.ShouldBeNull();
        project.Id.ShouldBe(Guid.Parse("12345678-1234-1234-1234-123456789abc"));
        project.Name.ShouldBe("Legacy");
        project.Path.ShouldBe("/old/Legacy.nupdproj");
        project.UpdateUrl.ShouldBe("https://updates.example.com/legacy/");
        project.AssemblyVersionPath.ShouldBe("C:\\app\\app.exe");
        project.PublicKey.ShouldBe(TestKeys.PublicKey);
        project.Transfer.Protocol.ShouldBe(TransferProtocol.FtpsExplicit);
        project.Transfer.Host.ShouldBe("ftp.example.com");
        project.Transfer.Port.ShouldBe(2121);
        project.Transfer.Directory.ShouldBe("/updates");
        project.Transfer.Username.ShouldBe("ftpuser");
        project.Transfer.UsePassiveMode.ShouldBeFalse();
        project.Transfer.PluginAssemblyPath.ShouldBeNull();
        project.Transfer.Proxy!.Address.ShouldBe("http://proxy:8080");
        project.Transfer.Proxy.Username.ShouldBe("proxyuser");
        project.HttpAuthentication!.Username.ShouldBe("web");
        project.Statistics.Enabled.ShouldBeTrue();
        // statistics.php keeps serving the clients of nUpdate 4; the converted project uses the default script of this version.
        project.Statistics.EndpointUrl.ShouldBeNull();
        project.Statistics.Database!.Host.ShouldBe("db.example.com");
        project.Statistics.Database.Name.ShouldBe("stats");
        project.Statistics.Database.Username.ShouldBe("sqluser");
        project.Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0-beta.2"]);
        project.Packages[0].Description.ShouldBe("first");
        project.Packages[0].Released.ShouldBeTrue();
        project.Log.Count.ShouldBe(2);
        project.Log[0].Kind.ShouldBe(LogEntryKind.Create);
        project.Log[0].At.Year.ShouldBe(2020);
        project.Log[0].Version.ShouldBe(new UpdateVersion("1.0.0"));
        project.Log[0].User.ShouldBe("DOM\\user");
        project.Log[1].Kind.ShouldBe(LogEntryKind.Upload);
        project.Log[1].At.ShouldBe(DateTimeOffset.MinValue);
        project.Log[1].Version.ShouldBeNull();

        result.Secrets.TransferPassword.ShouldBe("ftp-pw");
        result.Secrets.ProxyPassword.ShouldBe("proxy-pw");
        result.Secrets.HttpAuthenticationPassword.ShouldBe("webpw");
        result.Secrets.StatisticsDatabasePassword.ShouldBe("sql-pw");
        result.Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
        // nUpdate 4 had no admin secret; the migration generates one so that the statistics script v2 can be set up.
        result.Secrets.StatisticsAdminSecret.ShouldNotBeNullOrEmpty();

        await _context.Store.SaveAsync(project);
        (await _context.Store.LoadAsync(project.Path)).Migrated.ShouldBeFalse();
    }

    [Fact]
    public async Task Load_MigratesV3WithMasterPassword()
    {
        _context.FileSystem.AddFile("/old/Legacy.nupdproj", new MockFileData(LegacyProjectJson(false, "master", "ftpuser")));

        var withoutPassword = await _context.Store.LoadAsync("/old/Legacy.nupdproj");
        withoutPassword.Secrets.TransferPassword.ShouldBeNull();
        withoutPassword.SecretsState.ShouldBe(SecretsState.Loaded);

        var withPassword = await _context.Store.LoadAsync("/old/Legacy.nupdproj", "master");
        withPassword.Secrets.TransferPassword.ShouldBe("ftp-pw");
        withPassword.SecretsState.ShouldBe(SecretsState.Loaded);

        var wrongPassword = await _context.Store.LoadAsync("/old/Legacy.nupdproj", "wrong");
        wrongPassword.SecretsState.ShouldBe(SecretsState.Unreadable);
        wrongPassword.Secrets.TransferPassword.ShouldBeNull();
    }

    [Fact]
    public async Task Load_MigratesV5WithProtectedSecrets()
    {
        _context.FileSystem.AddFile("/old/Five/Five.nupdproj", new MockFileData(V5ProjectJson(true)));

        var result = await _context.Store.LoadAsync("/old/Five/Five.nupdproj");

        result.Migrated.ShouldBeTrue();
        result.SecretsState.ShouldBe(SecretsState.Loaded);
        var project = result.Project;
        project.Id.ShouldBe(Guid.Parse("12345678-1234-1234-1234-123456789abc"));
        project.Name.ShouldBe("Five");
        project.UpdateUrl.ShouldBe("https://updates.example.com/five/");
        project.PublicKey.ShouldBe(TestKeys.PublicKey);
        project.Transfer.Protocol.ShouldBe(TransferProtocol.Sftp);
        project.Transfer.Host.ShouldBe("sftp.example.com");
        project.Transfer.Port.ShouldBe(2222);
        project.Transfer.Directory.ShouldBe("/five");
        project.Transfer.Username.ShouldBe("deploy");
        project.Transfer.SftpPrivateKeyPath.ShouldBe("/keys/id");
        project.Transfer.TrustedHostKeyFingerprint.ShouldBe("ab:cd");
        project.Transfer.Proxy!.Address.ShouldBe("http://proxy");
        project.Transfer.Proxy.Username.ShouldBe("pu");
        project.HttpAuthentication!.Username.ShouldBe("web");
        project.Statistics.Enabled.ShouldBeTrue();
        project.Statistics.EndpointUrl.ShouldBeNull(); // the pre-release default follows the default of this version
        project.Statistics.Database!.Host.ShouldBe("db");
        project.Packages.Single().Version.ShouldBe(new UpdateVersion("1.0.0"));
        project.Packages.Single().Released.ShouldBeTrue();
        project.Packages.Single().CreatedAt.Year.ShouldBe(2026);
        project.Log.Select(l => l.Kind).ShouldBe([LogEntryKind.Upload, LogEntryKind.Edit]);
        project.Log[0].Version.ShouldBe(new UpdateVersion("1.0.0"));
        project.Log[1].At.ShouldBe(DateTimeOffset.MinValue);

        result.Secrets.TransferPassword.ShouldBe("sftp-pw");
        result.Secrets.SftpKeyPassphrase.ShouldBe("phrase");
        result.Secrets.ProxyPassword.ShouldBe("proxy-pw");
        result.Secrets.HttpAuthenticationPassword.ShouldBe("web-pw");
        result.Secrets.StatisticsAdminSecret.ShouldBe("admin");
        result.Secrets.StatisticsDatabasePassword.ShouldBe("db-pw");
        result.Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
    }

    [Fact]
    public async Task Load_ReportsV5SecretsOfAnotherMachineAndSparseFiles()
    {
        var foreign = V5ProjectJson(true).Replace(_context.Protector.Protect("sftp-pw"), "foreign", StringComparison.Ordinal);
        _context.FileSystem.AddFile("/old/Five.nupdproj", new MockFileData(V5ProjectJson(true)));
        var json = JObject.Parse(_context.FileSystem.File.ReadAllText("/old/Five.nupdproj"));
        json["Transfer"]!["ProtectedPassword"] = "foreign";
        _context.FileSystem.File.WriteAllText("/old/Five.nupdproj", json.ToString());
        var result = await _context.Store.LoadAsync("/old/Five.nupdproj");
        result.SecretsState.ShouldBe(SecretsState.Unreadable);
        result.Secrets.TransferPassword.ShouldBeNull();
        result.Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
        foreign.ShouldNotBeNull();

        _context.FileSystem.AddFile("/old/Sparse.nupdproj", new MockFileData("""{"ConfigVersion":"v5","SaveCredentials":false}"""));
        var sparse = await _context.Store.LoadAsync("/old/Sparse.nupdproj");
        sparse.SecretsState.ShouldBe(SecretsState.NotSaved);
        sparse.Project.Id.ShouldNotBe(Guid.Empty);
        sparse.Project.Transfer.Protocol.ShouldBe(TransferProtocol.Sftp);
        sparse.Project.Transfer.Proxy.ShouldBeNull();
        sparse.Project.HttpAuthentication.ShouldBeNull();
        sparse.Project.Statistics.Database.ShouldBeNull();
        sparse.Project.Packages.ShouldBeEmpty();
        sparse.Secrets.PrivateKey.ShouldBeNull();
        sparse.Project.UpdateUrl.ShouldBe("");

        _context.FileSystem.AddFile("/old/Pem.nupdproj", new MockFileData("""{"ConfigVersion":"v5","Name":null,"PublicKey":"-----BEGIN PUBLIC KEY-----\nAAAA\n-----END PUBLIC KEY-----","Transfer":{"Protocol":"Ftp","Host":null,"Proxy":{"Address":null}},"HttpAuthentication":{"Username":null},"Statistics":{"Database":{"Name":null}},"Packages":[{"LiteralVersion":"1.0","Description":null}],"Log":[{"Username":null}]}"""));
        var pem = await _context.Store.LoadAsync("/old/Pem.nupdproj");
        pem.Project.PublicKey.ShouldStartWith("-----BEGIN PUBLIC KEY-----");
        pem.Project.Transfer.Protocol.ShouldBe(TransferProtocol.Ftp);
        pem.Project.Name.ShouldBe("");
        pem.Project.Transfer.Proxy!.Address.ShouldBe("");
        pem.Project.HttpAuthentication!.Username.ShouldBe("");
        pem.Project.Statistics.Database!.Name.ShouldBe("");
        pem.Project.Packages.Single().Description.ShouldBe("");
        pem.Project.Log.Single().User.ShouldBe("");
    }

    [Fact]
    public async Task Load_RejectsInvalidAndUnknownFiles()
    {
        _context.FileSystem.AddFile("/p/noversion.nupdproj", new MockFileData("""{"Name":"Old"}"""));
        (await _context.Store.LoadAsync("/p/noversion.nupdproj")).Migrated.ShouldBeTrue();
        _context.FileSystem.AddFile("/p/broken.nupdproj", new MockFileData("{not json"));
        _context.FileSystem.AddFile("/p/future.nupdproj", new MockFileData("""{"ConfigVersion":"v9"}"""));
        _context.FileSystem.AddFile("/p/format9.nupdproj", new MockFileData("""{"format":9}"""));
        _context.FileSystem.AddFile("/p/formatx.nupdproj", new MockFileData("""{"format":"six"}"""));
        _context.FileSystem.AddFile("/p/nulls.nupdproj", new MockFileData("""{"format":6,"transfer":null,"statistics":null,"packages":null,"log":null,"secrets":null}"""));
        await Should.ThrowAsync<InvalidDataException>(() => _context.Store.LoadAsync("/p/broken.nupdproj"));
        await Should.ThrowAsync<UnsupportedFormatException>(() => _context.Store.LoadAsync("/p/future.nupdproj"));
        await Should.ThrowAsync<UnsupportedFormatException>(() => _context.Store.LoadAsync("/p/format9.nupdproj"));
        await Should.ThrowAsync<UnsupportedFormatException>(() => _context.Store.LoadAsync("/p/formatx.nupdproj"));
        (await _context.Store.LoadAsync("/p/nulls.nupdproj")).Project.Packages.ShouldBeEmpty();
        await Should.ThrowAsync<ArgumentException>(() => _context.Store.LoadAsync(" "));
        await Should.ThrowAsync<FileNotFoundException>(() => _context.Store.LoadAsync("/p/missing.nupdproj"));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Store.SaveAsync(null!));
        await Should.ThrowAsync<ArgumentException>(() => _context.Store.SaveAsync(new UpdateProject()));
        Should.Throw<ArgumentNullException>(() => new ProjectStore(null!, _context.Paths, _context.Protector));
        Should.Throw<ArgumentNullException>(() => new ProjectStore(_context.FileSystem, null!, _context.Protector));
        Should.Throw<ArgumentNullException>(() => new ProjectStore(_context.FileSystem, _context.Paths, null!));
        Should.Throw<ArgumentNullException>(() => new ProjectLoadResult(null!, new ProjectSecrets(), false, SecretsState.Loaded));
        Should.Throw<ArgumentNullException>(() => new ProjectLoadResult(new UpdateProject(), null!, false, SecretsState.Loaded));
    }

    [Theory]
    [InlineData("1b2")]
    [InlineData("3b2")]
    [InlineData("v3")]
    public async Task Load_MigratesEveryShippedFormatVersion(string configVersion)
    {
        var json = LegacyProjectJson(true, LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword).Replace("\"ConfigVersion\": \"v3\"", $"\"ConfigVersion\": \"{configVersion}\"");
        _context.FileSystem.AddFile("/old/Legacy.nupdproj", new MockFileData(json));
        var result = await _context.Store.LoadAsync("/old/Legacy.nupdproj");
        result.Migrated.ShouldBeTrue();
        result.Project.Name.ShouldBe("Legacy");
        result.Secrets.TransferPassword.ShouldBe("ftp-pw");
    }

    [Fact]
    public async Task Load_MigratesProjectsFromBefore1Beta2WithVersionObjects()
    {
        // Before 1.0 Beta 2 there was no ConfigVersion and package versions were objects whose stage counted 0 = Release, 1 = Beta, 2 = Alpha.
        const string json = """
            {"Name":"Ancient","Guid":"12345678-1234-1234-1234-123456789abc","UpdateUrl":"https://updates.example.com/ancient/","FtpHost":"h","FtpUsername":"u","FtpProtocol":0,"SaveCredentials":true,
             "PublicKey":"pk","NewestPackage":"1.1.0.0",
             "Packages":[
               {"Version":{"Major":1,"Minor":0,"Build":0,"Revision":0,"DevelopmentalStage":0,"DevelopmentBuild":0},"Description":"first","IsReleased":true},
               {"Version":{"Major":1,"Minor":1,"Build":0,"Revision":0,"DevelopmentalStage":1,"DevelopmentBuild":2},"IsReleased":false},
               {"Version":{"Major":1,"Minor":2},"DevelopmentalStage":2},
               {"Version":{"Major":1,"Minor":3,"DevelopmentalStage":2,"DevelopmentBuild":1}},
               {"Version":7}],
             "Log":[{"Entry":0,"EntryTime":"2015-01-02 03:04:05","PackageVersion":"1.0.0.0"}]}
            """;
        _context.FileSystem.AddFile("/old/Ancient.nupdproj", new MockFileData(json));

        var result = await _context.Store.LoadAsync("/old/Ancient.nupdproj");

        result.Migrated.ShouldBeTrue();
        result.Project.Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0-beta.2", "1.2.0", "1.3.0-alpha.1"]);
        result.Project.Log.Single().User.ShouldBe("");
        ProjectMigrator.IsV3(null).ShouldBeTrue();
        ProjectMigrator.IsV3("V3").ShouldBeTrue();
        ProjectMigrator.IsV3("4").ShouldBeFalse();
        ProjectMigrator.IsV5("V5").ShouldBeTrue();
        ProjectMigrator.IsV5(null).ShouldBeFalse();
    }

    [Fact]
    public async Task Load_RejectsTheUnreleased2017Format()
    {
        _context.FileSystem.AddFile("/old/four.nupdproj", new MockFileData("""{"ConfigVersion":"4","Name":"Four"}"""));
        (await Should.ThrowAsync<UnsupportedFormatException>(() => _context.Store.LoadAsync("/old/four.nupdproj"))).Message.ShouldContain("\"4\"");
    }
}
