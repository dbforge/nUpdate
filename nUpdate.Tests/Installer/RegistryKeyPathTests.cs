using nUpdate.UpdateInstaller.Windows;

namespace nUpdate.Tests.Installer;

public class RegistryKeyPathTests
{
    [Theory]
    [InlineData("HKEY_CURRENT_USER\\Software\\Vendor", RegistryHive.CurrentUser, "Software\\Vendor")]
    [InlineData("hkcu\\Software", RegistryHive.CurrentUser, "Software")]
    [InlineData("HKEY_LOCAL_MACHINE", RegistryHive.LocalMachine, "")]
    [InlineData("HKLM\\x", RegistryHive.LocalMachine, "x")]
    [InlineData("HKEY_CLASSES_ROOT\\.ext", RegistryHive.ClassesRoot, ".ext")]
    [InlineData("HKCR\\.ext", RegistryHive.ClassesRoot, ".ext")]
    [InlineData("HKEY_USERS\\S-1", RegistryHive.Users, "S-1")]
    [InlineData("HKU\\S-1", RegistryHive.Users, "S-1")]
    [InlineData("HKEY_CURRENT_CONFIG\\a", RegistryHive.CurrentConfig, "a")]
    [InlineData("HKCC\\a", RegistryHive.CurrentConfig, "a")]
    public void Split_ReturnsHiveAndSubKey(string path, RegistryHive hive, string subKey)
    {
        RegistryKeyPath.Split(path).ShouldBe((hive, subKey));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("NOPE\\x")]
    public void Split_RejectsInvalid(string path)
    {
        Should.Throw<ArgumentException>(() => RegistryKeyPath.Split(path));
    }
}
