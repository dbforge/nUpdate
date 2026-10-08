using System.Net;
using nUpdate.Administration.Core.Packages;
using nUpdate.Tests.Administration.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class FeedStoreTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public async Task FeedStore_LoadsRemoteAndHandlesMissingFiles()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(new UpdateFeed { ProjectId = project.Id, Packages = [new PackageInfo { Version = new UpdateVersion("1.0.0"), Files = [new PackageFile { Path = "packages/1.0.0/any.zip", Signature = new PackageSignature { Value = "s" } }] }] });
        (await _context.Feeds.LoadRemoteAsync(project, secrets))!.Packages.Single().Version.ShouldBe(new UpdateVersion("1.0.0"));

        _context.ServeFeed(null);
        (await _context.Feeds.LoadRemoteAsync(project, secrets)).ShouldBeNull();

        _context.Http.Text(HttpMethod.Get, "https://updates.example.com/demo/nupdate.json", "boom", HttpStatusCode.InternalServerError);
        await Should.ThrowAsync<HttpRequestException>(() => _context.Feeds.LoadRemoteAsync(project, secrets));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.LoadRemoteAsync(null!, secrets));

        _context.ServeLegacyFeed("[]");
        (await _context.Feeds.LegacyFeedExistsAsync(project, secrets)).ShouldBeTrue();
        _context.ServeLegacyFeed(null);
        (await _context.Feeds.LegacyFeedExistsAsync(project, secrets)).ShouldBeFalse();
        _context.Http.Text(HttpMethod.Get, "https://updates.example.com/demo/updates.json", "boom", HttpStatusCode.Forbidden);
        await Should.ThrowAsync<HttpRequestException>(() => _context.Feeds.LegacyFeedExistsAsync(project, secrets));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.LegacyFeedExistsAsync(null!, secrets));
    }

    [Fact]
    public async Task FeedStore_SavesAndLoadsEntriesAndUploadsTheFeed()
    {
        var project = _context.NewProject();
        var version = new UpdateVersion("1.0.0");
        (await _context.Feeds.LoadEntryAsync(project, version)).ShouldBeNull();
        await _context.Feeds.SaveEntryAsync(project, new PackageInfo { Version = version, Necessary = true });
        (await _context.Feeds.LoadEntryAsync(project, version))!.Necessary.ShouldBeTrue();
        _context.FileSystem.File.Exists(_context.FileSystem.Path.Combine(project.PackageDirectory(version), "feed-entry.json")).ShouldBeTrue();

        string? uploadedContent = null;
        _context.Transfer.UploadFileAsync(Arg.Any<string>(), "nupdate.json", null, Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                uploadedContent = _context.FileSystem.File.ReadAllText(call.ArgAt<string>(0));
                return Task.CompletedTask;
            });
        await _context.Feeds.UploadAsync(_context.Transfer, new UpdateFeed { ProjectId = project.Id, Packages = [new PackageInfo { Version = new UpdateVersion("2.0.0") }] });
        uploadedContent!.ShouldContain("\"version\": \"2.0.0\"");
        uploadedContent!.ShouldContain("\"format\": 1");
        _context.FileSystem.Directory.GetFiles(_context.FileSystem.Path.GetTempPath(), "nupdate-*.json").ShouldBeEmpty();

        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.SaveEntryAsync(null!, new PackageInfo()));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.SaveEntryAsync(project, null!));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.LoadEntryAsync(null!, version));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.LoadEntryAsync(project, null!));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.UploadAsync(null!, new UpdateFeed()));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Feeds.UploadAsync(_context.Transfer, null!));
        Should.Throw<ArgumentNullException>(() => new FeedStore(null!, _context.HttpClientFactory));
        Should.Throw<ArgumentNullException>(() => new FeedStore(_context.FileSystem, null!));
    }
}
