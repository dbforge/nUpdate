using System.Globalization;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Publishing;

/// <summary>What the user entered for a new package.</summary>
public sealed class PublishRequest
{
    public PublishRequest(UpdateProject project, ProjectSecrets secrets, PackageDefinition package)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
        Secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));
        Package = package ?? throw new ArgumentNullException(nameof(package));
    }

    public UpdateProject Project { get; }

    public ProjectSecrets Secrets { get; }

    public PackageDefinition Package { get; }

    public string Description { get; set; } = string.Empty;

    /// <summary>Changelog per culture. English is required.</summary>
    public Dictionary<CultureInfo, string> Changelog { get; } = [];

    public bool Necessary { get; set; }

    /// <summary>Client versions that must not install this package.</summary>
    public List<UpdateVersion> UnsupportedVersions { get; } = [];

    public RolloutConditionMode RolloutConditionMode { get; set; } = RolloutConditionMode.Any;

    public List<RolloutCondition> RolloutConditions { get; } = [];

    /// <summary>Upload to the server now, or only create the package locally.</summary>
    public bool Publish { get; set; } = true;

    /// <summary>Whether downloads of this package are counted (requires statistics in the project).</summary>
    public bool IncludeInStatistics { get; set; } = true;
}

/// <summary>The server still serves only the legacy <c>updates.json</c>; the project has to be migrated before anything is published.</summary>
public class MigrationRequiredException : InvalidOperationException
{
    public MigrationRequiredException()
        : base("The server still has the updates.json of nUpdate 3 or 4 and no nupdate.json. Migrate the published packages first.")
    {
    }

    public MigrationRequiredException(string message)
        : base(message)
    {
    }

    public MigrationRequiredException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
