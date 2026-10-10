using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Windows;

/// <summary>Maps a <see cref="RegistryValue" /> to what <c>Microsoft.Win32.RegistryKey.SetValue</c> expects.</summary>
internal static class RegistryValueConverter
{
    /// <summary>
    ///     The CLR value: a string, an <see cref="int" /> for DWORD (values above <see cref="int.MaxValue" /> wrap to the
    ///     negative <see cref="int" /> with the same 32 bits, which is how the registry API stores them), a <see cref="long" />
    ///     for QWORD, a string array or a byte array.
    /// </summary>
    public static object ToRegistryValue(RegistryValue value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return value.Kind switch
        {
            RegistryValueKind.DWord => unchecked((int)(uint)(long)value.Value!),
            RegistryValueKind.QWord => (long)value.Value!,
            RegistryValueKind.MultiString => (string[])value.Value!,
            RegistryValueKind.Binary => (byte[])value.Value!,
            _ => (string?)value.Value ?? string.Empty,
        };
    }

    /// <summary>The Win32 kind of a value.</summary>
    public static Microsoft.Win32.RegistryValueKind ToWin32Kind(RegistryValueKind kind) => kind switch
    {
        RegistryValueKind.String => Microsoft.Win32.RegistryValueKind.String,
        RegistryValueKind.ExpandString => Microsoft.Win32.RegistryValueKind.ExpandString,
        RegistryValueKind.DWord => Microsoft.Win32.RegistryValueKind.DWord,
        RegistryValueKind.QWord => Microsoft.Win32.RegistryValueKind.QWord,
        RegistryValueKind.MultiString => Microsoft.Win32.RegistryValueKind.MultiString,
        RegistryValueKind.Binary => Microsoft.Win32.RegistryValueKind.Binary,
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };
}
