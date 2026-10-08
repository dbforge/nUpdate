namespace nUpdate.UpdateInstaller.Windows;

/// <summary>Parses registry paths such as <c>HKEY_CURRENT_USER\Software\Vendor</c>.</summary>
public static class RegistryKeyPath
{
    private static readonly Dictionary<string, RegistryHive> Hives = new(StringComparer.OrdinalIgnoreCase)
    {
        ["HKEY_CLASSES_ROOT"] = RegistryHive.ClassesRoot,
        ["HKCR"] = RegistryHive.ClassesRoot,
        ["HKEY_CURRENT_USER"] = RegistryHive.CurrentUser,
        ["HKCU"] = RegistryHive.CurrentUser,
        ["HKEY_LOCAL_MACHINE"] = RegistryHive.LocalMachine,
        ["HKLM"] = RegistryHive.LocalMachine,
        ["HKEY_USERS"] = RegistryHive.Users,
        ["HKU"] = RegistryHive.Users,
        ["HKEY_CURRENT_CONFIG"] = RegistryHive.CurrentConfig,
        ["HKCC"] = RegistryHive.CurrentConfig,
    };

    public static (RegistryHive Hive, string SubKeyPath) Split(string keyPath)
    {
        if (string.IsNullOrWhiteSpace(keyPath))
            throw new ArgumentException("The registry key path is empty.", nameof(keyPath));

        var parts = keyPath.Split(['\\'], 2);
        if (!Hives.TryGetValue(parts[0], out var hive))
            throw new ArgumentException($"\"{parts[0]}\" is not a registry hive.", nameof(keyPath));
        return (hive, parts.Length > 1 ? parts[1] : string.Empty);
    }
}

/// <summary>The registry hives the installer supports.</summary>
public enum RegistryHive
{
    ClassesRoot,
    CurrentUser,
    LocalMachine,
    Users,
    CurrentConfig,
}
