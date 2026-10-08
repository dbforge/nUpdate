using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Tests.Administration.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class ProjectLoggerTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public void ProjectLogger_WritesEntries()
    {
        var project = _context.NewProject();
        _context.Logger.Write(project, LogEntryKind.Upload, new UpdateVersion("1.0.0"));
        var entry = project.Log.Single();
        entry.Kind.ShouldBe(LogEntryKind.Upload);
        entry.Version.ShouldBe(new UpdateVersion("1.0.0"));
        entry.User.ShouldBe("tester");
        entry.At.ShouldBe(AdminTestContext.Now);

        new ProjectLogger().Write(project, LogEntryKind.Delete, null);
        project.Log[1].User.ShouldNotBeNullOrEmpty();
        Should.Throw<ArgumentNullException>(() => _context.Logger.Write(null!, LogEntryKind.Create, null));
        ProjectLogger.FormatUserName(null, "PC", "u").ShouldBe("u");
        ProjectLogger.FormatUserName("", "PC", "u").ShouldBe("u");
        ProjectLogger.FormatUserName("pc", "PC", "u").ShouldBe("u");
        ProjectLogger.FormatUserName("CORP", "PC", "u").ShouldBe("CORP\\u");
        Should.Throw<ArgumentNullException>(() => ProjectLogger.FormatUserName("CORP", "PC", null!));
        Should.Throw<ArgumentNullException>(() => new ProjectLogger(null!, () => "u"));
        Should.Throw<ArgumentNullException>(() => new ProjectLogger(() => DateTimeOffset.UtcNow, null!));
    }
}
