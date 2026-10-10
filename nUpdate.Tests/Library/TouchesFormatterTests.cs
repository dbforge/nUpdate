using nUpdate.Localization;
using nUpdate.Operations;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class TouchesFormatterTests
{
    [Fact]
    public void Describe_ListsDistinctAreasInOrder()
    {
        var texts = new UpdateTexts();
        var packages = new[]
        {
            new PackageInfo
            {
                Files =
                [
                    new PackageFile { Platform = "win", Touches = [OperationArea.Services, OperationArea.Files] },
                    new PackageFile { Platform = "linux", Touches = [OperationArea.Processes] }
                ]
            },
            new PackageInfo
            {
                Files = [new PackageFile { Platform = "any", Touches = [OperationArea.Files, OperationArea.Registry] }]
            },
            new PackageInfo { Files = [new PackageFile { Platform = "osx" }] },
            new PackageInfo(),
        };
        TouchesFormatter.Describe(packages, "win-x64", texts).ShouldBe(["File system", "Registry", "Services"]);
        TouchesFormatter.Describe(packages, "linux-x64", texts).ShouldBe(["File system", "Registry", "Processes"]);
        TouchesFormatter.Describe(OperationArea.Registry, texts).ShouldBe("Registry");
        TouchesFormatter.Describe(OperationArea.Processes, texts).ShouldBe("Processes");
        Should.Throw<ArgumentOutOfRangeException>(() => TouchesFormatter.Describe((OperationArea)99, texts));
        Should.Throw<ArgumentNullException>(() => TouchesFormatter.Describe(packages, "win", null!));
        Should.Throw<ArgumentNullException>(() => TouchesFormatter.Describe(null!, "win", texts));
        Should.Throw<ArgumentNullException>(() => TouchesFormatter.Describe(OperationArea.Files, null!));
    }
}
