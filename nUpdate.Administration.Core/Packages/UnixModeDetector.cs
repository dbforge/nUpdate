using System.IO.Abstractions;
using nUpdate.Platform;

namespace nUpdate.Administration.Core.Packages;

/// <summary>
///     The Unix permissions a package file gets on Linux and macOS. On those systems the file's own permission bits are
///     taken, without write access for group and others, which files on FAT, NTFS or SMB mounts often claim (0777) and
///     an installed application must not have; Windows has none, so executables are recognized by their first bytes:
///     ELF, Mach-O (also universal binaries) and scripts with <c>#!</c> get 0755, everything else 0644.
/// </summary>
public static class UnixModeDetector
{
    private static readonly byte[][] ExecutableMagics =
    [
        [0x7F, 0x45, 0x4C, 0x46], // ELF
        [0xFE, 0xED, 0xFA, 0xCE], // Mach-O 32-bit
        [0xCE, 0xFA, 0xED, 0xFE],
        [0xFE, 0xED, 0xFA, 0xCF], // Mach-O 64-bit
        [0xCF, 0xFA, 0xED, 0xFE],
        [0xCA, 0xFE, 0xBA, 0xBE], // universal binary
        [0xBE, 0xBA, 0xFE, 0xCA],
    ];

    /// <param name="fileSystem">The file system the file is on.</param>
    /// <param name="path">The file.</param>
    /// <param name="isWindows">Whether the Administration runs on Windows, where files have no Unix permissions.</param>
    public static int Detect(IFileSystem fileSystem, string path, bool isWindows)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(path);
        if (isWindows)
            return LooksExecutable(fileSystem, path) ? FilePermissions.ExecutableMode : FilePermissions.RegularMode;
#pragma warning disable CA1416 // Only called off Windows, as the caller says.
        return (int)fileSystem.File.GetUnixFileMode(path) & FilePermissions.ExecutableMode;
#pragma warning restore CA1416
    }

    /// <summary>Whether the file starts like an executable: ELF, Mach-O or <c>#!</c>.</summary>
    private static bool LooksExecutable(IFileSystem fileSystem, string path)
    {
        var head = new byte[4];
        int read;
        using (var stream = fileSystem.File.OpenRead(path))
            read = stream.Read(head, 0, head.Length);
        if (read >= 2 && head[0] == '#' && head[1] == '!')
            return true;
        return read == 4 && ExecutableMagics.Any(magic => magic.SequenceEqual(head));
    }
}
