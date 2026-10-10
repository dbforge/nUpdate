using System.IO.Abstractions.TestingHelpers;
using System.Net;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public sealed class FeedCheckerTests
{
    private const string Base = "https://updates.example.com/demo/";
    private readonly AdminTestContext _context = new();
    private readonly UpdateProject _project;
    private readonly FeedChecker _checker;

    public FeedCheckerTests()
    {
        _project = _context.NewProject();
        _checker = new FeedChecker(_context.FileSystem, _context.HttpClientFactory, _context.Feeds, _context.Signer,
            _context.Statistics);
    }

    /// <summary>Serves a correctly signed package and returns its feed entry.</summary>
    private PackageInfo Serve(string version, byte[]? zip = null, string? path = null, Guid? projectId = null,
        string platform = "any", string? manifestPlatform = null)
    {
        zip ??= TestPackages.Build(version, projectId ?? _project.Id, [1, 2, 3],
            platform: manifestPlatform ?? platform);
        var local = _context.FileSystem.Path.Combine(_context.FileSystem.Path.GetTempPath(),
            $"signed-{version}-{platform}.zip");
        _context.FileSystem.AddFile(local, new MockFileData(zip));
        path ??= $"packages/{version}/{platform}.zip";
        _context.Http.Bytes(path.StartsWith("http", StringComparison.Ordinal) ? path : Base + path, zip);
        return new PackageInfo
        {
            Version = new UpdateVersion(version),
            Files =
            [
                new PackageFile
                {
                    Platform = platform,
                    Path = path,
                    Size = zip.Length,
                    Sha512 = _context.Signer.Hash(local),
                    Signature = new PackageSignature { Value = _context.Signer.Sign(local, TestKeys.PrivateKey) },
                },
            ],
        };
    }

    [Fact]
    public async Task Check_DownloadsAndVerifiesEveryPackageLikeAClient()
    {
        var relative = Serve("1.0.0");
        relative.Files.AddRange(Serve("1.0.0", platform: "linux-arm64").Files);
        var absolute = Serve("1.1.0", path: "https://mirror.example.com/1.1.0.zip");
        _context.ServeFeed(new UpdateFeed { ProjectId = _project.Id, Packages = [absolute, relative] });
        var progress = new List<nUpdate.Administration.Core.Publishing.PipelineProgress>();

        var result = await _checker.CheckAsync(_project, AdminTestContext.NewSecrets(),
            new SyncProgress<PipelineProgress>(progress));

        result.Succeeded.ShouldBeTrue();
        result.FeedProblem.ShouldBeNull();
        result.Packages.Select(p => (p.Version.ToString(), p.Platform, p.Uri.ToString(), p.Problem)).ShouldBe([
            ("1.0.0", "any", Base + "packages/1.0.0/any.zip", null),
            ("1.0.0", "linux-arm64", Base + "packages/1.0.0/linux-arm64.zip", null),
            ("1.1.0", "any", "https://mirror.example.com/1.1.0.zip", null),
        ]);
        result.StatisticsChecked.ShouldBeFalse();
        result.StatisticsProblem.ShouldBeNull();
        progress.Select(p => p.StepName).ShouldBe([
            "Reading nupdate.json", "Checking 1.0.0 for any", "Checking 1.0.0 for linux-arm64", "Checking 1.1.0 for any"
        ]);
        _context.FileSystem.Directory.GetFiles(_context.FileSystem.Path.GetTempPath(), "nupdate-check-*")
            .ShouldBeEmpty();
    }

    [Fact]
    public async Task Check_ReportsWhatAClientWouldReject()
    {
        var missing = Serve("1.0.0");
        _context.Http.Text(HttpMethod.Get, Base + "packages/1.0.0/any.zip", "gone", HttpStatusCode.NotFound);
        var resized = Serve("1.1.0");
        resized.Files[0].Size++;
        var rehashed = Serve("1.2.0");
        rehashed.Files[0].Sha512 = "AAAA";
        var unsigned = Serve("1.3.0");
        unsigned.Files[0].Signature.Value = Convert.ToBase64String(new byte[512]);
        var garbled = Serve("1.4.0");
        garbled.Files[0].Signature.Value = "not base64!";
        var foreignAlgorithm = Serve("1.5.0");
        foreignAlgorithm.Files[0].Signature.Algorithm = "rsa-pkcs1-sha512";
        var noManifest = Serve("1.6.0", TestPackages.WithoutManifest([1]));
        var otherProject = Serve("1.7.0", projectId: Guid.NewGuid());
        var otherPlatform = Serve("1.7.1", platform: "win", manifestPlatform: "osx");
        var notAZip = Serve("1.8.0", [1, 2, 3, 4]);
        var badManifest = Serve("1.9.0", TestPackages.Build("1.9.0", _project.Id, [1], manifestJson: "{broken"));
        var unreachable = Serve("2.0.0");
        _context.Http.On(r => r.RequestUri!.ToString() == Base + "packages/2.0.0/any.zip",
            (_, _) => throw new HttpRequestException("connection reset"));
        var slow = Serve("2.1.0");
        _context.Http.On(r => r.RequestUri!.ToString() == Base + "packages/2.1.0/any.zip",
            (_, _) => throw new TaskCanceledException("timeout"));
        _context.ServeFeed(new UpdateFeed
        {
            ProjectId = _project.Id,
            Packages =
            [
                missing, resized, rehashed, unsigned, garbled, foreignAlgorithm, noManifest, otherProject,
                otherPlatform, notAZip, badManifest, unreachable, slow
            ]
        });

        var result = await _checker.CheckAsync(_project, AdminTestContext.NewSecrets());

        result.Succeeded.ShouldBeFalse();
        var problems = result.Packages.Select(p => p.Problem!).ToList();
        problems[0].ShouldBe("The server answered 404 (Not Found).");
        problems[1].ShouldContain("the feed announces");
        problems[2].ShouldBe("The SHA-512 hash does not match the feed.");
        problems[3].ShouldBe("The signature does not match the public key of the project.");
        problems[4].ShouldBe("The signature is not valid Base64.");
        problems[5].ShouldContain("rsa-pkcs1-sha512");
        problems[6].ShouldBe("The package has no manifest.json.");
        problems[7].ShouldContain("another project, version or platform");
        problems[8].ShouldContain("another project, version or platform");
        problems[9].ShouldStartWith("The package cannot be read");
        problems[10].ShouldStartWith("The package cannot be read");
        problems[11].ShouldBe("connection reset");
        problems[12].ShouldBe("The download did not finish in time.");
    }

    [Fact]
    public async Task Check_ReportsAFeedThatCannotBeRead()
    {
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        var missing = await _checker.CheckAsync(_project, secrets);
        missing.FeedProblem.ShouldBe($"There is no nupdate.json at {Base}nupdate.json.");
        missing.Packages.ShouldBeEmpty();
        missing.Succeeded.ShouldBeFalse();

        _context.Http.Text(HttpMethod.Get, Base + "nupdate.json", "{broken");
        (await _checker.CheckAsync(_project, secrets)).FeedProblem.ShouldNotBeNullOrEmpty();
        _context.Http.Text(HttpMethod.Get, Base + "nupdate.json", """{"format":9,"packages":[]}""");
        (await _checker.CheckAsync(_project, secrets)).FeedProblem!.ShouldContain("format 9");
        _context.Http.Text(HttpMethod.Get, Base + "nupdate.json", "error", HttpStatusCode.InternalServerError);
        (await _checker.CheckAsync(_project, secrets)).FeedProblem!.ShouldContain("500");
        _context.Http.On(r => r.RequestUri!.ToString() == Base + "nupdate.json",
            (_, _) => throw new TaskCanceledException("timeout"));
        (await _checker.CheckAsync(_project, secrets)).FeedProblem.ShouldBe(
            $"{Base}nupdate.json did not answer in time.");

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _checker.CheckAsync(_project, secrets, null, cancelled.Token));
    }

    [Fact]
    public async Task Check_AsksTheStatisticsApiWhenTheProjectHasStatistics()
    {
        var project = _context.NewProject(statistics: true);
        _context.ServeFeed(new UpdateFeed { ProjectId = project.Id });

        var withoutSecret = await _checker.CheckAsync(project, AdminTestContext.NewSecrets());
        withoutSecret.StatisticsChecked.ShouldBeTrue();
        withoutSecret.StatisticsProblem!.ShouldContain("admin secret is missing");

        var fine = await _checker.CheckAsync(project, AdminTestContext.NewSecrets(statistics: true));
        fine.StatisticsProblem.ShouldBeNull();
        fine.Succeeded.ShouldBeTrue();
        await _context.Statistics.Received()
            .VerifyAsync(Arg.Is<StatisticsEndpoint>(e => e.Uri.ToString() == Base + "nupdate-statistics.php"),
                Arg.Any<CancellationToken>());

        _context.Statistics.VerifyAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new StatisticsException("no PATH_INFO"));
        (await _checker.CheckAsync(project, AdminTestContext.NewSecrets(statistics: true))).StatisticsProblem.ShouldBe(
            "no PATH_INFO");
    }

    [Fact]
    public async Task Constructor_ValidatesArguments()
    {
        var c = _context;
        Should.Throw<ArgumentNullException>(() =>
            new FeedChecker(null!, c.HttpClientFactory, c.Feeds, c.Signer, c.Statistics));
        Should.Throw<ArgumentNullException>(() =>
            new FeedChecker(c.FileSystem, null!, c.Feeds, c.Signer, c.Statistics));
        Should.Throw<ArgumentNullException>(() =>
            new FeedChecker(c.FileSystem, c.HttpClientFactory, null!, c.Signer, c.Statistics));
        Should.Throw<ArgumentNullException>(() =>
            new FeedChecker(c.FileSystem, c.HttpClientFactory, c.Feeds, null!, c.Statistics));
        Should.Throw<ArgumentNullException>(() =>
            new FeedChecker(c.FileSystem, c.HttpClientFactory, c.Feeds, c.Signer, null!));
        await Should.ThrowAsync<ArgumentNullException>(() => _checker.CheckAsync(null!, new ProjectSecrets()));
        await Should.ThrowAsync<ArgumentNullException>(() => _checker.CheckAsync(_project, null!));
        Should.Throw<ArgumentNullException>(() => new FeedCheckResult(null, null!, false, null));
        Should.Throw<ArgumentNullException>(() => new PackageCheck(null!, "any", new Uri("https://x/"), null));
        Should.Throw<ArgumentNullException>(() =>
            new PackageCheck(new UpdateVersion("1.0.0"), null!, new Uri("https://x/"), null));
        Should.Throw<ArgumentNullException>(() => new PackageCheck(new UpdateVersion("1.0.0"), "any", null!, null));
    }
}
