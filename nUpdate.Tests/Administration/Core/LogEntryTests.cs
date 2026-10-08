using nUpdate.Administration.Core.Models;

namespace nUpdate.Tests.Administration.Core;

public class LogEntryTests
{
    [Fact]
    public void Kind_DefaultsToCreate()
    {
        new LogEntry().Kind.ShouldBe(LogEntryKind.Create);
    }
}
