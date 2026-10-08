using nUpdate.Administration.Core.Models;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Projects;

/// <summary>Appends entries to a project's history.</summary>
public interface IProjectLogger
{
    void Write(UpdateProject project, LogEntryKind kind, UpdateVersion? version);
}

public sealed class ProjectLogger : IProjectLogger
{
    private readonly Func<DateTimeOffset> _now;
    private readonly Func<string> _userName;

    public ProjectLogger()
        : this(() => DateTimeOffset.UtcNow, DefaultUserName)
    {
    }

    public ProjectLogger(Func<DateTimeOffset> now, Func<string> userName)
    {
        _now = now ?? throw new ArgumentNullException(nameof(now));
        _userName = userName ?? throw new ArgumentNullException(nameof(userName));
    }

    public void Write(UpdateProject project, LogEntryKind kind, UpdateVersion? version)
    {
        ArgumentNullException.ThrowIfNull(project);
        project.Log.Add(new LogEntry { Kind = kind, At = _now(), Version = version, User = _userName() });
    }

    /// <summary>Formats <c>DOMAIN\user</c>, or just the user name when there is no domain or the domain is the machine itself.</summary>
    public static string FormatUserName(string? domain, string machineName, string userName)
    {
        ArgumentNullException.ThrowIfNull(userName);
        return string.IsNullOrEmpty(domain) || string.Equals(domain, machineName, StringComparison.OrdinalIgnoreCase) ? userName : $"{domain}\\{userName}";
    }

    private static string DefaultUserName() => FormatUserName(Environment.UserDomainName, Environment.MachineName, Environment.UserName);
}
