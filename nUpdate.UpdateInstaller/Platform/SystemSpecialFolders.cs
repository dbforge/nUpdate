using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller.Platform;

/// <summary>Special folders from <see cref="Environment" />.</summary>
internal sealed class SystemSpecialFolders : ISpecialFolders
{
    public string ApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public string Temp => Path.GetTempPath();

    public string Desktop => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
}
