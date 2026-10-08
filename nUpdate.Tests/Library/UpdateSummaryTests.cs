using nUpdate.Operations;
using nUpdate.Tests.Library.Support;
using nUpdate.Tests.Support;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public sealed class UpdateSummaryTests : IDisposable
{
    private const string FeedUri = "https://h/u/nupdate.json";
    private readonly TestServices _services = new();
    private readonly UpdateManager _manager;

    public UpdateSummaryTests()
    {
        _manager = new UpdateManager(new Uri(FeedUri), TestKeys.PublicKey, services: _services.Build());
    }

    public void Dispose() => _manager.Dispose();

    private async Task CheckAsync(params (string Version, OperationArea[] Touches)[] packages)
    {
        var feed = new UpdateFeed
        {
            Packages = packages.Select(p => new PackageInfo
            {
                Version = new UpdateVersion(p.Version),
                Necessary = true,
                Changelog = { ["en"] = $"Changes in {p.Version}." },
                Files = [new PackageFile { Path = $"packages/{p.Version}/any.zip", Size = 2048, Sha512 = "x", Signature = new PackageSignature { Value = "s" }, Touches = p.Touches.ToList() }],
            }).ToList(),
        };
        _services.Http.Text(HttpMethod.Get, FeedUri, Serializer.Serialize(feed));
        (await _manager.CheckForUpdatesAsync()).ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateSummary_DescribesOneUpdateWithWhatItTouches()
    {
        await CheckAsync(("1.1.0", [OperationArea.Processes]));

        var summary = new UpdateSummary(_manager, "|");

        summary.Header.ShouldBe("1 new update available.");
        summary.InfoText.ShouldBe("New updates can be downloaded for TestApp.");
        summary.AvailableVersionsText.ShouldBe("Available versions: 1.1.0");
        summary.CurrentVersionText.ShouldBe("Current version: 1.0.0");
        summary.UpdateSizeText.ShouldStartWith("Total package size: 2");
        summary.TouchesText.ShouldBe("Accesses: Processes");
        summary.ChangelogText.ShouldContain("Changes in 1.1.0.");
        Should.Throw<ArgumentNullException>(() => new UpdateSummary(null!, "\n"));
    }

    [Fact]
    public async Task UpdateSummary_DescribesSeveralUpdatesThatTouchNothing()
    {
        await CheckAsync(("1.1.0", []), ("1.2.0", []));

        var summary = new UpdateSummary(_manager, "|");

        summary.Header.ShouldBe("2 new updates available.");
        summary.TouchesText.ShouldBe("Accesses: -");
        summary.ChangelogText.ShouldContain("|");
    }
}
