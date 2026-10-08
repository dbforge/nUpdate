using nUpdate.Administration.Core.Transfer;

namespace nUpdate.Tests.Administration.Core;

public class RemotePathTests
{
    [Theory]
    [InlineData("/base", "", "/base")]
    [InlineData("/base/", "packages/1.0.0.zip", "/base/packages/1.0.0.zip")]
    [InlineData("base", "nupdate.json", "/base/nupdate.json")]
    [InlineData("/", "x", "/x")]
    [InlineData("", "", "/")]
    [InlineData("/a//b", "./c\\d", "/a/b/c/d")]
    public void Combine_JoinsAndNormalizes(string baseDirectory, string relative, string expected)
    {
        RemotePath.Combine(baseDirectory, relative).ShouldBe(expected);
    }

    [Fact]
    public void RemotePath_SplitsPathsAndRejectsEscapingCombinations()
    {
        Should.Throw<ArgumentException>(() => RemotePath.Combine("/base", "../x"));
        Should.Throw<ArgumentNullException>(() => RemotePath.Combine(null!, "x"));
        Should.Throw<ArgumentNullException>(() => RemotePath.Combine("/", null!));
        RemotePath.Parent("/a/b/c").ShouldBe("/a/b");
        RemotePath.Parent("/a/").ShouldBe("/");
        RemotePath.Parent("a").ShouldBe("/");
        RemotePath.FileName("/a/b/c.txt").ShouldBe("c.txt");
        RemotePath.FileName("/a/b/").ShouldBe("b");
        Should.Throw<ArgumentNullException>(() => RemotePath.Parent(null!));
        Should.Throw<ArgumentNullException>(() => RemotePath.FileName(null!));
    }
}
