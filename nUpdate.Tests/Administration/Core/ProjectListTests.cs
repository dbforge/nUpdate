using nUpdate.Administration.Core.Models;

namespace nUpdate.Tests.Administration.Core;

public class ProjectListTests
{
    [Fact]
    public void Format_DefaultsTo1()
    {
        new ProjectList().Format.ShouldBe(1);
    }
}
