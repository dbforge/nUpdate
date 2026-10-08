using nUpdate.Updating;

namespace nUpdate.Administration.Core.Models;

/// <summary>A package the project has created, released or not.</summary>
public sealed class UpdatePackage
{
    public UpdateVersion Version { get; set; } = new();

    public string Description { get; set; } = string.Empty;

    /// <summary>True once the package and its feed entry are on the server.</summary>
    public bool Released { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
