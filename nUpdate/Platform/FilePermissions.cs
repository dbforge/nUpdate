using System.Runtime.InteropServices;
using System.Text;

namespace nUpdate.Platform;

/// <summary>Sets permissions with the C library's <c>chmod</c>, which .NET Standard 2.0 does not wrap.</summary>
public sealed class FilePermissions : IFilePermissions
{
    /// <summary>0755: read and execute for everybody, write for the owner.</summary>
    public const int ExecutableMode = 0x1ED;

    /// <summary>0644: read for everybody, write for the owner.</summary>
    public const int RegularMode = 0x1A4;

    private readonly bool _isWindows;

    public FilePermissions()
        : this(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
    }

    internal FilePermissions(bool isWindows)
    {
        _isWindows = isWindows;
    }

    public bool CanWrite(string directory)
    {
        if (directory is null)
            throw new ArgumentNullException(nameof(directory));

        var probe = Path.Combine(directory, ".nupdate-write-test-" + Guid.NewGuid().ToString("N"));
        try
        {
            using (new FileStream(probe, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose))
            {
            }

            return true;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            return false;
        }
    }

    /// <exception cref="IOException">The mode could not be set.</exception>
    public void SetMode(string path, int mode)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        if (mode is < 0 or > 0xFFF)
            throw new ArgumentOutOfRangeException(nameof(mode));
        if (_isWindows)
            return;
        if (NativeMethods.chmod(Encoding.UTF8.GetBytes(path + "\0"), (uint)mode) != 0)
            throw new IOException($"The permissions of \"{path}\" could not be set (error {Marshal.GetLastWin32Error()}).");
    }

    private static class NativeMethods
    {
        // The runtime maps "libc" to the C library of the running system (libc.so.6, libSystem.dylib).
        // The path goes in as null-terminated UTF-8 bytes: .NET Standard 2.0 cannot marshal a string as UTF-8.
        [DllImport("libc", SetLastError = true)]
        public static extern int chmod(byte[] path, uint mode);
    }
}
