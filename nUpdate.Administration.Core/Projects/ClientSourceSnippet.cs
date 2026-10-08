using nUpdate.Administration.Core.Models;

namespace nUpdate.Administration.Core.Projects;

/// <summary>
///     The lines a developer pastes into the client application to create its <c>UpdateManager</c>. The current version
///     comes from <c>[assembly: ApplicationVersion("…")]</c>, so the snippet cannot pin a version that goes stale.
/// </summary>
public static class ClientSourceSnippet
{
    public static string CSharp(UpdateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        return $"var manager = new UpdateManager(new Uri(\"{project.FeedUri}\"),{Environment.NewLine}" +
               $"    @\"{project.PublicKey.Replace("\"", "\"\"", StringComparison.Ordinal)}\",{Environment.NewLine}" +
               "    CultureInfo.CurrentUICulture);";
    }

    public static string VisualBasic(UpdateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        return $"Dim manager As New UpdateManager(New Uri(\"{project.FeedUri}\"),{Environment.NewLine}" +
               $"    \"{project.PublicKey.Replace("\"", "\"\"", StringComparison.Ordinal)}\",{Environment.NewLine}" +
               "    CultureInfo.CurrentUICulture)";
    }
}
