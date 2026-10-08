using nUpdate.Administration.Core.Packages;

namespace nUpdate.Tests.Administration.Core;

public class AssemblyVersionReaderTests
{
    [Fact]
    public void TryRead_ReadsManagedAssembliesOnly()
    {
        AssemblyVersionReader.TryRead(typeof(AssemblyVersionReaderTests).Assembly.Location).ShouldBe(typeof(AssemblyVersionReaderTests).Assembly.GetName().Version);
        AssemblyVersionReader.TryRead(null).ShouldBeNull();
        AssemblyVersionReader.TryRead(" ").ShouldBeNull();
        AssemblyVersionReader.TryRead(Path.Combine(Path.GetTempPath(), "nupdate-missing-" + Guid.NewGuid().ToString("N") + ".dll")).ShouldBeNull();

        var text = Path.Combine(Path.GetTempPath(), "nupdate-notassembly-" + Guid.NewGuid().ToString("N") + ".dll");
        File.WriteAllText(text, "not an assembly");
        try
        {
            AssemblyVersionReader.TryRead(text).ShouldBeNull();
        }
        finally
        {
            File.Delete(text);
        }
    }
}
