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

        resolver.Resolve("%program%").ShouldBe(services.AppDirectory);
        resolver.Resolve("%program%\\bin\\app.dll").ShouldBe(Combine(services.AppDirectory, "bin", "app.dll"));
        resolver.Resolve("%PROGRAM%/bin/app.dll").ShouldBe(Combine(services.AppDirectory, "bin", "app.dll"));
        resolver.Resolve("%appdata%\\Vendor").ShouldBe(Combine(services.Root("appdata"), "Vendor"));
        resolver.Resolve("%temp%\\x").ShouldBe(Combine(services.Root("temp"), "x"));
        resolver.Resolve("%desktop%\\link.lnk").ShouldBe(Combine(services.Root("desktop"), "link.lnk"));
        resolver.Resolve("%program%\\%program%\\same").ShouldBe(Combine(services.AppDirectory, "%program%", "same"));
        resolver.Resolve("%program%\\\\double").ShouldBe(Combine(services.AppDirectory, "double"));
        resolver.Resolve("C:\\absolute\\path").ShouldBe("C:\\absolute\\path");
        resolver.Resolve("").ShouldBe("");
        Should.Throw<ArgumentNullException>(() => resolver.Resolve(null!));
        return;

        string Combine(params string[] parts) => services.FileSystem.Path.Combine(parts);
    }

    [Fact]
    public void Constructor_ValidatesArguments()
    {
        var services = new TestInstallerServices();
        Should.Throw<ArgumentNullException>(() => new PathPlaceholderResolver(null!, "/app", services.SpecialFolders));
        Should.Throw<ArgumentNullException>(() =>
            new PathPlaceholderResolver(services.FileSystem, null!, services.SpecialFolders));
        Should.Throw<ArgumentNullException>(() => new PathPlaceholderResolver(services.FileSystem, "/app", null!));
    }
}
