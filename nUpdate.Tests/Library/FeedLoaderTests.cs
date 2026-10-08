using System.IO.Abstractions.TestingHelpers;
using System.Net;
using nUpdate.Exceptions;
using nUpdate.Operations;
using nUpdate.Tests.Library.Support;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class FeedLoaderTests
{
    private const string FeedJson = """
        {
          "format": 1,
          "projectId": "8f3c0a2e-5b1d-4e8a-9c7f-2d6b1e4a9f10",
          "packages": [
            {
              "version": "2.1.0-beta.1",
              "publishedAt": "2026-10-05T14:12:00+00:00",
              "necessary": true,
              "changelog": { "en": "English", "de-DE": "Deutsch" },
              "unsupportedVersions": ["1.0.0"],
              "rollout": { "mode": "all", "conditions": [{ "key": "Region", "value": "EU", "negated": false }] },
              "files": [
                {
                  "platform": "win-x64",
                  "path": "packages/2.1.0-beta.1/win-x64.zip",
                  "size": 1234,
                  "sha512": "aGFzaA==",
                  "signature": { "algorithm": "rsa-pss-sha512", "value": "c2ln" },
                  "touches": ["files", "registry"]
                },
                {
                  "platform": "linux",
                  "path": "packages/2.1.0-beta.1/linux.zip",
                  "size": 1000,
                  "sha512": "aGFzaA==",
                  "signature": { "algorithm": "rsa-pss-sha512", "value": "c2ln" },
                  "touches": null
                }
              ],
              "statistics": { "url": "statistics.php", "enabled": true }
            }
          ]
        }
        """;

    [Fact]
    public void Parse_ReadsEveryField()
    {
        var feed = FeedLoader.Parse(FeedJson);
        feed.Format.ShouldBe(1);
        feed.ProjectId.ShouldBe(TestFeed.ProjectId);
        var package = feed.Packages.ShouldHaveSingleItem();
        package.Version.ShouldBe(new UpdateVersion("2.1.0-beta.1"));
        package.PublishedAt.ShouldBe(new DateTimeOffset(2026, 10, 5, 14, 12, 0, TimeSpan.Zero));
        package.Necessary.ShouldBeTrue();
        package.Changelog["de-DE"].ShouldBe("Deutsch");
        package.UnsupportedVersions.ShouldBe([new UpdateVersion("1.0.0")]);
        package.Rollout.Mode.ShouldBe(RolloutConditionMode.All);
        package.Rollout.Conditions.Single().Key.ShouldBe("Region");
        var file = package.Files[0];
        file.Platform.ShouldBe("win-x64");
        file.Touches.ShouldBe([OperationArea.Files, OperationArea.Registry]);
        file.Path.ShouldBe("packages/2.1.0-beta.1/win-x64.zip");
        file.Size.ShouldBe(1234);
        file.Sha512.ShouldBe("aGFzaA==");
        file.Signature.Algorithm.ShouldBe(PackageSignature.RsaPssSha512);
        file.Signature.Value.ShouldBe("c2ln");
        package.Files[1].Touches.ShouldBeEmpty();
        package.Statistics!.Url.ShouldBe("statistics.php");
        package.Statistics.Enabled.ShouldBeTrue();
    }

    [Fact]
    public void Parse_RejectsBrokenFeeds()
    {
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse(null)).Message.ShouldContain("empty");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("  "));
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("null"));
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("{not json")).Message.ShouldContain("not valid JSON");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"nope"}]}""")).Message.ShouldContain("nope");
        Should.Throw<UnsupportedFormatException>(() => FeedLoader.Parse("""{"format":2,"packages":[]}""")).Message.ShouldContain("format 2");
        Should.Throw<UnsupportedFormatException>(() => FeedLoader.Parse("""[{"LiteralVersion":"1.0"}]""")).Message.ShouldContain("nUpdate 3 or 4");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("{\"format\":1,\"packages\":[" + Package("1.0.0", "a", "x") + "," + Package("1.0.0+build.2", "b", "y") + "]}")).Message.ShouldContain("more than once");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse(Entry("1.0.0", path: ""))).Message.ShouldContain("any file of the package \"1.0.0\" has no path");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse(Entry("1.0.0", signature: ""))).Message.ShouldContain("no signature");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse(Entry("1.0.0", platform: " "))).Message.ShouldContain("without a platform");
        FeedLoader.Parse("""{"format":1,"packages":[]}""").Packages.ShouldBeEmpty();
        FeedLoader.Parse("""{"format":1,"packages":null}""").Packages.ShouldBeEmpty();
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[null]}""")).Message.ShouldContain("without a version");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":null}]}""")).Message.ShouldContain("without a version");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":null}]}""")).Message.ShouldContain("has no files");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[]}]}""")).Message.ShouldContain("has no files");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[null]}]}""")).Message.ShouldContain("without a platform");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[{"platform":"win","path":"p","signature":null}]}]}""")).Message.ShouldContain("no signature");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[{"platform":"win","path":"p","signature":{"value":"s"}},{"platform":"WIN","path":"q","signature":{"value":"s"}}]}]}""")).Message.ShouldContain("more than one file for the platform \"WIN\"");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[{"platform":"any","path":"p","signature":{"value":"s"}}],"unsupportedVersions":[null]}]}""")).Message.ShouldContain("empty entry");
        Should.Throw<InvalidFeedException>(() => FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[{"platform":"any","path":"p","signature":{"value":"s"}}],"rollout":{"conditions":[null]}}]}""")).Message.ShouldContain("empty entry");
        var sparse = FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[{"platform":"any","path":"p","signature":{"value":"s"},"touches":null}],"changelog":null,"unsupportedVersions":null,"rollout":null}]}""").Packages.Single();
        sparse.Changelog.ShouldBeEmpty();
        sparse.UnsupportedVersions.ShouldBeEmpty();
        sparse.Rollout.Conditions.ShouldBeEmpty();
        sparse.Files.Single().Touches.ShouldBeEmpty();
        FeedLoader.Parse("""{"format":1,"packages":[{"version":"1.0.0","files":[{"platform":"any","path":"p","signature":{"value":"s"}}],"rollout":{"mode":"all","conditions":null}}]}""").Packages.Single().Rollout.Conditions.ShouldBeEmpty();
    }

    private static string Entry(string version, string path = "packages/x.zip", string signature = "c2ln", string platform = "any") =>
        "{\"format\":1,\"packages\":[" + Package(version, path, signature, platform) + "]}";

    private static string Package(string version, string path, string signature, string platform = "any") =>
        "{\"version\":\"" + version + "\",\"files\":[{\"platform\":\"" + platform + "\",\"path\":\"" + path + "\",\"size\":1,\"sha512\":\"x\",\"signature\":{\"value\":\"" + signature + "\"}}]}";

    [Fact]
    public void FromFile_ReadsThroughFileSystem()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/u/nupdate.json", new MockFileData(FeedJson));
        FeedLoader.FromFile(fileSystem, "/u/nupdate.json").Packages.Count.ShouldBe(1);
        Should.Throw<ArgumentNullException>(() => FeedLoader.FromFile(null!, "/x"));
    }

    [Fact]
    public async Task Load_DownloadsAndParses()
    {
        var http = new StubHttpMessageHandler().Text(HttpMethod.Get, "https://h/u/nupdate.json", FeedJson);
        using var client = http.CreateClient();
        (await FeedLoader.LoadAsync(client, new Uri("https://h/u/nupdate.json"))).Packages.Count.ShouldBe(1);

        http.Text(HttpMethod.Get, "https://h/u/missing.json", "", HttpStatusCode.NotFound);
        (await Should.ThrowAsync<HttpRequestException>(() => FeedLoader.LoadAsync(client, new Uri("https://h/u/missing.json")))).Message.ShouldContain("404");
        await Should.ThrowAsync<ArgumentNullException>(() => FeedLoader.LoadAsync(client, null!));
        await Should.ThrowAsync<ArgumentNullException>(() => FeedLoader.LoadAsync(null!, new Uri("https://h")));
    }
}
