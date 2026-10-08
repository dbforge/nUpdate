using nUpdate.Administration.Core.Packages;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class PackageDefinitionTests
{
    [Fact]
    public void GetOrAddPlatform_HasOnePackagePerKnownPlatform()
    {
        var definition = new PackageDefinition(new UpdateVersion("1.0.0"));
        var linux = definition.GetOrAddPlatform("linux");
        definition.GetOrAddPlatform("linux").ShouldBeSameAs(linux);
        definition.GetOrAddPlatform("win-x64");
        definition.Platforms.Select(p => p.Platform).ShouldBe(["linux", "win-x64"]);
        Should.Throw<ArgumentException>(() => new PlatformPackage("freebsd-x64")).Message.ShouldContain("not a platform nUpdate knows");
        Should.Throw<ArgumentException>(() => new PlatformPackage(null!));
    }
}
