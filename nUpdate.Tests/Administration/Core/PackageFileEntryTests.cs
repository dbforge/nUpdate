using nUpdate.Administration.Core.Packages;
using nUpdate.Packaging;

namespace nUpdate.Tests.Administration.Core;

public class PackageFileEntryTests
{
    [Fact]
    public void Constructor_NormalizesAndValidates()
    {
        var entry = new PackageFileEntry(PackageRoot.Program, "\\bin\\app.dll", "/src/app.dll");
        entry.RelativePath.ShouldBe("bin/app.dll");
        entry.EntryName.ShouldBe("Program/bin/app.dll");
        entry.SourcePath.ShouldBe("/src/app.dll");
        Should.Throw<ArgumentException>(() => new PackageFileEntry(PackageRoot.Program, "/", "/s"));
        Should.Throw<ArgumentException>(() => new PackageFileEntry(PackageRoot.Program, "../x", "/s"));
        Should.Throw<ArgumentException>(() => new PackageFileEntry(PackageRoot.Program, "a/./b", "/s"));
        Should.Throw<ArgumentNullException>(() => new PackageFileEntry(PackageRoot.Program, null!, "/s"));
        Should.Throw<ArgumentNullException>(() => new PackageFileEntry(PackageRoot.Program, "x", null!));
        Should.Throw<ArgumentNullException>(() => new PackageDefinition(null!));
    }

    [Fact]
    public void TryParseEntryName_AcceptsOnlyNamesBelowAKnownRoot()
    {
        PackageFileEntry.TryParseEntryName("Program/bin/app.dll", out var root, out var relativePath).ShouldBeTrue();
        root.ShouldBe(PackageRoot.Program);
        relativePath.ShouldBe("bin/app.dll");
        PackageFileEntry.TryParseEntryName("AppData/settings.json", out root, out _).ShouldBeTrue();
        root.ShouldBe(PackageRoot.AppData);

        foreach (var rejected in new[] { null, "", "Program/", "Program", "program/app.dll", "Unknown/x.txt", "manifest.json", "Program//x", "Program/./x", "Program/../x", "Program/sub\\x", "Program/c:x" })
            PackageFileEntry.TryParseEntryName(rejected!, out _, out _).ShouldBeFalse(rejected ?? "<null>");
    }
}
