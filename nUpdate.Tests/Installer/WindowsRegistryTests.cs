using Microsoft.Win32;
using nUpdate.Operations;
using nUpdate.Tests.Support;
using nUpdate.UpdateInstaller.Windows;

namespace nUpdate.Tests.Installer;

/// <summary>
///     Exercises an adapter that touches the real machine. It is excluded from coverage and runs on the Windows CI job;
///     the engine and the operation handlers are tested against substitutes everywhere.
/// </summary>
public class WindowsRegistryTests
{
    private const string SoftwareKey = @"HKEY_CURRENT_USER\Software";

    [WindowsFact]
    public void WindowsRegistry_CreatesSetsAndDeletesKeysAndValues()
    {
        var registry = new WindowsRegistry();
        var name = "nUpdateTests-" + Guid.NewGuid().ToString("N");
        var keyPath = $@"{SoftwareKey}\{name}";
        try
        {
            registry.CreateSubKey(SoftwareKey, name);
            registry.SetValue(keyPath, RegistryValue.String("Text", "hello"));
            registry.SetValue(keyPath, RegistryValue.DWord("Number", 5));
            registry.SetValue(keyPath, RegistryValue.QWord("Big", 7));
            registry.SetValue(keyPath, RegistryValue.MultiString("List", ["a", "b"]));
            registry.SetValue(keyPath, RegistryValue.Binary("Bytes", [1, 2, 3]));
            registry.SetValue(keyPath, RegistryValue.ExpandString("Expand", "%TEMP%"));

            using (var key = Registry.CurrentUser.OpenSubKey($@"Software\{name}")!)
            {
                key.GetValue("Text").ShouldBe("hello");
                key.GetValue("Number").ShouldBe(5);
                key.GetValue("Big").ShouldBe(7L);
                ((string[])key.GetValue("List")!).ShouldBe(["a", "b"]);
                ((byte[])key.GetValue("Bytes")!).ShouldBe([1, 2, 3]);
                key.GetValueKind("Expand").ShouldBe(Microsoft.Win32.RegistryValueKind.ExpandString);
                key.GetValue("Expand", null, RegistryValueOptions.DoNotExpandEnvironmentNames).ShouldBe("%TEMP%");
            }

            registry.DeleteValue(keyPath, "Text");
            registry.DeleteValue(keyPath, "Missing");
            using (var key = Registry.CurrentUser.OpenSubKey($@"Software\{name}")!)
                key.GetValue("Text").ShouldBeNull();

            registry.CreateSubKey(keyPath, "Child");
            registry.DeleteSubKey(SoftwareKey, name);
            registry.DeleteSubKey(SoftwareKey, name);
            Registry.CurrentUser.OpenSubKey($@"Software\{name}").ShouldBeNull();

            Should.Throw<InvalidOperationException>(() =>
                registry.SetValue($@"{keyPath}\Nope", RegistryValue.String("x", "y")));
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree($@"Software\{name}", throwOnMissingSubKey: false);
        }
    }
}
