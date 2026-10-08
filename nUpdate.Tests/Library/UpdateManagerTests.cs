using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using System.Net;
using Microsoft.Extensions.Logging;
using nUpdate.Exceptions;
using nUpdate.Installer;
using nUpdate.Tests.Library.Support;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class UpdateManagerTests
{
    private const string FeedUri = "https://h/u/nupdate.json";
    private const string ReportUri = "https://h/u/statistics.php/v2/downloads";
    private static readonly Guid ProjectId = Guid.Parse("11111111-2222-3333-4444-555555555555");
    private readonly TestServices _services = new();

    private UpdateManager Create(CultureInfo? culture = null, UpdateVersion? current = null, bool injectHttp = true) =>
        new(new Uri(FeedUri), TestKeys.PublicKey, culture, current, _services.Build(injectHttp));

    private static byte[] Package(int seed, int size = 300)
    {
        var data = new byte[size];
        new Random(seed).NextBytes(data);
        return data;
    }

    /// <summary>Serves a package zip with the payload and a matching manifest and returns the feed entry that announces it.</summary>
    private PackageInfo Publish(string version, byte[] payload, bool statistics = false, bool necessary = false, byte[]? served = null, Guid? manifestProject = null,
        string? manifestVersion = null, string platform = PackagePlatform.Any, string? manifestPlatform = null, AfterInstall? afterInstall = null)
    {
        var path = $"packages/{new UpdateVersion(version)}/{platform}.zip";
        var package = TestPackages.Build(manifestVersion ?? version, manifestProject ?? ProjectId, payload, platform: manifestPlatform ?? platform);
        _services.Http.Bytes("https://h/u/" + path, served is null ? package : TestPackages.Build(version, ProjectId, served, platform: platform));
        return new PackageInfo
        {
            Version = new UpdateVersion(version),
            Necessary = necessary,
            AfterInstall = afterInstall,
            Files = [File(path, package, platform)],
            Statistics = statistics ? new PackageStatistics { Url = "statistics.php" } : null,
        };
    }

    private static PackageFile File(string path, byte[] package, string platform = PackagePlatform.Any) =>
        new() { Platform = platform, Path = path, Size = package.Length, Sha512 = TestKeys.Sha512(package), Signature = new PackageSignature { Value = TestKeys.Sign(package) } };

    private void ServeFeed(params PackageInfo[] packages) =>
        _services.Http.Text(HttpMethod.Get, FeedUri, Serializer.Serialize(new UpdateFeed { ProjectId = ProjectId, Packages = packages.ToList() }));

    private IEnumerable<string> Warnings() => _services.Logger.Entries.Where(e => e.Level == LogLevel.Warning).Select(e => e.Message);

    // --- construction ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("TestApp.exe")]
    public void Constructor_TakesAnExecutablePathItCannotUseAsUnknown(string? derived)
    {
        _services.ApplicationInfo.ExecutablePath.Returns(derived);
        using var manager = Create();
        manager.ApplicationExecutablePath.ShouldBeNull();
        manager.InstallerPath.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("MyApp.exe")]
    [InlineData("bin/MyApp")]
    public void ApplicationExecutablePath_MustBeAbsolute(string path)
    {
        using var manager = Create();
        Should.Throw<ArgumentException>(() => manager.ApplicationExecutablePath = path).Message.ShouldContain("must be absolute");
        manager.ApplicationExecutablePath.ShouldBe(_services.ApplicationInfo.ExecutablePath);
    }

    [Fact]
    public void Constructor_DefaultsToEnglishAndEntryAssemblyFacts()
    {
        using var manager = Create();
        manager.Culture.Name.ShouldBe("en");
        manager.Texts.Cancel.ShouldBe("Cancel");
        manager.CurrentVersion.ShouldBe(new UpdateVersion("1.0.0"));
        manager.ApplicationName.ShouldBe("TestApp");
        manager.ApplicationExecutablePath.ShouldBe(_services.ApplicationInfo.ExecutablePath);
        manager.DownloadDirectory.ShouldBe(_services.FileSystem.Path.Combine(_services.FileSystem.Path.GetTempPath(), "nUpdate", "TestApp"));
        manager.PublicKey.ShouldBe(TestKeys.PublicKey);
        manager.FeedUri.ToString().ShouldBe(FeedUri);
        manager.AvailableUpdates.ShouldBeEmpty();
        manager.DownloadedPackages.ShouldBeEmpty();
        manager.TotalDownloadSize.ShouldBe(0);
        manager.DefaultAfterInstall.ShouldBe(AfterInstall.Restart);
        manager.AfterInstall.ShouldBe(AfterInstall.Restart);
        manager.RunInstallerAsAdmin.ShouldBeTrue();
        manager.ReportDownloads.ShouldBeTrue();
        manager.MinimumStability.ShouldBe(Stability.Release);
        manager.AcceptedPreReleaseLabels.ShouldBeEmpty();
        manager.RolloutConditions.ShouldBeEmpty();
        manager.HttpTimeout.ShouldBe(TimeSpan.FromSeconds(100));
    }

    [Fact]
    public void Constructor_AcceptsExplicitCultureAndVersion()
    {
        using var manager = Create(new CultureInfo("de-DE"), new UpdateVersion("2.0.0"));
        manager.Culture.Name.ShouldBe("de-DE");
        manager.Texts.Cancel.ShouldBe("Abbrechen");
        manager.CurrentVersion.ShouldBe(new UpdateVersion("2.0.0"));
    }

    [Fact]
    public void Constructor_ValidatesArguments()
    {
        Should.Throw<ArgumentNullException>(() => new UpdateManager(null!, TestKeys.PublicKey));
        Should.Throw<ArgumentException>(() => new UpdateManager(new Uri("relative", UriKind.Relative), TestKeys.PublicKey));
        Should.Throw<ArgumentNullException>(() => new UpdateManager(new Uri(FeedUri), " ", services: _services.Build()));
        Create(new CultureInfo("fr-FR")).Culture.Name.ShouldBe("fr-FR");

        _services.ApplicationInfo.DeclaredVersion.Returns((string?)null);
        Should.Throw<InvalidOperationException>(() => Create()).Message.ShouldContain(nameof(ApplicationVersionAttribute));
        _services.ApplicationInfo.DeclaredVersion.Returns("1.0.0");
        Create(current: null).CurrentVersion.ShouldBe(new UpdateVersion("1.0.0"));
        // An attribute still written the nUpdate 4 way is reported with the form that is expected now.
        _services.ApplicationInfo.DeclaredVersion.Returns("1.0.0.0b2");
        var invalid = Should.Throw<InvalidOperationException>(() => Create(current: null));
        invalid.Message.ShouldContain("\"1.0.0.0b2\"");
        invalid.Message.ShouldContain("major.minor.patch");
    }

    [Fact]
    public void Constructor_UsesProductionServicesWhenNoneGiven()
    {
        // The entry assembly of the test host has no ApplicationVersionAttribute, so a version must be given.
        using var manager = new UpdateManager(new Uri(FeedUri), TestKeys.PublicKey, currentVersion: new UpdateVersion("1.0.0"));
        manager.ApplicationName.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Culture_SupportsCustomFilesAndRejectsUnknownCultures()
    {
        using var manager = Create();
        _services.FileSystem.AddFile("/loc/fr.json", new MockFileData("""{"cancel":"Annuler"}"""));
        manager.TextFiles[new CultureInfo("fr-FR")] = "/loc/fr.json";
        manager.Culture = new CultureInfo("fr-FR");
        manager.Texts.Cancel.ShouldBe("Annuler");

        manager.Culture = new CultureInfo("fr-CA");
        manager.Texts.Cancel.ShouldBe("Annuler");
        manager.Culture.Name.ShouldBe("fr-CA");
        Should.Throw<ArgumentNullException>(() => manager.Culture = null!);
        manager.Culture.Name.ShouldBe("fr-CA");

        // Unknown cultures fall back to English; parents and siblings of the shipped German files are found.
        manager.Culture = new CultureInfo("pt-BR");
        manager.Texts.Cancel.ShouldBe("Cancel");
        manager.Culture = new CultureInfo("de-LI");
        manager.Texts.Cancel.ShouldBe("Abbrechen");
        manager.Culture = new CultureInfo("de");
        manager.Texts.Cancel.ShouldBe("Abbrechen");
        manager.Culture = new CultureInfo("en-GB");
        manager.Texts.Cancel.ShouldBe("Cancel");
        manager.Culture = new CultureInfo("it");
        manager.Texts.Cancel.ShouldBe("Annulla");
        manager.Culture = CultureInfo.InvariantCulture;
        manager.Texts.Cancel.ShouldBe("Cancel");
    }

    [Fact]
    public void UserAgent_ContainsProductOsAndArchitecture()
    {
        using var manager = Create();
        manager.UserAgent().ShouldStartWith("TestApp/1.0.0.0 (Windows 11; win-x64; nUpdate/");
        _services.SystemInformation.RuntimeIdentifier.Returns("win-x86");
        manager.UserAgent().ShouldContain("; win-x86;");
    }

    // --- check ---

    [Fact]
    public async Task CheckForUpdates_SelectsPackagesAndSumsSizesFromTheFeed()
    {
        var first = Package(1, 100);
        var second = Package(2, 250);
        ServeFeed(Publish("1.1.0", first, necessary: true), Publish("1.2.0", second), Publish("0.5.0", Package(3)));
        using var manager = Create();

        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        manager.AvailableUpdates.Select(c => c.Version.ToString()).ShouldBe(["1.1.0", "1.2.0"]);
        manager.TotalDownloadSize.ShouldBe(manager.AvailableUpdates.Sum(p => p.Files[0].Size));
        manager.TotalDownloadSize.ShouldBeGreaterThan(350);
        _services.Http.Requests.ShouldAllBe(r => r.Method == HttpMethod.Get);
    }

    [Fact]
    public async Task CheckForUpdates_ReturnsFalseWhenUpToDate()
    {
        ServeFeed(Publish("1.0.0", Package(1)));
        using var manager = Create();
        (await manager.CheckForUpdatesAsync()).ShouldBeFalse();
        manager.AvailableUpdates.ShouldBeEmpty();
    }

    [Fact]
    public async Task CheckForUpdates_AppliesPolicyLabelsAndRolloutConditions()
    {
        var beta = Publish("1.1.0-beta.1", Package(1), necessary: true);
        var nightly = Publish("1.1.0-nightly.2", Package(2), necessary: true);
        var regional = Publish("1.2.0", Package(3), necessary: true);
        regional.Rollout.Conditions.Add(new RolloutCondition("Region", "EU"));
        ServeFeed(beta, nightly, regional);
        using var manager = Create();

        (await manager.CheckForUpdatesAsync()).ShouldBeFalse();
        manager.MinimumStability = Stability.Beta;
        manager.AcceptedPreReleaseLabels.Add("nightly");
        manager.RolloutConditions["Region"] = "eu";
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        manager.AvailableUpdates.Select(c => c.Version.ToString()).ShouldBe(["1.1.0-beta.1", "1.1.0-nightly.2", "1.2.0"]);

        regional.Files[0].Platform = "linux";
        ServeFeed(regional);
        (await manager.CheckForUpdatesAsync()).ShouldBeFalse();
    }

    [Fact]
    public async Task CheckForUpdates_TakesTheMostSpecificFileForThePlatform()
    {
        _services.SystemInformation.RuntimeIdentifier.Returns("linux-arm64");
        var any = Publish("1.1.0", Package(1));
        var linux = Publish("1.1.0", Package(2), platform: "linux");
        var arm = Publish("1.1.0", Package(3, 500), platform: "linux-arm64");
        var windowsOnly = Publish("1.2.0", Package(4), platform: "win-x64");
        any.Files.AddRange(linux.Files.Concat(arm.Files));
        ServeFeed(any, windowsOnly);
        using var manager = Create();
        manager.Platform.ShouldBe("linux-arm64");

        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        manager.AvailableUpdates.Single().Version.ShouldBe(new UpdateVersion("1.1.0"));
        manager.TotalDownloadSize.ShouldBe(arm.Files[0].Size);
        await manager.DownloadAsync();
        _services.Http.Requests.Select(r => r.RequestUri!.ToString()).ShouldContain("https://h/u/packages/1.1.0/linux-arm64.zip");
        (await manager.VerifyAsync()).ShouldBeTrue();

        _services.SystemInformation.RuntimeIdentifier.Returns("linux-x64");
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        manager.TotalDownloadSize.ShouldBe(linux.Files[0].Size);
        _services.SystemInformation.RuntimeIdentifier.Returns("win-x64");
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        manager.AvailableUpdates.Select(p => p.Version.ToString()).ShouldBe(["1.2.0"]);
    }

    [Fact]
    public async Task CheckForUpdates_ResolvesRelativeAndAbsolutePackagePaths()
    {
        var relative = Publish("1.1.0", Package(1), necessary: true);
        var absolute = Publish("1.2.0", Package(2), necessary: true);
        absolute.Files[0].Path = "https://mirror/elsewhere/1.2.0.zip";
        _services.Http.Bytes("https://mirror/elsewhere/1.2.0.zip", TestPackages.Build("1.2.0", ProjectId, Package(2)));
        // A root-relative path is relative to the feed's host, not a file URI, even on Unix.
        var rooted = Publish("1.3.0", Package(3), necessary: true);
        rooted.Files[0].Path = "/u/packages/1.3.0/any.zip";
        ServeFeed(relative, absolute, rooted);
        using var manager = Create();

        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        await manager.DownloadAsync();
        _services.Http.Requests.Select(r => r.RequestUri!.ToString()).ShouldContain("https://h/u/packages/1.1.0/any.zip");
        _services.Http.Requests.Select(r => r.RequestUri!.ToString()).ShouldContain("https://mirror/elsewhere/1.2.0.zip");
        _services.Http.Requests.Select(r => r.RequestUri!.ToString()).ShouldContain("https://h/u/packages/1.3.0/any.zip");
    }

    [Fact]
    public async Task Download_StopsAsSoonAsAPackageGrowsBeyondTheAnnouncedSize()
    {
        var oversized = Publish("1.1.0", Package(1, 100), necessary: true, served: Package(1, 400_000));
        ServeFeed(oversized);
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        (await Should.ThrowAsync<InvalidPackageException>(() => manager.DownloadAsync())).Message.ShouldContain("larger");
        manager.DownloadedPackages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Download_LogsAStatisticsUrlThatCannotBeUsed()
    {
        var broken = Publish("1.1.0", Package(1), statistics: true, necessary: true);
        broken.Statistics!.Url = "http://[::1";
        var fileScheme = Publish("1.2.0", Package(2), statistics: true, necessary: true);
        fileScheme.Statistics!.Url = "file:///tmp/statistics";
        ServeFeed(broken, fileScheme);
        using var manager = Create();
        await manager.CheckForUpdatesAsync();

        await manager.DownloadAsync();

        manager.DownloadedPackages.Count.ShouldBe(2);
        Warnings().Count().ShouldBe(2);
    }

    [Fact]
    public async Task CheckForUpdates_ThrowsForBrokenFeeds()
    {
        _services.Http.Text(HttpMethod.Get, FeedUri, "{broken");
        using var manager = Create();
        await Should.ThrowAsync<InvalidFeedException>(() => manager.CheckForUpdatesAsync());

        _services.Http.Text(HttpMethod.Get, FeedUri, """{"format":9,"packages":[]}""");
        await Should.ThrowAsync<UnsupportedFormatException>(() => manager.CheckForUpdatesAsync());

        _services.Http.Text(HttpMethod.Get, FeedUri, "", HttpStatusCode.NotFound);
        await Should.ThrowAsync<HttpRequestException>(() => manager.CheckForUpdatesAsync());
        manager.AvailableUpdates.ShouldBeEmpty();
    }

    [Fact]
    public async Task CheckForUpdates_HonoursCancellation()
    {
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        using var cts = new CancellationTokenSource();
        _services.Http.On(r => r.Method == HttpMethod.Get, (_, _) =>
        {
            cts.Cancel();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Serializer.Serialize(new UpdateFeed())) });
        });
        _services.Http.On(HttpMethod.Get, FeedUri, (_, token) =>
        {
            cts.Cancel();
            token.ThrowIfCancellationRequested();
            return Task.FromResult(new HttpResponseMessage());
        });
        await Should.ThrowAsync<OperationCanceledException>(() => manager.CheckForUpdatesAsync(cts.Token));
    }

    [Fact]
    public async Task CheckForUpdates_CreatesOwnHttpClientWhenNoneInjected()
    {
        using var manager = Create(injectHttp: false);
        manager.HttpTimeout = TimeSpan.FromMilliseconds(1);
        manager.Proxy = new WebProxy("http://127.0.0.1:1");
        manager.HttpAuthenticationCredentials = new NetworkCredential("u", "p");
        await Should.ThrowAsync<Exception>(() => manager.CheckForUpdatesAsync());
    }

    // --- download ---

    [Fact]
    public async Task Download_StoresFilesReportsProgressAndStatistics()
    {
        var first = Package(1, 100);
        var second = Package(2, 200);
        ServeFeed(Publish("1.1.0", first, statistics: true, necessary: true), Publish("1.2.0", second, statistics: true));
        _services.Http.On(HttpMethod.Post, ReportUri, _ => new HttpResponseMessage(HttpStatusCode.NoContent));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();

        var reports = new List<UpdateDownloadProgress>();
        await manager.DownloadAsync(new SynchronousProgress(reports));

        manager.DownloadedPackages.Count.ShouldBe(2);
        _services.FileSystem.File.ReadAllBytes(manager.DownloadedPackages[new UpdateVersion("1.1.0")]).ShouldBe(TestPackages.Build("1.1.0", ProjectId, first));
        _services.FileSystem.File.ReadAllBytes(manager.DownloadedPackages[new UpdateVersion("1.2.0")]).ShouldBe(TestPackages.Build("1.2.0", ProjectId, second));
        manager.DownloadedPackages[new UpdateVersion("1.1.0")].ShouldEndWith("1.1.0.zip");
        reports.Last().BytesReceived.ShouldBe(manager.TotalDownloadSize);
        reports.Last().TotalBytesToReceive.ShouldBe(manager.TotalDownloadSize);
        reports.Last().Percentage.ShouldBe(100f);

        var bodies = _services.Http.RequestBodies.Where(b => b.Contains("projectId")).ToList();
        bodies.Count.ShouldBe(2);
        bodies[0].ShouldBe($$"""{"projectId":"{{ProjectId}}","version":"1.1.0","os":"Windows 11"}""");
        Warnings().ShouldBeEmpty();
    }

    [Fact]
    public async Task Download_SkipsStatisticsWhenDisabledOrOptedOut()
    {
        var withoutUrl = Publish("1.1.0", Package(1), statistics: true, necessary: true);
        withoutUrl.Statistics!.Url = " ";
        var excluded = Publish("1.2.0", Package(2), statistics: true, necessary: true);
        excluded.Statistics!.Enabled = false;
        var optedOut = Publish("1.3.0", Package(3), statistics: true, necessary: true);
        var noStatistics = Publish("1.4.0", Package(4));
        ServeFeed(withoutUrl, excluded, noStatistics);
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        _services.Http.Requests.ShouldNotContain(r => r.Method == HttpMethod.Post);

        ServeFeed(optedOut);
        manager.ReportDownloads = false;
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        _services.Http.Requests.ShouldNotContain(r => r.Method == HttpMethod.Post);
        Warnings().ShouldBeEmpty();
    }

    [Fact]
    public async Task Download_StatisticsFailureIsLoggedButDoesNotAbort()
    {
        ServeFeed(Publish("1.1.0", Package(1), statistics: true));
        _services.Http.Text(HttpMethod.Post, ReportUri, "boom", HttpStatusCode.InternalServerError);
        using var manager = Create();
        await manager.CheckForUpdatesAsync();

        await manager.DownloadAsync();

        manager.DownloadedPackages.Count.ShouldBe(1);
        var warning = _services.Logger.Entries.Single(e => e.Level == LogLevel.Warning);
        warning.Message.ShouldContain("1.1.0");
        warning.Exception.ShouldBeOfType<HttpRequestException>().Message.ShouldContain("500");
    }

    [Fact]
    public async Task Download_StatisticsTimeoutIsLogged()
    {
        ServeFeed(Publish("1.1.0", Package(1), statistics: true));
        _services.Http.On(HttpMethod.Post, ReportUri, (_, _) => throw new TaskCanceledException("timeout"));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        Warnings().Count().ShouldBe(1);
    }

    [Fact]
    public async Task Download_CancellationDuringStatisticsPropagates()
    {
        ServeFeed(Publish("1.1.0", Package(1), statistics: true));
        using var cts = new CancellationTokenSource();
        _services.Http.On(HttpMethod.Post, ReportUri, (_, _) =>
        {
            cts.Cancel();
            throw new OperationCanceledException(cts.Token);
        });
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await Should.ThrowAsync<OperationCanceledException>(() => manager.DownloadAsync(cancellationToken: cts.Token));
        manager.DownloadedPackages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Download_UnexpectedStatisticsErrorPropagates()
    {
        ServeFeed(Publish("1.1.0", Package(1), statistics: true));
        _services.Http.On(HttpMethod.Post, ReportUri, (_, _) => throw new InvalidOperationException("unexpected"));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await Should.ThrowAsync<InvalidOperationException>(() => manager.DownloadAsync());
    }

    [Fact]
    public async Task Download_RejectsPackagesThatDoNotMatchTheFeed()
    {
        var package = Package(1, 200);
        var wrongHash = Publish("1.1.0", package, necessary: true, served: Package(2, 200));
        ServeFeed(wrongHash);
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        (await Should.ThrowAsync<InvalidPackageException>(() => manager.DownloadAsync())).Message.ShouldContain("hash");
        manager.DownloadedPackages.ShouldBeEmpty();
        _services.FileSystem.Directory.GetFiles(manager.DownloadDirectory).ShouldBeEmpty();

        var wrongSize = Publish("1.2.0", package, necessary: true, served: Package(1, 150));
        ServeFeed(wrongSize);
        await manager.CheckForUpdatesAsync();
        (await Should.ThrowAsync<InvalidPackageException>(() => manager.DownloadAsync())).Message.ShouldContain("bytes, but the feed announced");
    }

    [Fact]
    public async Task Download_DeletesPartialFilesOnCancellationAndFailure()
    {
        var gone = Publish("1.2.0", Package(2));
        _services.Http.Text(HttpMethod.Get, "https://h/u/packages/1.2.0/any.zip", "gone", HttpStatusCode.Gone);
        ServeFeed(Publish("1.1.0", Package(1), necessary: true), gone);
        using var manager = Create();
        await manager.CheckForUpdatesAsync();

        await Should.ThrowAsync<HttpRequestException>(() => manager.DownloadAsync());
        manager.DownloadedPackages.ShouldBeEmpty();
        _services.FileSystem.Directory.GetFiles(manager.DownloadDirectory).ShouldBeEmpty();

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(() => manager.DownloadAsync(cancellationToken: cts.Token));
        manager.DownloadedPackages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Download_Timeouts_AreReportedAsFailuresNotCancellations()
    {
        _services.Http.On(HttpMethod.Get, FeedUri, (_, _) => throw new TaskCanceledException("timed out"));
        using var manager = Create();
        (await Should.ThrowAsync<HttpRequestException>(() => manager.CheckForUpdatesAsync())).Message.ShouldContain("timed out");

        ServeFeed(Publish("1.1.0", Package(1)));
        await manager.CheckForUpdatesAsync();
        _services.Http.On(HttpMethod.Get, "https://h/u/packages/1.1.0/any.zip", (_, _) => throw new TaskCanceledException("timed out"));
        (await Should.ThrowAsync<HttpRequestException>(() => manager.DownloadAsync())).Message.ShouldContain("packages/1.1.0/any.zip");
        manager.DownloadedPackages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Download_ClearsStaleFilesAndLogsWhatItCannotRemove()
    {
        ServeFeed(Publish("1.1.0", Package(1, 500_000)));
        using var manager = Create();
        var fs = _services.FileSystem;
        fs.AddFile(fs.Path.Combine(manager.DownloadDirectory, "0.9.0.zip"), new MockFileData("stale"));
        fs.AddFile(fs.Path.Combine(manager.DownloadDirectory, "pinned.zip"), new MockFileData("stale") { Attributes = FileAttributes.ReadOnly });
        await manager.CheckForUpdatesAsync();

        using var cts = new CancellationTokenSource();
        var progress = new SynchronousProgress([])
        {
            OnReport = () =>
            {
                fs.GetFile(manager.DownloadedPackages.Values.Single()).Attributes = FileAttributes.ReadOnly;
                cts.Cancel();
            },
        };
        await Should.ThrowAsync<OperationCanceledException>(() => manager.DownloadAsync(progress, cts.Token));

        manager.DownloadedPackages.ShouldBeEmpty();
        fs.File.Exists(fs.Path.Combine(manager.DownloadDirectory, "0.9.0.zip")).ShouldBeFalse();
        fs.File.Exists(fs.Path.Combine(manager.DownloadDirectory, "pinned.zip")).ShouldBeTrue();
        Warnings().ShouldContain(w => w.Contains("pinned.zip"));
        Warnings().ShouldContain(w => w.Contains("1.1.0.zip"));
    }

    [Fact]
    public async Task Download_CancelledMidStreamDeletesFile()
    {
        ServeFeed(Publish("1.1.0", Package(1, 500_000)));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        using var cts = new CancellationTokenSource();
        var progress = new SynchronousProgress([]) { OnReport = () => cts.Cancel() };
        await Should.ThrowAsync<OperationCanceledException>(() => manager.DownloadAsync(progress, cts.Token));
        manager.DownloadedPackages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Download_RequiresCheck()
    {
        using var manager = Create();
        await Should.ThrowAsync<InvalidOperationException>(() => manager.DownloadAsync());
    }

    [Fact]
    public async Task Download_TwiceReplacesPreviousFiles()
    {
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        await manager.DownloadAsync();
        manager.DownloadedPackages.Count.ShouldBe(1);
        _services.FileSystem.Directory.GetFiles(manager.DownloadDirectory).Length.ShouldBe(1);
    }

    // --- verification ---

    [Fact]
    public async Task Verify_AcceptsAuthenticPackagesAndRejectsTamperedOnes()
    {
        ServeFeed(Publish("1.1.0", Package(1), necessary: true), Publish("1.2.0", Package(2)));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeTrue();
        manager.DownloadedPackages.Count.ShouldBe(2);

        var path = manager.DownloadedPackages[new UpdateVersion("1.2.0")];
        _services.FileSystem.File.WriteAllBytes(path, TestPackages.Build("1.2.0", ProjectId, Package(99)));
        (await manager.VerifyAsync()).ShouldBeFalse();
        manager.DownloadedPackages.ShouldBeEmpty();
        _services.FileSystem.Directory.GetFiles(manager.DownloadDirectory).ShouldBeEmpty();
        await Should.ThrowAsync<InvalidOperationException>(() => manager.VerifyAsync());
    }

    [Fact]
    public async Task Verify_RejectsASignedPackageWhoseManifestDoesNotMatchTheFeedEntry()
    {
        // A feed that relabels an old, validly signed package as a newer version (or another project's package) is caught by the manifest.
        ServeFeed(Publish("2.0.0", Package(1), necessary: true, manifestVersion: "1.0.0"));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeFalse();

        ServeFeed(Publish("2.0.0", Package(1), necessary: true, manifestProject: Guid.NewGuid()));
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeFalse();

        // The feed must not hand out the signed package of another platform.
        ServeFeed(Publish("2.0.0", Package(1), necessary: true, platform: "win", manifestPlatform: "linux"));
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeFalse();
        ServeFeed(Publish("2.0.0", Package(1), necessary: true, platform: "win", manifestPlatform: "WIN"));
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeTrue();

        // Packages without a manifest, or with an unreadable one, are not accepted either.
        var legacy = TestPackages.WithoutManifest(Package(1));
        var legacyEntry = new PackageInfo { Version = new UpdateVersion("2.0.0"), Necessary = true, Files = [File("packages/2.0.0/any.zip", legacy)] };
        _services.Http.Bytes("https://h/u/packages/2.0.0/any.zip", legacy);
        ServeFeed(legacyEntry);
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeFalse();

        var broken = TestPackages.Build("2.0.0", ProjectId, Package(1), manifestJson: "{broken");
        var brokenEntry = new PackageInfo { Version = new UpdateVersion("2.0.0"), Necessary = true, Files = [File("packages/2.0.0/any.zip", broken)] };
        _services.Http.Bytes("https://h/u/packages/2.0.0/any.zip", broken);
        ServeFeed(brokenEntry);
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeFalse();
        Warnings().ShouldContain(w => w.Contains("manifest", StringComparison.OrdinalIgnoreCase));

        var notAZip = Package(5);
        var notAZipEntry = new PackageInfo { Version = new UpdateVersion("2.0.0"), Necessary = true, Files = [File("packages/2.0.0/any.zip", notAZip)] };
        _services.Http.Bytes("https://h/u/packages/2.0.0/any.zip", notAZip);
        ServeFeed(notAZipEntry);
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeFalse();
    }

    [Fact]
    public async Task Verify_ReportsMissingFilesAndBadSignatureData()
    {
        var package = Publish("1.1.0", Package(1));
        ServeFeed(package);
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        _services.FileSystem.File.Delete(manager.DownloadedPackages.Values.Single());
        await Should.ThrowAsync<FileNotFoundException>(() => manager.VerifyAsync());

        await manager.DownloadAsync();
        manager.AvailableUpdates[0].Files[0].Signature.Value = "not base64!";
        (await Should.ThrowAsync<InvalidFeedException>(() => manager.VerifyAsync())).Message.ShouldContain("Base64");
        manager.AvailableUpdates[0].Files[0].Signature.Algorithm = "rsa-sha1";
        (await Should.ThrowAsync<InvalidFeedException>(() => manager.VerifyAsync())).Message.ShouldContain("rsa-sha1");

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(() => manager.VerifyAsync(cts.Token));
    }

    [Fact]
    public async Task Verify_ThrowsForInvalidPublicKey()
    {
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = new UpdateManager(new Uri(FeedUri), "<RSAKeyValue><Modulus>AQ==</Modulus></RSAKeyValue>", services: _services.Build());
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        await Should.ThrowAsync<ArgumentException>(() => manager.VerifyAsync());
    }

    // --- install ---

    private InstallerOptions StartedOptions() =>
        Serializer.Deserialize<InstallerOptions>(_services.FileSystem.File.ReadAllText(((string)_services.ProcessLauncher.ReceivedCalls().Single().GetArguments()[1]!).Trim('"')))!;

    [Fact]
    public async Task StartInstaller_CopiesInstallerWritesOptionsStartsElevatedAndTerminates()
    {
        _services.AddInstaller();
        ServeFeed(Publish("1.1.0", Package(1), necessary: true), Publish("1.2.0", Package(2)));
        using var manager = Create(new CultureInfo("de-DE"));
        manager.Arguments.Add(new InstallerArgument("--restarted", ArgumentCondition.Succeeded));
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        manager.StartInstaller().ShouldBeTrue();

        var call = _services.ProcessLauncher.ReceivedCalls().Single();
        var arguments = call.GetArguments();
        var exePath = (string)arguments[0]!;
        exePath.ShouldEndWith(TestServices.InstallerFileName);
        exePath.ShouldContain("nUpdate Installer");
        ((bool)arguments[2]!).ShouldBeTrue();
        var optionsPath = ((string)arguments[1]!).Trim('"');
        _services.FileSystem.File.Exists(optionsPath).ShouldBeTrue();
        var copied = _services.FileSystem.Path.GetDirectoryName(exePath)!;
        _services.FileSystem.File.Exists(_services.FileSystem.Path.Combine(copied, "de", "resources.dll")).ShouldBeTrue();
        _services.FileSystem.File.Exists(_services.FileSystem.Path.Combine(copied, "extra.dll")).ShouldBeTrue();
        _services.FilePermissions.DidNotReceive().CanWrite(Arg.Any<string>()); // Windows has no write check; setting the mode does nothing there

        var options = StartedOptions();
        options.Format.ShouldBe(InstallerOptions.CurrentFormat);
        options.Packages.Count.ShouldBe(2);
        options.Packages[0].Path.ShouldEndWith("1.1.0.zip");
        options.Application.Directory.ShouldBe(_services.AppDirectory);
        options.Application.ExecutablePath.ShouldBe(_services.ApplicationInfo.ExecutablePath);
        options.Application.Name.ShouldBe("TestApp");
        options.Application.Bundle.ShouldBeNull();
        options.Host.ProcessId.ShouldBe(4242);
        options.Host.AfterInstall.ShouldBe(AfterInstall.Restart);
        options.Arguments.Single().Value.ShouldBe("--restarted");
        options.Arguments.Single().When.ShouldBe(ArgumentCondition.Succeeded);
        options.Ui.ShowWindow.ShouldBeTrue();
        options.Ui.IconPath.ShouldBeNull();
        options.Ui.AccentColor.ShouldBeNull();
        options.Text(InstallerText.Copying).ShouldBe("Kopiere {0}...");
        options.Text(InstallerText.RetryButton).ShouldBe("Wiederholen");
        _services.ApplicationTerminator.Received(1).Terminate();
    }

    [Fact]
    public void InstallerPath_DefaultsToTheBuiltInInstallerOfTheRunningProcess()
    {
        using var manager = Create();
        var fs = _services.FileSystem;
        manager.InstallerPath.ShouldBe(fs.Path.Combine(_services.AppDirectory, "nUpdate.Installer", "win-x64", "nUpdate.UpdateInstaller.UI.Avalonia.exe"));

        _services.SystemInformation.RuntimeIdentifier.Returns("linux-arm64");
        manager.InstallerPath.ShouldBe(fs.Path.Combine(_services.AppDirectory, "nUpdate.Installer", "linux-arm64", "nUpdate.UpdateInstaller.UI.Avalonia"));

        manager.InstallerPath = "/opt/my/installer";
        manager.InstallerPath.ShouldBe("/opt/my/installer");
        manager.InstallerPath = null;
        manager.InstallerPath.ShouldEndWith("nUpdate.UpdateInstaller.UI.Avalonia");

        manager.ApplicationExecutablePath = null;
        manager.InstallerPath.ShouldBeNull();
        manager.ApplicationExecutablePath = "/";
        manager.InstallerPath.ShouldBe(fs.Path.Combine("nUpdate.Installer", "linux-arm64", "nUpdate.UpdateInstaller.UI.Avalonia"));
    }

    [Fact]
    public async Task StartInstaller_PassesTheWindowOptionsAndCopiesTheIcon()
    {
        _services.AddInstaller();
        var fs = _services.FileSystem;
        var icon = fs.Path.Combine(_services.AppDirectory, "icon.png");
        fs.AddFile(icon, new MockFileData([1, 2, 3]));
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        manager.ShowInstallerWindow = false;
        manager.InstallerIcon = icon;
        manager.InstallerAccentColor = "#FF3366cc";
        manager.InstallerAccentColor.ShouldBe("#FF3366cc");
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        manager.StartInstaller().ShouldBeTrue();

        var options = StartedOptions();
        options.Ui.ShowWindow.ShouldBeFalse();
        options.Ui.AccentColor.ShouldBe("#FF3366cc");
        options.Ui.IconPath.ShouldNotBe(icon);
        options.Ui.IconPath!.ShouldEndWith("installer-icon.png");
        fs.File.ReadAllBytes(options.Ui.IconPath).ShouldBe([1, 2, 3]);

        // A missing icon must not stop the update: the installer shows its own.
        _services.ProcessLauncher.ClearReceivedCalls();
        manager.InstallerIcon = fs.Path.Combine(_services.AppDirectory, "missing.png");
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        manager.StartInstaller().ShouldBeTrue();
        StartedOptions().Ui.IconPath.ShouldBeNull();
        Warnings().ShouldContain(w => w.Contains("missing.png", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("#12345")]
    [InlineData("#1234567")]
    [InlineData("123456")]
    [InlineData("#12345G")]
    [InlineData("red")]
    [InlineData("")]
    public void InstallerAccentColor_RejectsAnythingButHexColors(string color)
    {
        using var manager = Create();
        Should.Throw<ArgumentException>(() => manager.InstallerAccentColor = color).Message.ShouldContain("#RRGGBB");
        manager.InstallerAccentColor = "#abcdef";
        manager.InstallerAccentColor = null;
        manager.InstallerAccentColor.ShouldBeNull();
    }

    [Fact]
    public async Task StartInstaller_OnUnix_ChecksWriteAccessMakesTheCopyExecutableAndNeverElevates()
    {
        _services.SystemInformation.RuntimeIdentifier.Returns("linux-x64");
        var fs = _services.FileSystem;
        fs.AddFile(fs.Path.Combine(_services.AppDirectory, "nUpdate.Installer", "linux-x64", "nUpdate.UpdateInstaller.UI.Avalonia"), new MockFileData("elf"));
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        manager.RunInstallerAsAdmin.ShouldBeTrue();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        _services.FilePermissions.CanWrite(_services.AppDirectory).Returns(false);
        var denied = Should.Throw<UnauthorizedAccessException>(() => manager.StartInstaller());
        denied.Message.ShouldContain("TestApp cannot be updated");
        denied.Message.ShouldContain(_services.AppDirectory);
        _services.ProcessLauncher.ReceivedCalls().ShouldBeEmpty();

        _services.FilePermissions.CanWrite(_services.AppDirectory).Returns(true);
        manager.StartInstaller().ShouldBeTrue();
        var arguments = _services.ProcessLauncher.ReceivedCalls().Single().GetArguments();
        ((bool)arguments[2]!).ShouldBeFalse();
        _services.FilePermissions.Received(1).SetMode((string)arguments[0]!, 0x1ED);
        StartedOptions().Application.Bundle.ShouldBeNull();
    }

    [Fact]
    public async Task StartInstaller_OnLinux_TakesNoFolderForAMacOSBundle()
    {
        _services.SystemInformation.RuntimeIdentifier.Returns("linux-x64");
        _services.FilePermissions.CanWrite(Arg.Any<string>()).Returns(true);
        var fs = _services.FileSystem;
        fs.AddFile("/opt/Tool.app/Contents/MacOS/nUpdate.Installer/linux-x64/nUpdate.UpdateInstaller.UI.Avalonia", new MockFileData("elf"));
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        manager.ApplicationExecutablePath = "/opt/Tool.app/Contents/MacOS/tool";
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        manager.StartInstaller().ShouldBeTrue();
        StartedOptions().Application.Bundle.ShouldBeNull();
        _services.FilePermissions.Received().CanWrite(fs.Path.GetDirectoryName("/opt/Tool.app/Contents/MacOS/tool")!);
    }

    [Fact]
    public async Task StartInstaller_ForAMacOSBundle_PassesTheBundleAndChecksItsParentFolder()
    {
        _services.SystemInformation.RuntimeIdentifier.Returns("osx-arm64");
        var fs = _services.FileSystem;
        fs.AddFile("/Applications/Test App.app/Contents/MacOS/nUpdate.Installer/osx-arm64/nUpdate.UpdateInstaller.UI.Avalonia", new MockFileData("macho"));
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        manager.ApplicationExecutablePath = "/Applications/Test App.app/Contents/MacOS/TestApp";
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        // The paths are macOS paths; the expectations go through the file system so the test also passes on Windows.
        var applications = fs.Path.GetDirectoryName("/Applications/Test App.app")!;
        _services.FilePermissions.CanWrite(applications).Returns(false);
        Should.Throw<UnauthorizedAccessException>(() => manager.StartInstaller()).Message.ShouldContain($"\"{applications}\"");
        _services.FilePermissions.CanWrite(applications).Returns(true);
        manager.StartInstaller().ShouldBeTrue();
        var application = StartedOptions().Application;
        application.Bundle.ShouldBe("/Applications/Test App.app");
        application.Directory.ShouldBe(fs.Path.GetDirectoryName("/Applications/Test App.app/Contents/MacOS/TestApp"));
    }

    [Theory]
    [InlineData("/Applications/My App.app/Contents/MacOS/MyApp", "/Applications/My App.app")]
    [InlineData("/Users/me/Apps/X.APP/Contents/MacOS/x", "/Users/me/Apps/X.APP")]
    [InlineData("/opt/app/MyApp", null)]
    [InlineData("/Applications/My App.app/Contents/Resources/tool", null)]
    [InlineData("/Applications/MyApp/Contents/MacOS/MyApp", null)]
    [InlineData("/Applications/My App.app/Content/MacOS/MyApp", null)]
    [InlineData("MacOS/MyApp", null)]
    public void FindBundle_RecognizesExecutablesInsideAnAppBundle(string executable, string? bundle)
    {
        UpdateManager.FindBundle(executable).ShouldBe(bundle);
    }

    [Fact]
    public async Task StartInstaller_KeepRunning_DoesNotTerminateAndSendsNoProcessId()
    {
        _services.AddInstaller();
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        manager.DefaultAfterInstall = AfterInstall.KeepRunning;
        manager.RunInstallerAsAdmin = false;
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        manager.StartInstaller().ShouldBeTrue();
        _services.ApplicationTerminator.DidNotReceive().Terminate();
        StartedOptions().Host.ProcessId.ShouldBeNull();
        ((bool)_services.ProcessLauncher.ReceivedCalls().Single().GetArguments()[2]!).ShouldBeFalse();
    }

    [Fact]
    public async Task AfterInstall_FollowsThePackagesOverTheDefaultAndClosedWins()
    {
        using var manager = Create();
        manager.DefaultAfterInstall = AfterInstall.KeepRunning;

        ServeFeed(Publish("1.1.0", Package(1)));
        await manager.CheckForUpdatesAsync();
        manager.AfterInstall.ShouldBe(AfterInstall.KeepRunning);

        ServeFeed(Publish("1.1.0", Package(1), necessary: true), Publish("1.2.0", Package(2), afterInstall: AfterInstall.Restart));
        await manager.CheckForUpdatesAsync();
        manager.AfterInstall.ShouldBe(AfterInstall.Restart);

        ServeFeed(Publish("1.1.0", Package(1), necessary: true, afterInstall: AfterInstall.Close), Publish("1.2.0", Package(2), afterInstall: AfterInstall.Restart));
        await manager.CheckForUpdatesAsync();
        manager.AfterInstall.ShouldBe(AfterInstall.Close);

        // A package that restarts the application overrides an application that stays closed by default.
        manager.DefaultAfterInstall = AfterInstall.Close;
        ServeFeed(Publish("1.2.0", Package(2), afterInstall: AfterInstall.Restart));
        await manager.CheckForUpdatesAsync();
        manager.AfterInstall.ShouldBe(AfterInstall.Restart);
    }

    [Fact]
    public async Task StartInstaller_ClosesTheApplicationWhenAPackageAsksForIt()
    {
        _services.AddInstaller();
        ServeFeed(Publish("1.1.0", Package(1), afterInstall: AfterInstall.Close));
        using var manager = Create();
        manager.DefaultAfterInstall = AfterInstall.KeepRunning;
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        manager.StartInstaller().ShouldBeTrue();
        var host = StartedOptions().Host;
        host.AfterInstall.ShouldBe(AfterInstall.Close);
        host.ProcessId.ShouldBe(4242);
        _services.ApplicationTerminator.Received(1).Terminate();
    }

    [Fact]
    public async Task StartInstaller_ReusesOneInstallerFolderPerApplication()
    {
        var installer = _services.AddInstaller();
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        manager.StartInstaller().ShouldBeTrue();
        var first = (string)_services.ProcessLauncher.ReceivedCalls().Single().GetArguments()[0]!;
        var fs = _services.FileSystem;
        var leftover = fs.Path.Combine(fs.Path.GetDirectoryName(first)!, "leftover.tmp");
        fs.AddFile(leftover, new MockFileData("x"));

        _services.ProcessLauncher.ClearReceivedCalls();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        manager.StartInstaller().ShouldBeTrue();
        var second = (string)_services.ProcessLauncher.ReceivedCalls().Single().GetArguments()[0]!;

        second.ShouldBe(first);
        fs.File.Exists(leftover).ShouldBeFalse();
        fs.Directory.GetDirectories(fs.Path.Combine(fs.Path.GetTempPath(), "nUpdate Installer")).Length.ShouldBe(1);
        fs.File.Exists(fs.Path.Combine(installer, TestServices.InstallerFileName)).ShouldBeTrue();
    }

    [Fact]
    public async Task StartInstaller_UsesAnotherFolderWhileTheInstallerFolderIsInUse()
    {
        _services.AddInstaller();
        var fs = _services.FileSystem;
        var root = fs.Path.Combine(fs.Path.GetTempPath(), "nUpdate Installer");
        var folder = fs.Path.Combine(root, "TestApp");
        // An earlier installer still holds a file: deleting the folder fails as it did in #117.
        var held = fs.Path.Combine(folder, "held.dll");
        fs.AddFile(held, new MockFileData("x") { Attributes = FileAttributes.ReadOnly });
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        manager.StartInstaller().ShouldBeTrue();
        var started = (string)_services.ProcessLauncher.ReceivedCalls().Single().GetArguments()[0]!;
        var alternative = fs.Path.GetDirectoryName(started)!;
        fs.Path.GetDirectoryName(alternative).ShouldBe(root);
        fs.Path.GetFileName(alternative).ShouldMatch("^TestApp-[0-9a-f]{32}$");
        fs.File.Exists(held).ShouldBeTrue();
        Warnings().ShouldContain(w => w.Contains("still in use", StringComparison.Ordinal));

        // Once the folder is free again, the next update uses it and removes the other folders it can.
        fs.File.SetAttributes(held, FileAttributes.Normal);
        var stuck = fs.Path.Combine(root, "TestApp-" + new string('0', 32));
        fs.AddFile(fs.Path.Combine(stuck, "held.dll"), new MockFileData("x") { Attributes = FileAttributes.ReadOnly });
        var others = new[] { "TestApp-Pro", "TestApp-" + new string('z', 32), "TestApq-" + new string('0', 32) }
            .Select(name => fs.Path.Combine(root, name)).ToList(); // other applications' folders, or not ours
        others.ForEach(other => fs.AddDirectory(other));
        _services.ProcessLauncher.ClearReceivedCalls();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        manager.StartInstaller().ShouldBeTrue();
        fs.Path.GetDirectoryName((string)_services.ProcessLauncher.ReceivedCalls().Single().GetArguments()[0]!).ShouldBe(folder);
        fs.Directory.Exists(alternative).ShouldBeFalse();
        fs.Directory.Exists(stuck).ShouldBeTrue();
        others.ShouldAllBe(other => fs.Directory.Exists(other));
    }

    [Fact]
    public async Task StartInstaller_DeclinedElevation_DeletesDownloadsAndReturnsFalse()
    {
        _services.AddInstaller();
        _services.ProcessLauncher.Start(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(false);
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        manager.StartInstaller().ShouldBeFalse();
        manager.DownloadedPackages.ShouldBeEmpty();
        _services.ApplicationTerminator.DidNotReceive().Terminate();
    }

    [Fact]
    public async Task StartInstaller_StartsAnInstallerOfYourOwn()
    {
        _services.FileSystem.AddFile("/custom/installer/MyInstaller.exe", new MockFileData("exe"));
        _services.FileSystem.AddFile("/custom/installer/MyInstaller.dll", new MockFileData("dll"));
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        manager.InstallerPath = "/custom/installer/MyInstaller.exe";
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        manager.StartInstaller().ShouldBeTrue();
        var started = (string)_services.ProcessLauncher.ReceivedCalls().Single().GetArguments()[0]!;
        started.ShouldEndWith("MyInstaller.exe");
        started.ShouldContain("nUpdate Installer");
        _services.FileSystem.File.Exists(_services.FileSystem.Path.Combine(_services.FileSystem.Path.GetDirectoryName(started)!, "MyInstaller.dll")).ShouldBeTrue();
    }

    [Fact]
    public async Task StartInstaller_ReportsMissingPreconditions()
    {
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        Should.Throw<InvalidOperationException>(() => manager.StartInstaller());

        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        var ex = Should.Throw<FileNotFoundException>(() => manager.StartInstaller());
        ex.Message.ShouldContain(_services.FileSystem.Path.Combine("nUpdate.Installer", "win-x64", "nUpdate.UpdateInstaller.UI.Avalonia.exe"));
        ex.Message.ShouldContain("InstallerPath");

        manager.ApplicationExecutablePath = null;
        Should.Throw<InvalidOperationException>(() => manager.StartInstaller()).Message.ShouldContain("Environment.ProcessPath");
    }

    [Fact]
    public async Task StartInstaller_RefusesAnExecutablePathWithoutAFolder()
    {
        // An empty application folder used to reach the installer, which failed with "the path is empty" (#101).
        _services.FileSystem.AddFile("/custom/installer/" + TestServices.InstallerFileName, new MockFileData("exe"));
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        manager.InstallerPath = "/custom/installer/" + TestServices.InstallerFileName;
        manager.ApplicationExecutablePath = "/";
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();

        Should.Throw<InvalidOperationException>(() => manager.StartInstaller()).Message.ShouldContain("no application folder");
        _services.ProcessLauncher.ReceivedCalls().ShouldBeEmpty();
    }

    // --- delete and dispose ---

    [Fact]
    public async Task DeleteDownloads_LogsIoFailuresAndKeepsGoing()
    {
        ServeFeed(Publish("1.1.0", Package(1)));
        using var manager = Create();
        await manager.CheckForUpdatesAsync();
        await manager.DownloadAsync();
        var path = manager.DownloadedPackages.Values.Single();
        _services.FileSystem.File.SetAttributes(path, FileAttributes.ReadOnly);
        manager.DeleteDownloads();
        _services.Logger.Entries.Single().Exception.ShouldBeOfType<UnauthorizedAccessException>();
        manager.DownloadedPackages.ShouldBeEmpty();
        _services.FileSystem.File.Exists(path).ShouldBeTrue();

        await manager.CheckForUpdatesAsync();
        _services.FileSystem.File.SetAttributes(path, FileAttributes.Normal);
        await manager.DownloadAsync();
        _services.FileSystem.GetFile(path).AllowedFileShare = FileShare.None;
        manager.DeleteDownloads();
        _services.Logger.Entries.Last().Exception.ShouldBeOfType<IOException>();
        _services.FileSystem.GetFile(path).AllowedFileShare = FileShare.ReadWrite | FileShare.Delete;
        manager.DeleteDownloads();
    }

    [Fact]
    public async Task Dispose_PreventsFurtherUseAndIsIdempotent()
    {
        var manager = Create();
        manager.Dispose();
        manager.Dispose();
        await Should.ThrowAsync<ObjectDisposedException>(() => manager.CheckForUpdatesAsync());
        await Should.ThrowAsync<ObjectDisposedException>(() => manager.DownloadAsync());
        await Should.ThrowAsync<ObjectDisposedException>(() => manager.VerifyAsync());
        Should.Throw<ObjectDisposedException>(() => manager.StartInstaller());
    }

    [Fact]
    public void Dispose_DisposesOwnedHttpClientOnly()
    {
        var owned = Create(injectHttp: false);
        owned.HttpTimeout = TimeSpan.FromSeconds(1);
        _ = owned.CheckForUpdatesAsync().ContinueWith(_ => { }, TaskScheduler.Default);
        owned.Dispose();

        var injected = Create();
        injected.Dispose();
        _services.Http.CreateClient().ShouldNotBeNull();
    }
}
