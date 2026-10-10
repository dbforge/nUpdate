using nUpdate.Operations;

namespace nUpdate.Tests.Library;

public class RegistryValueTests
{
    [Fact]
    public void RegistryValue_RejectsValuesOfTheWrongType()
    {
        Should.Throw<ArgumentNullException>(() => new RegistryValue(null!, RegistryValueKind.String, "x"));
        Should.Throw<ArgumentException>(() => new RegistryValue("n", RegistryValueKind.DWord, "text"));
        Should.Throw<ArgumentException>(() => RegistryValue.DWord("n", (long)uint.MaxValue + 1)).Message
            .ShouldContain("4294967296");
        Should.Throw<ArgumentException>(() => RegistryValue.DWord("n", (long)int.MinValue - 1));
        RegistryValue.DWord("n", uint.MaxValue).Value.ShouldBe((long)uint.MaxValue);
        RegistryValue.DWord("n", int.MinValue).Value.ShouldBe((long)int.MinValue);
        RegistryValue.QWord("n", long.MaxValue).Value.ShouldBe(long.MaxValue);
        Should.Throw<ArgumentException>(() => new RegistryValue("n", RegistryValueKind.String, 1L));
        Should.Throw<ArgumentException>(() => new RegistryValue("n", RegistryValueKind.MultiString, "x"));
        Should.Throw<ArgumentException>(() => new RegistryValue("n", RegistryValueKind.Binary, null));
        Should.Throw<ArgumentOutOfRangeException>(() => new RegistryValue("n", (RegistryValueKind)42, null));
    }
}
