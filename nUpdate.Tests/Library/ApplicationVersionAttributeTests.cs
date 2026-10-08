namespace nUpdate.Tests.Library;

public class ApplicationVersionAttributeTests
{
    [Fact]
    public void ApplicationVersionAttribute_StoresTheVersion()
    {
        new ApplicationVersionAttribute("1.2").Version.ShouldBe("1.2");
        Should.Throw<ArgumentNullException>(() => new ApplicationVersionAttribute(null!));
    }
}
