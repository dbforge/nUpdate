using System.IO.Abstractions.TestingHelpers;
using nUpdate.UpdateInstaller.Reporting;

namespace nUpdate.Tests.Installer.Support;

/// <summary>Install logs on a mock file system that stamp every line with the same time.</summary>
public sealed class TestInstallLogs
{
    public static readonly DateTimeOffset Noon = new(2026, 10, 7, 12, 0, 0, TimeSpan.FromHours(2));

    public MockFileSystem FileSystem { get; } = new();

    public InstallLog Log(string? path = "/tmp/install.log") => new(FileSystem, path, () => Noon);

    /// <summary>The lines of a log the installer still holds open.</summary>
    public string[] ReadLog(string path) => FileSystem.GetFile(path).TextContents
        .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
}
