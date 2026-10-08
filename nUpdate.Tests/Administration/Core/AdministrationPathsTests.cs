using nUpdate.Administration.Core;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.Core;

public class AdministrationPathsTests
{
    [Fact]
    public void AdministrationPaths_LayOutTheDataFolder()
    {
        var context = new AdminTestContext();
        var paths = context.Paths;
        var fs = context.FileSystem;
        paths.ProjectsConfigFile.ShouldBe(fs.Path.Combine(paths.Root, "projects.json"));
        paths.LegacyProjectsConfigFile.ShouldBe(fs.Path.Combine(paths.Root, "projconf.json"));
        paths.LegacyProjectDataDirectory("Demo").ShouldBe(fs.Path.Combine(paths.Root, "Projects", "Demo"));
        paths.PasswordsFile.ShouldBe(fs.Path.Combine(paths.Root, "passwords.json"));
        paths.KeyRingDirectory.ShouldBe(fs.Path.Combine(paths.Root, "keys"));
        paths.SuggestedProjectFolder("X").ShouldBe(fs.Path.Combine(paths.DefaultProjectsDirectory, "X"));
        AdministrationPaths.Default(fs).Root.ShouldEndWith(AdministrationPaths.ApplicationFolderName);
        AdministrationPaths.Default(fs).DefaultProjectsDirectory.ShouldEndWith(AdministrationPaths.DefaultProjectsFolderName);
        Should.Throw<ArgumentNullException>(() => AdministrationPaths.Default(null!));
        Should.Throw<ArgumentNullException>(() => new AdministrationPaths(null!, "/r", "/p"));
        Should.Throw<ArgumentNullException>(() => new AdministrationPaths(fs, null!, "/p"));
        Should.Throw<ArgumentNullException>(() => new AdministrationPaths(fs, "/r", null!));
    }
}
