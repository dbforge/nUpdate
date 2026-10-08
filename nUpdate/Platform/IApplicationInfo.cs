namespace nUpdate.Platform;

/// <summary>Facts about the host application that defaults are derived from.</summary>
public interface IApplicationInfo
{
    /// <summary>The product name, used for temp folder names and the installer window.</summary>
    string ProductName { get; }

    /// <summary>The full path of the host executable, or <c>null</c> when it cannot be determined.</summary>
    string? ExecutablePath { get; }

    /// <summary>The version declared with <see cref="ApplicationVersionAttribute" />, or <c>null</c>.</summary>
    string? DeclaredVersion { get; }

    /// <summary>The assembly name and version used for the HTTP user agent.</summary>
    string UserAgentProduct { get; }

    int CurrentProcessId { get; }
}
