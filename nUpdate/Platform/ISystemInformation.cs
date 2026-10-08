namespace nUpdate.Platform;

/// <summary>Facts about the machine the update runs on.</summary>
public interface ISystemInformation
{
    /// <summary>A display name such as "Windows 11", used for download statistics.</summary>
    string OperatingSystemName { get; }

    /// <summary>The runtime identifier of the running process, for example <c>win-x64</c>; see <see cref="Updating.PackagePlatform.Current" />.</summary>
    string RuntimeIdentifier { get; }
}
