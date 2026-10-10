using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using nUpdate.Installer;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Platform;
using nUpdate.UpdateInstaller;
using nUpdate.UpdateInstaller.Abstractions;
using nUpdate.UpdateInstaller.Operations;
using nUpdate.Updating;

namespace nUpdate.Tests.Installer.Support;

/// <summary>An in-memory installer environment: mock file system, substituted system services, recording reporter.</summary>
public sealed class TestInstallerServices
{
    public TestInstallerServices()
    {
        FileSystem = new MockFileSystem();
        FileSystem.AddDirectory(AppDirectory);
        FileSystem.AddDirectory(PackagesDirectory);
        SpecialFolders = Substitute.For<ISpecialFolders>();
        SpecialFolders.ApplicationData.Returns(Root("appdata"));
        SpecialFolders.Temp.Returns(Root("temp"));
        SpecialFolders.Desktop.Returns(Root("desktop"));
        EnvironmentInfo = Substitute.For<IEnvironmentInfo>();
        EnvironmentInfo.IsServiceContext.Returns(false);
        EnvironmentInfo.IsWindows.Returns(true);
        EnvironmentInfo.HasDisplay.Returns(true);
        FilePermissions = Substitute.For<IFilePermissions>();
        DirectorySwap = Substitute.For<IDirectorySwap>();
        EventLog = Substitute.For<IEventLog>();
        Registry = Substitute.For<IRegistry>();
        ServiceController = Substitute.For<IServiceController>();
        ProcessService = Substitute.For<IProcessService>();
        ProcessService.WaitForExit(Arg.Any<int>(), Arg.Any<TimeSpan>()).Returns(true);
        ProcessService.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        Reporter = new RecordingReporter();
        Services = new InstallerServices
        {
            FileSystem = FileSystem,
            PackageExtractor = new ZipPackageExtractor(FileSystem, FilePermissions),
            Registry = Registry,
            ServiceController = ServiceController,
            ProcessService = ProcessService,
            SpecialFolders = SpecialFolders,
            EnvironmentInfo = EnvironmentInfo,
            DirectorySwap = DirectorySwap,
            EventLog = EventLog,
            ErrorOutput = ErrorOutput,
            Clock = () => new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero),
            Delay = Delays.Add,
            HostExitTimeout = TimeSpan.FromSeconds(1),
            MaxLockedFileAttempts = 3,
        };
    }

    public MockFileSystem FileSystem { get; }

    public ISpecialFolders SpecialFolders { get; }

    public IEnvironmentInfo EnvironmentInfo { get; }

    public IRegistry Registry { get; }

    public IServiceController ServiceController { get; }

    public IProcessService ProcessService { get; }

    public IFilePermissions FilePermissions { get; }

    public IDirectorySwap DirectorySwap { get; }

    public IEventLog EventLog { get; }

    public StringWriter ErrorOutput { get; } = new();

    public List<TimeSpan> Delays { get; } = [];

    public RecordingReporter Reporter { get; }

    public InstallerServices Services { get; }

    public string AppDirectory => Root("app");

    public string PackagesDirectory => Root("packages");

    public string Root(string name) => FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), "nupdate-test", name);

    public static Guid ProjectId { get; } = new("5f1e4a8c-3b2d-4c6e-9f10-0123456789ab");

    /// <summary>Writes the options next to the packages, where the installer's temp folder would be, and returns the file.</summary>
    public string WriteOptions(InstallerOptions options)
    {
        var path = FileSystem.Path.Combine(PackagesDirectory, "installer-options.json");
        FileSystem.File.WriteAllText(path, Serializer.Serialize(options));
        return path;
    }

    public InstallerOptions Options(params string[] packagePaths) => new()
    {
        Packages = packagePaths.Select(p => new InstallerPackage { Path = p }).ToList(),
        Application = new ApplicationOptions
        {
            Name = "App",
            Directory = AppDirectory,
            ExecutablePath = FileSystem.Path.Combine(AppDirectory, "app.exe")
        },
        Host = new HostOptions { ProcessId = 77, AfterInstall = AfterInstall.Restart },
    };

    public OperationContext Context(InstallerOptions? options = null, ProgressTracker? progress = null) =>
        new(options ?? Options(FileSystem.Path.Combine(PackagesDirectory, "1.0.0.0.zip")), Services,
            new PathPlaceholderResolver(FileSystem, AppDirectory, SpecialFolders), progress ?? new ProgressTracker(),
            Reporter);

    /// <summary>Writes a package zip with the given entries (relative paths inside the zip), a manifest and optional operations.</summary>
    public string AddPackage(string version, IDictionary<string, string> files,
        IEnumerable<Operation>? operations = null, IDictionary<string, int>? modes = null)
    {
        var path = FileSystem.Path.Combine(PackagesDirectory, $"{version}.zip");
        FileSystem.AddFile(path, new MockFileData(BuildZip(files, Manifest(version, operations), modes)));
        return path;
    }

    public static PackageManifest Manifest(string version, IEnumerable<Operation>? operations = null) => new()
    {
        ProjectId = ProjectId,
        Version = new UpdateVersion(version),
        CreatedAt = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero),
        Operations = operations?.ToList() ?? [],
    };

    /// <param name="files">The entries and their text.</param>
    /// <param name="manifest">The manifest, or <c>null</c> for none.</param>
    /// <param name="modes">Unix modes per entry, stored the way nUpdate Administration stores them.</param>
    public static byte[] BuildZip(IDictionary<string, string> files, PackageManifest? manifest = null,
        IDictionary<string, int>? modes = null)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var pair in files)
            {
                var entry = archive.CreateEntry(pair.Key);
                entry.ExternalAttributes =
                    modes is not null && modes.TryGetValue(pair.Key, out var mode) ? mode << 16 : 0;
                using var writer = new StreamWriter(entry.Open());
                writer.Write(pair.Value);
            }

            if (manifest is not null)
            {
                var entry = archive.CreateEntry(PackageLayout.ManifestFileName);
                using var writer = new StreamWriter(entry.Open());
                writer.Write(Serializer.Serialize(manifest));
            }
        }

        return stream.ToArray();
    }
}

/// <summary>Records every call of the reporter contract.</summary>
public sealed class RecordingReporter : IProgressReporter
{
    public List<(float Progress, string Text)> Unpacking { get; } = [];

    public List<(float Progress, string Text)> Operations { get; } = [];

    public List<(string Path, int Attempt)> LockedFiles { get; } = [];

    public List<Exception> Failures { get; } = [];

    public int Terminated { get; private set; }

    public bool Initialized { get; private set; }

    public Func<string, int, LockedFileDecision> LockedFileDecision { get; set; } =
        (_, _) => nUpdate.Installer.LockedFileDecision.Abort;

    public void Initialize() => Initialized = true;

    public void ReportUnpackingProgress(float progress, string currentFile) => Unpacking.Add((progress, currentFile));

    public void ReportOperationProgress(float progress, string currentOperation) =>
        Operations.Add((progress, currentOperation));

    public LockedFileDecision ReportLockedFile(string filePath, int attempt)
    {
        LockedFiles.Add((filePath, attempt));
        return LockedFileDecision(filePath, attempt);
    }

    public void Fail(Exception exception) => Failures.Add(exception);

    public void Terminate() => Terminated++;
}
