using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class PathPlaceholderResolverTests
{
    [Fact]
    public void Resolve_ReplacesPlaceholdersAndKeepsRepeatedSegments()
    {
        var services = new TestInstallerServices();
        var resolver = new PathPlaceholderResolver(services.FileSystem, services.AppDirectory, services.SpecialFolders);
        string combine(params string[] parts) => services.FileSystem.Path.Combine(parts);

        resolver.Resolve("%program%").ShouldBe(services.AppDirectory);
        resolver.Resolve("%program%\\bin\\app.dll").ShouldBe(combine(services.AppDirectory, "bin", "app.dll"));
        resolver.Resolve("%PROGRAM%/bin/app.dll").ShouldBe(combine(services.AppDirectory, "bin", "app.dll"));
        resolver.Resolve("%appdata%\\Vendor").ShouldBe(combine(services.Root("appdata"), "Vendor"));
        resolver.Resolve("%temp%\\x").ShouldBe(combine(services.Root("temp"), "x"));
        resolver.Resolve("%desktop%\\link.lnk").ShouldBe(combine(services.Root("desktop"), "link.lnk"));
        resolver.Resolve("%program%\\%program%\\same").ShouldBe(combine(services.AppDirectory, "%program%", "same"));
        resolver.Resolve("%program%\\\\double").ShouldBe(combine(services.AppDirectory, "double"));
        resolver.Resolve("C:\\absolute\\path").ShouldBe("C:\\absolute\\path");
        resolver.Resolve("").ShouldBe("");
        Should.Throw<ArgumentNullException>(() => resolver.Resolve(null!));
    }

    [Fact]
    public void Constructor_ValidatesArguments()
    {
        var services = new TestInstallerServices();
        Should.Throw<ArgumentNullException>(() => new PathPlaceholderResolver(null!, "/app", services.SpecialFolders));
        Should.Throw<ArgumentNullException>(() => new PathPlaceholderResolver(services.FileSystem, null!, services.SpecialFolders));
        Should.Throw<ArgumentNullException>(() => new PathPlaceholderResolver(services.FileSystem, "/app", null!));
    }
}
