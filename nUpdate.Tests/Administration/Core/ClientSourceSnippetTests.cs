using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;

namespace nUpdate.Tests.Administration.Core;

public class ClientSourceSnippetTests
{
    [Fact]
    public void ClientSourceSnippet_EmbedsFeedUrlAndPublicKey()
    {
        var project = new UpdateProject
        {
            UpdateUrl = "https://updates.example.com/app/",
            PublicKey = "-----BEGIN PUBLIC KEY-----\nAB\"CD\n-----END PUBLIC KEY-----"
        };
        var csharp = ClientSourceSnippet.CSharp(project);
        csharp.ShouldStartWith(
            "var manager = new UpdateManager(new Uri(\"https://updates.example.com/app/nupdate.json\"),");
        csharp.ShouldContain("@\"-----BEGIN PUBLIC KEY-----\nAB\"\"CD\n-----END PUBLIC KEY-----\"");
        csharp.ShouldEndWith("CultureInfo.CurrentUICulture);");
        csharp.ShouldNotContain("UpdateVersion"); // the version comes from the ApplicationVersion attribute

        var vb = ClientSourceSnippet.VisualBasic(project);
        vb.ShouldStartWith(
            "Dim manager As New UpdateManager(New Uri(\"https://updates.example.com/app/nupdate.json\"),");
        vb.ShouldContain("AB\"\"CD");
        vb.ShouldEndWith("CultureInfo.CurrentUICulture)");

        Should.Throw<ArgumentNullException>(() => ClientSourceSnippet.CSharp(null!));
        Should.Throw<ArgumentNullException>(() => ClientSourceSnippet.VisualBasic(null!));
    }
}
