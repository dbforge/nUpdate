namespace nUpdate.Administration.Core.Models;

/// <summary>An entry of <c>projects.json</c>: a known project and where its file lives.</summary>
public sealed class ProjectRegistration
{
    public ProjectRegistration()
    {
    }

    public ProjectRegistration(Guid id, string name, string path)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>The path of the project file.</summary>
    public string Path { get; set; } = string.Empty;
}

/// <summary>The <c>projects.json</c> document.</summary>
public sealed class ProjectList
{
    public const int CurrentFormat = 1;

    public int Format { get; set; } = CurrentFormat;

    public List<ProjectRegistration> Projects { get; set; } = [];
}
