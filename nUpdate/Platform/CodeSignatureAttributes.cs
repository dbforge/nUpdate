using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace nUpdate.Platform;

/// <summary>Reads and writes the code signature attributes with the C library of macOS; elsewhere there are none.</summary>
public sealed class CodeSignatureAttributes : ICodeSignatureAttributes
{
    /// <summary>The prefix of the attributes <c>codesign</c> writes; others, such as the quarantine flag, are never copied.</summary>
    public const string Prefix = "com.apple.cs.";

    private readonly Func<string, IReadOnlyDictionary<string, byte[]>> _read;
    private readonly Action<string, string, byte[]> _write;

    public CodeSignatureAttributes()
        : this(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
    }

    /// <param name="isMacOS">Whether files have the attributes; elsewhere there are none to read or write.</param>
    internal CodeSignatureAttributes(bool isMacOS)
        : this(isMacOS ? ReadOnMacOS : _ => new Dictionary<string, byte[]>(), isMacOS ? WriteOnMacOS : (_, _, _) => { })
    {
    }

    internal CodeSignatureAttributes(Func<string, IReadOnlyDictionary<string, byte[]>> read,
        Action<string, string, byte[]> write)
    {
        _read = read;
        _write = write;
    }

    public IReadOnlyDictionary<string, byte[]> Read(string path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        return _read(path);
    }

    /// <exception cref="IOException">The attribute could not be set.</exception>
    public void Write(string path, string name, byte[] value)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        if (name is null)
            throw new ArgumentNullException(nameof(name));
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (!name.StartsWith(Prefix, StringComparison.Ordinal))
            throw new ArgumentException($"\"{name}\" is not a code signature attribute.", nameof(name));
        _write(path, name, value);
    }

    [ExcludeFromCodeCoverage] // macOS only; verified by the macOS published-installer run.
    private static Dictionary<string, byte[]> ReadOnMacOS(string path)
    {
        var attributes = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        var file = Utf8(path);
        var length = NativeMethods.listxattr(file, null, UIntPtr.Zero, 0).ToInt64();
        if (length <= 0)
            return attributes;
        var names = new byte[length];
        length = NativeMethods.listxattr(file, names, (UIntPtr)names.Length, 0).ToInt64();
        foreach (var name in Encoding.UTF8.GetString(names, 0, (int)Math.Max(0, length)).Split('\0'))
        {
            if (!name.StartsWith(Prefix, StringComparison.Ordinal))
                continue;
            var size = NativeMethods.getxattr(file, Utf8(name), null, UIntPtr.Zero, 0, 0).ToInt64();
            if (size < 0)
                continue;
            var value = new byte[size];
            if (NativeMethods.getxattr(file, Utf8(name), value, (UIntPtr)value.Length, 0, 0).ToInt64() == size)
                attributes[name] = value;
        }

        return attributes;
    }

    [ExcludeFromCodeCoverage] // macOS only; verified by the macOS published-installer run.
    private static void WriteOnMacOS(string path, string name, byte[] value)
    {
        if (NativeMethods.setxattr(Utf8(path), Utf8(name), value, (UIntPtr)value.Length, 0, 0) != 0)
            throw new IOException(
                $"The code signature attribute {name} of \"{path}\" could not be set (error {Marshal.GetLastWin32Error()}).");
    }

    [ExcludeFromCodeCoverage] // macOS only, like its callers.
    private static byte[] Utf8(string text) => Encoding.UTF8.GetBytes(text + "\0");

    private static class NativeMethods
    {
        // The macOS signatures, which take a position and options that Linux's do not; only called on macOS.
        [DllImport("libc", SetLastError = true)]
        public static extern IntPtr listxattr(byte[] path, byte[]? names, UIntPtr size, int options);

        [DllImport("libc", SetLastError = true)]
        public static extern IntPtr getxattr(byte[] path, byte[] name, byte[]? value, UIntPtr size, uint position,
            int options);

        [DllImport("libc", SetLastError = true)]
        public static extern int setxattr(byte[] path, byte[] name, byte[] value, UIntPtr size, uint position,
            int options);
    }
}
