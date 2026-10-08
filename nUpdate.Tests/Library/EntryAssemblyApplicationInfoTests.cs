using System.Reflection;
using System.Runtime.Loader;
using nUpdate.Platform;

[assembly: nUpdate.ApplicationVersion("1.2.3.4")]

namespace nUpdate.Tests.Library;

public class EntryAssemblyApplicationInfoTests
{
    private static readonly Assembly TestAssembly = typeof(EntryAssemblyApplicationInfoTests).Assembly;

    [Fact]
    public void Constructor_ReadsTheEntryAssemblyAndTheProcess()
    {
        var info = new EntryAssemblyApplicationInfo();
        info.ProductName.ShouldBe("nUpdate.Tests");
        info.DeclaredVersion.ShouldBe("1.2.3.4");
        info.UserAgentProduct.ShouldBe($"nUpdate.Tests/{TestAssembly.GetName().Version}");
        info.CurrentProcessId.ShouldBe(Environment.ProcessId);
        info.ExecutablePath.ShouldNotBeNullOrEmpty();
        File.Exists(info.ExecutablePath).ShouldBeTrue();
    }

    [Theory]
    [InlineData("/usr/share/dotnet/dotnet", false)]
    [InlineData(@"C:\Program Files\dotnet\dotnet.exe", true)]
    [InlineData(null, true)]
    public void ExecutablePath_UnderTheDotnetHost_PointsToTheApphostNextToTheAssembly(string? mainModule, bool windows)
    {
        var info = new EntryAssemblyApplicationInfo(TestAssembly, mainModule, windows);
        var expected = windows ? Path.ChangeExtension(TestAssembly.Location, ".exe") : Path.ChangeExtension(TestAssembly.Location, null);
        info.ExecutablePath.ShouldBe(expected);
        info.ProductName.ShouldBe("nUpdate.Tests");
    }

    [Fact]
    public void ExecutablePath_PrefersTheProcessImageOfAnApphost()
    {
        new EntryAssemblyApplicationInfo(TestAssembly, "/opt/app/MyApp", false).ExecutablePath.ShouldBe("/opt/app/MyApp");
        new EntryAssemblyApplicationInfo(TestAssembly, @"C:\app\MyApp.exe", true).ExecutablePath.ShouldBe(@"C:\app\MyApp.exe");
    }

    [Fact]
    public void Constructor_WithoutAnEntryAssembly_FallsBackToTheProcess()
    {
        var info = new EntryAssemblyApplicationInfo(null, "/usr/share/dotnet/dotnet", false);
        info.ProductName.ShouldBe("dotnet");
        info.ExecutablePath.ShouldBe("/usr/share/dotnet/dotnet");
        info.DeclaredVersion.ShouldBeNull();
        info.UserAgentProduct.ShouldBe("nUpdate/5.0");

        var bare = new EntryAssemblyApplicationInfo(null, null, true);
        bare.ProductName.ShouldBe("Application");
        bare.ExecutablePath.ShouldBeNull();

        var relative = new EntryAssemblyApplicationInfo(null, "dotnet", false);
        relative.ProductName.ShouldBe("dotnet");
        relative.ExecutablePath.ShouldBe("dotnet");
    }

    [Fact]
    public void DeclaredVersion_IsNullWithoutTheAttribute()
    {
        new EntryAssemblyApplicationInfo(typeof(Updating.UpdateVersion).Assembly, null, false).DeclaredVersion.ShouldBeNull();
    }

    [Fact]
    public void ExecutablePath_OnNetFramework_IsTheEntryAssemblyItself()
    {
        // On .NET Framework the entry assembly is the .exe; simulate it with a copy of the library under an .exe name.
        var directory = Directory.CreateTempSubdirectory("nupdate-exe-");
        var path = Path.Combine(directory.FullName, "Host.exe");
        File.Copy(typeof(Updating.UpdateVersion).Assembly.Location, path);
        var context = new AssemblyLoadContext("exe-probe", isCollectible: true);
        try
        {
            var assembly = context.LoadFromAssemblyPath(path);
            new EntryAssemblyApplicationInfo(assembly, @"C:\Program Files\dotnet\dotnet.exe", true).ExecutablePath.ShouldBe(path);
        }
        finally
        {
            context.Unload();
            try
            {
                directory.Delete(recursive: true);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                // Windows keeps the file locked until the unloaded context is collected; the temp folder stays behind then.
            }
        }
    }
}
