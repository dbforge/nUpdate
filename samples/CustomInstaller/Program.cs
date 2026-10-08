using nUpdate.UpdateInstaller;

namespace CustomInstaller;

/// <summary>
///     The whole installer: <see cref="InstallerHost" /> reads the options the application wrote, runs the update and
///     shows the window this sample builds, or none when there is no desktop.
/// </summary>
internal static class Program
{
    [STAThread]
    private static int Main(string[] args) => InstallerHost.Run(args, session => new WpfProgressReporter(session));
}
