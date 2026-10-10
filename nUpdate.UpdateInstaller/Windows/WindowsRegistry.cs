using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Microsoft.Win32;
using nUpdate.Operations;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller.Windows;

/// <summary>Registry access through <see cref="Microsoft.Win32.Registry" />.</summary>
[SupportedOSPlatform("windows")]
[ExcludeFromCodeCoverage] // Touches the real Windows registry; verified by the Windows-only test run.
internal sealed class WindowsRegistry : IRegistry
{
    public void CreateSubKey(string keyPath, string subKeyName)
    {
        using var key = Open(keyPath);
        key.CreateSubKey(subKeyName)?.Dispose();
    }

    public void DeleteSubKey(string keyPath, string subKeyName)
    {
        using var key = Open(keyPath);
        key.DeleteSubKeyTree(subKeyName, throwOnMissingSubKey: false);
    }

    public void SetValue(string keyPath, RegistryValue value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        using var key = Open(keyPath);
        key.SetValue(value.Name, RegistryValueConverter.ToRegistryValue(value),
            RegistryValueConverter.ToWin32Kind(value.Kind));
    }

    public void DeleteValue(string keyPath, string valueName)
    {
        using var key = Open(keyPath);
        key.DeleteValue(valueName, throwOnMissingValue: false);
    }

    private static RegistryKey Open(string keyPath)
    {
        var (hive, subKeyPath) = RegistryKeyPath.Split(keyPath);
        var root = hive switch
        {
            RegistryHive.ClassesRoot => Registry.ClassesRoot,
            RegistryHive.CurrentUser => Registry.CurrentUser,
            RegistryHive.LocalMachine => Registry.LocalMachine,
            RegistryHive.Users => Registry.Users,
            _ => Registry.CurrentConfig,
        };
        return root.OpenSubKey(subKeyPath, writable: true)
               ?? throw new InvalidOperationException($"The registry key \"{keyPath}\" does not exist.");
    }
}
