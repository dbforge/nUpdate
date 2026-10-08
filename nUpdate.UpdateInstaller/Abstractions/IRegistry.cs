using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>Registry access. Key paths start with a hive name such as <c>HKEY_CURRENT_USER</c>.</summary>
public interface IRegistry
{
    void CreateSubKey(string keyPath, string subKeyName);

    void DeleteSubKey(string keyPath, string subKeyName);

    void SetValue(string keyPath, RegistryValue value);

    void DeleteValue(string keyPath, string valueName);
}
