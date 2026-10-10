using System.Reflection;
using System.Reflection.Emit;
using nUpdate.Administration;

namespace nUpdate.Tests.Administration.App;

/// <summary>The version nUpdate Administration shows: the released one, with pre-release label, without build metadata.</summary>
public class AdministrationVersionTests
{
    private static AssemblyBuilder Assembly(Version? version, string? informational = null)
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Versioned") { Version = version },
            AssemblyBuilderAccess.Run);
        if (informational is not null)
            assembly.SetCustomAttribute(new CustomAttributeBuilder(
                typeof(AssemblyInformationalVersionAttribute).GetConstructor([typeof(string)])!, [informational]));
        return assembly;
    }

    [Fact]
    public void Read_TakesTheInformationalVersionWithoutBuildMetadata()
    {
        AdministrationVersion.Read(Assembly(new Version(5, 0, 0, 0), "5.0.0-rc.1+0123abcd")).ShouldBe("5.0.0-rc.1");
        AdministrationVersion.Read(Assembly(new Version(5, 0, 0, 0), "5.0.0")).ShouldBe("5.0.0");
        AdministrationVersion.Read(Assembly(new Version(1, 2, 3, 4))).ShouldBe("1.2.3");
        AdministrationVersion.Read(Assembly(null)).ShouldBe("0.0.0");
    }

    [Fact]
    public void Text_IsTheVersionOfTheAdministration()
    {
        AdministrationVersion.Text.ShouldBe(AdministrationVersion.Read(typeof(AdministrationVersion).Assembly));
        AdministrationVersion.Text.ShouldNotContain('+');
    }
}
