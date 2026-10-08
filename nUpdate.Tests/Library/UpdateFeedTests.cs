using nUpdate.Operations;
using nUpdate.Tests.Library.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class UpdateFeedTests
{
    [Fact]
    public void UpdateFeed_Serialization_WritesTheDocumentedShape()
    {
        var feed = new UpdateFeed
        {
            ProjectId = TestFeed.ProjectId,
            Packages =
            [
                new PackageInfo
                {
                    Version = new UpdateVersion("1.0.0"),
                    Changelog = { ["en"] = "First" },
                    Files = [new PackageFile { Path = "packages/1.0.0/any.zip", Size = 10, Sha512 = "aGFzaA==", Signature = new PackageSignature { Value = "c2ln" }, Touches = [OperationArea.Services] }],
                },
            ],
        };
        var json = Serializer.Serialize(feed);
        json.ShouldStartWith("{\"format\":1,\"projectId\":\"8f3c0a2e-5b1d-4e8a-9c7f-2d6b1e4a9f10\",\"packages\":[{\"version\":\"1.0.0\"");
        json.ShouldNotContain("architecture");
        json.ShouldContain("\"rollout\":{\"mode\":\"any\",\"conditions\":[]}");
        json.ShouldContain("\"files\":[{\"platform\":\"any\",\"path\":\"packages/1.0.0/any.zip\",\"size\":10,\"sha512\":\"aGFzaA==\",\"signature\":{\"algorithm\":\"rsa-pss-sha512\",\"value\":\"c2ln\"},\"touches\":[\"services\"]}]");
        json.ShouldContain("\"necessary\":false,\"afterInstall\":null,");
        json.ShouldContain("\"statistics\":null");
        FeedLoader.Parse(json).Packages.Single().Statistics.ShouldBeNull();
    }
}
