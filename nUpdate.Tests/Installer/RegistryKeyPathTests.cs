using nUpdate.UpdateInstaller.Windows;

namespace nUpdate.Tests.Installer;

public class RegistryKeyPathTests
{
    [Theory]
    [InlineData("HKEY_CURRENT_USER\\Software\\Vendor", nameof(RegistryHive.CurrentUser), "Software\\Vendor")]
    [InlineData("hkcu\\Software", nameof(RegistryHive.CurrentUser), "Software")]
    [InlineData("HKEY_LOCAL_MACHINE", nameof(RegistryHive.LocalMachine), "")]
    [InlineData("HKLM\\x", nameof(RegistryHive.LocalMachine), "x")]
    [InlineData("HKEY_CLASSES_ROOT\\.ext", nameof(RegistryHive.ClassesRoot), ".ext")]
    [InlineData("HKCR\\.ext", nameof(RegistryHive.ClassesRoot), ".ext")]
    [InlineData("HKEY_USERS\\S-1", nameof(RegistryHive.Users), "S-1")]
    [InlineData("HKU\\S-1", nameof(RegistryHive.Users), "S-1")]
    [InlineData("HKEY_CURRENT_CONFIG\\a", nameof(RegistryHive.CurrentConfig), "a")]
    [InlineData("HKCC\\a", nameof(RegistryHive.CurrentConfig), "a")]
    public void Split_ReturnsHiveAndSubKey(string path, string hive, string subKey)
    {
        RegistryKeyPath.Split(path).ShouldBe((Enum.Parse<RegistryHive>(hive), subKey));
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
