using System.IO.Abstractions.TestingHelpers;
using nUpdate.Platform;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Library.Support;

/// <summary>Builds <see cref="UpdateManagerServices" /> with substitutes and an in-memory file system.</summary>
public sealed class TestServices
{
    public TestServices()
    {
        FileSystem = new MockFileSystem();
        Http = new StubHttpMessageHandler();
        ApplicationInfo = Substitute.For<IApplicationInfo>();
        ApplicationInfo.ProductName.Returns("TestApp");
        ApplicationInfo.ExecutablePath.Returns(FileSystem.Path.Combine(AppDirectory, "TestApp.exe"));
        ApplicationInfo.DeclaredVersion.Returns("1.0.0");
        ApplicationInfo.UserAgentProduct.Returns("TestApp/1.0.0.0");
        ApplicationInfo.CurrentProcessId.Returns(4242);
        SystemInformation = Substitute.For<ISystemInformation>();
        SystemInformation.RuntimeIdentifier.Returns("win-x64");
        SystemInformation.OperatingSystemName.Returns("Windows 11");
        FilePermissions = Substitute.For<IFilePermissions>();
        FilePermissions.CanWrite(Arg.Any<string>()).Returns(true);
        ProcessLauncher = Substitute.For<IProcessLauncher>();
        ProcessLauncher.Start(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(true);
        ApplicationTerminator = Substitute.For<IApplicationTerminator>();
    }

    public string AppDirectory => FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), "app");

    public MockFileSystem FileSystem { get; }

    public StubHttpMessageHandler Http { get; }

    public IApplicationInfo ApplicationInfo { get; }

    public ISystemInformation SystemInformation { get; }

    public IProcessLauncher ProcessLauncher { get; }

    public IFilePermissions FilePermissions { get; }

    public IApplicationTerminator ApplicationTerminator { get; }

    public ListLogger Logger { get; } = new();

    public UpdateManagerServices Build(bool injectHttpClient = true) => new()
    {
        HttpClient = injectHttpClient ? Http.CreateClient() : null,
        FileSystem = FileSystem,
        SystemInformation = SystemInformation,
        ApplicationInfo = ApplicationInfo,
        ProcessLauncher = ProcessLauncher,
        FilePermissions = FilePermissions,
        ApplicationTerminator = ApplicationTerminator,
        Logger = Logger,
    };

    /// <summary>The file name of the built-in installer on Windows, which the substitutes pretend to be.</summary>
    public const string InstallerFileName = UpdateManager.BuiltInInstallerName + ".exe";

    /// <summary>Puts the built-in installer folder of win-x64 next to the application executable, with a file and a folder besides the installer.</summary>
    public string AddInstaller()
    {
        var directory = FileSystem.Path.Combine(AppDirectory, UpdateManager.InstallerFolderName, "win-x64");
        FileSystem.AddFile(FileSystem.Path.Combine(directory, InstallerFileName), new MockFileData("exe"));
        FileSystem.AddFile(FileSystem.Path.Combine(directory, "extra.dll"), new MockFileData("dll"));
        FileSystem.AddFile(FileSystem.Path.Combine(directory, "de", "resources.dll"), new MockFileData("res"));
        return directory;
    }
}
