using nUpdate.Updating;

namespace nUpdate.Administration.Core.Models;

/// <summary>An action recorded in the project's history.</summary>
public sealed class LogEntry
{
    public LogEntryKind Kind { get; set; }

    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>The package the action concerned, or <c>null</c> for project-level actions.</summary>
    public UpdateVersion? Version { get; set; }

    public string User { get; set; } = string.Empty;
}

public enum LogEntryKind
{
    Create,
    Delete,
    Upload,
    Edit,
    Migrate,

    /// <summary>The package files of a version were built again with changed files or operations.</summary>
    Rebuild,
}
