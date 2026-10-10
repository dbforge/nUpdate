namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>Facts about the system and the session the installer runs in.</summary>
internal interface IEnvironmentInfo
{
    /// <summary>True when there is no interactive desktop, for example when started by a Windows service.</summary>
    bool IsServiceContext { get; }

    bool IsWindows { get; }

    bool IsMacOS { get; }

    /// <summary>Whether a window can be shown: an interactive Windows session, a Linux session with an X11 or Wayland display, or macOS.</summary>
    bool HasDisplay { get; }
}
