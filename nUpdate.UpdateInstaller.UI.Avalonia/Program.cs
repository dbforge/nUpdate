using System.Diagnostics.CodeAnalysis;

namespace nUpdate.UpdateInstaller.UI.Avalonia;

[ExcludeFromCodeCoverage] // Process entry point.
internal static class Program
{
    [STAThread]
    public static int Main(string[] args) => InstallerHost.Run(args, session => new InstallerWindowReporter(session));
}
