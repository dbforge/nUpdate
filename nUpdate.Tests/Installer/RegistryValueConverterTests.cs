using nUpdate.Operations;
using nUpdate.UpdateInstaller.Windows;

namespace nUpdate.Tests.Installer;

public class RegistryValueConverterTests
{
    [Fact]
    public void ToRegistryValue_ConvertsEveryKind()
    {
        RegistryValueConverter.ToRegistryValue(RegistryValue.Binary("n", [1, 2, 255])).ShouldBe(new byte[] { 1, 2, 255 });
        RegistryValueConverter.ToRegistryValue(RegistryValue.MultiString("n", ["a", "b"])).ShouldBe(new[] { "a", "b" });
        RegistryValueConverter.ToRegistryValue(RegistryValue.DWord("n", 42)).ShouldBe(42);
        RegistryValueConverter.ToRegistryValue(RegistryValue.QWord("n", 44)).ShouldBe(44L);
        RegistryValueConverter.ToRegistryValue(RegistryValue.String("n", "1.5")).ShouldBe("1.5");
        RegistryValueConverter.ToRegistryValue(new RegistryValue("n", RegistryValueKind.String, null)).ShouldBe("");
        RegistryValueConverter.ToRegistryValue(RegistryValue.ExpandString("n", "%TEMP%")).ShouldBe("%TEMP%");
        Should.Throw<ArgumentNullException>(() => RegistryValueConverter.ToRegistryValue(null!));
        // DWORDs above int.MaxValue keep their 32 bits, which is what RegistryKey.SetValue(..., DWord) expects.
        RegistryValueConverter.ToRegistryValue(RegistryValue.DWord("n", uint.MaxValue)).ShouldBe(-1);
        RegistryValueConverter.ToRegistryValue(RegistryValue.DWord("n", 0x80000000)).ShouldBe(int.MinValue);
        RegistryValueConverter.ToRegistryValue(RegistryValue.DWord("n", -1)).ShouldBe(-1);
    }

    [Theory]
    [InlineData(RegistryValueKind.String, Microsoft.Win32.RegistryValueKind.String)]
    [InlineData(RegistryValueKind.ExpandString, Microsoft.Win32.RegistryValueKind.ExpandString)]
    [InlineData(RegistryValueKind.DWord, Microsoft.Win32.RegistryValueKind.DWord)]
    [InlineData(RegistryValueKind.QWord, Microsoft.Win32.RegistryValueKind.QWord)]
    [InlineData(RegistryValueKind.MultiString, Microsoft.Win32.RegistryValueKind.MultiString)]
    [InlineData(RegistryValueKind.Binary, Microsoft.Win32.RegistryValueKind.Binary)]
    public void ToWin32Kind_MapsKinds(RegistryValueKind kind, Microsoft.Win32.RegistryValueKind expected)
    {
        RegistryValueConverter.ToWin32Kind(kind).ShouldBe(expected);
        Should.Throw<ArgumentOutOfRangeException>(() => RegistryValueConverter.ToWin32Kind((RegistryValueKind)99));
    }
}
