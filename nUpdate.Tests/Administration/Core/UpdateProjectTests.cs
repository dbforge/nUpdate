using nUpdate.Administration.Core.Models;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class UpdateProjectTests
{
    [Fact]
    public void UpdateProject_DerivesUrlsAndFoldersFromUpdateUrlAndPath()
    {
        var folder = Path.Combine(Path.GetTempPath(), "projects", "demo");
        var project = new UpdateProject { UpdateUrl = "https://h/u", Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Path = Path.Combine(folder, UpdateProject.FileName) };
        project.FeedUri.ToString().ShouldBe("https://h/u/nupdate.json");
        project.Resolve("packages/1.0.0.zip").ToString().ShouldBe("https://h/u/packages/1.0.0.zip");
        project.Format.ShouldBe(6);
        project.Folder.ShouldBe(folder);
        project.PackagesDirectory.ShouldBe(Path.Combine(folder, "packages"));
        project.PackageDirectory(new UpdateVersion("1.0.0")).ShouldBe(Path.Combine(folder, "packages", "1.0.0"));
        project.PlatformDirectory(new UpdateVersion("1.0.0-beta.1"), "linux").ShouldBe(Path.Combine(folder, "packages", "1.0.0-beta.1", "linux"));
        project.PackageFilePath(new UpdateVersion("1.0.0-beta.1"), "win-x64").ShouldBe(Path.Combine(folder, "packages", "1.0.0-beta.1", "win-x64", "win-x64.zip"));
        Should.Throw<ArgumentNullException>(() => project.PlatformDirectory(new UpdateVersion("1.0.0"), null!));
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.0.0") });
        project.FindPackage(new UpdateVersion("1.0.0")).ShouldNotBeNull();
        project.FindPackage(new UpdateVersion("2.0.0")).ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => project.FindPackage(null!));
        Should.Throw<ArgumentNullException>(() => project.PackageDirectory(null!));
        Should.Throw<ArgumentNullException>(() => project.Resolve(null!));
        project.UpdateUrl = "nope";
        Should.Throw<InvalidOperationException>(() => project.FeedUri);
        new UpdateProject().Folder.ShouldBe("");
        UpdateProject.NormalizeUpdateUrl(" https://h/u ").ShouldBe("https://h/u/");
        UpdateProject.NormalizeUpdateUrl("https://h/u/").ShouldBe("https://h/u/");
        Should.Throw<ArgumentNullException>(() => UpdateProject.NormalizeUpdateUrl(null!));
    }

    [Fact]
    public void UpdateProject_SerializesCamelCaseWithoutDerivedProperties()
    {
        var json = Serializer.Serialize(new UpdateProject { Path = "/secret/path", Packages = [new UpdatePackage { Version = new UpdateVersion("1.2.0-beta.1") }], Log = [new LogEntry { Kind = LogEntryKind.Upload }] });
        json.ShouldNotContain("/secret/path");
        json.ShouldNotContain("feedUri");
        json.ShouldNotContain("folder");
        json.ShouldContain("\"format\":6");
        json.ShouldContain("\"version\":\"1.2.0-beta.1\"");
        json.ShouldContain("\"kind\":\"upload\"");
        json.ShouldContain("\"protocol\":\"sftp\"");
    }

    [Theory]
    [InlineData("https://updates.example.com/app", true)]
    [InlineData(" http://updates.example.com/app/ ", true)]
    [InlineData("ftp://updates.example.com/app", false)]
    [InlineData("file:///C:/updates", false)]
    [InlineData("updates.example.com/app", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidUpdateUrl_AcceptsOnlyHttpUrls(string? url, bool valid)
    {
        UpdateProject.IsValidUpdateUrl(url).ShouldBe(valid);
    }
}
