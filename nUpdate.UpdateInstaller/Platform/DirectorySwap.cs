using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Text;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller.Platform;

/// <summary>
///     Swaps two directories. On macOS in one atomic step (<c>renamex_np</c> with <c>RENAME_SWAP</c>), so the bundle is
///     never missing; elsewhere, or when the volume cannot swap, with three renames that are rolled back on failure.
/// </summary>
internal sealed class DirectorySwap : IDirectorySwap
{
    /// <summary>The name the current directory has while the renames are under way, and keeps if it cannot be moved on.</summary>
    public const string ParkedSuffix = ".nupdate-old";

    private readonly IFileSystem _fileSystem;
    private readonly Func<string, string, bool> _swapAtomically;

    /// <param name="fileSystem">The file system for the renames.</param>
    /// <param name="atomic">Whether to try the atomic macOS swap first.</param>
    public DirectorySwap(IFileSystem fileSystem, bool atomic)
        : this(fileSystem, atomic ? SwapAtomically : (_, _) => false)
    {
    }

    internal DirectorySwap(IFileSystem fileSystem, Func<string, string, bool> swapAtomically)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _swapAtomically = swapAtomically;
    }

    public void Swap(string current, string replacement)
    {
        if (current is null)
            throw new ArgumentNullException(nameof(current));
        if (replacement is null)
            throw new ArgumentNullException(nameof(replacement));
        if (_swapAtomically(current, replacement))
            return;

        var parked = current + ParkedSuffix;
        if (_fileSystem.Directory.Exists(parked))
            _fileSystem.Directory.Delete(parked, recursive: true);
        _fileSystem.Directory.Move(current, parked);
        try
        {
            _fileSystem.Directory.Move(replacement, current);
        }
        catch (Exception)
        {
            _fileSystem.Directory.Move(parked, current);
            throw;
        }

        try
        {
            _fileSystem.Directory.Move(parked, replacement);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // The new directory is in place; the old one stays parked and the next swap removes it.
        }
    }

    [ExcludeFromCodeCoverage] // macOS only; verified by the macOS published-installer run.
    private static bool SwapAtomically(string current, string replacement) =>
        NativeMethods.renamex_np(Encoding.UTF8.GetBytes(replacement + "\0"), Encoding.UTF8.GetBytes(current + "\0"),
            NativeMethods.RenameSwap) == 0;

    private static class NativeMethods
    {
        public const uint RenameSwap = 0x2;

        [DllImport("libc", SetLastError = true)]
        public static extern int renamex_np(byte[] from, byte[] to, uint flags);
    }
}
