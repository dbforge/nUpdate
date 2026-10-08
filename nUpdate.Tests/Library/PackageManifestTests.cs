using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Tests.Library.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class PackageManifestTests
{
    [Fact]
    public void PackageManifest_RoundTripsAndListsTouchedAreas()
    {
        var manifest = new PackageManifest
        {
            ProjectId = TestFeed.ProjectId,
            Version = new UpdateVersion("2.0.0-beta.1"),
            Platform = "osx-arm64",
            CreatedAt = new DateTimeOffset(2026, 10, 7, 8, 0, 0, TimeSpan.Zero),
            Operations = [new TerminateProcessOperation { ProcessName = "app" }, new DeleteFilesOperation { Directory = "%program%", Files = ["old.dll"] }, new StopServiceOperation { ServiceName = "svc" }],
        };
        manifest.Touches.ShouldBe([OperationArea.Files, OperationArea.Processes, OperationArea.Services]);
        var json = Serializer.Serialize(manifest);
        json.ShouldStartWith("{\"format\":1,\"projectId\":\"8f3c0a2e-5b1d-4e8a-9c7f-2d6b1e4a9f10\",\"version\":\"2.0.0-beta.1\",\"platform\":\"osx-arm64\"");
        json.ShouldNotContain("touches");
        var restored = Serializer.Deserialize<PackageManifest>(json)!;
        restored.Operations.Select(o => o.Type).ShouldBe(["terminateProcess", "deleteFiles", "stopService"]);
        restored.CreatedAt.ShouldBe(manifest.CreatedAt);
        restored.Platform.ShouldBe("osx-arm64");
        new PackageManifest().Touches.ShouldBeEmpty();
        new PackageManifest().Platform.ShouldBe(PackagePlatform.Any);
    }
}
