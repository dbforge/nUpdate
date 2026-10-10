using System.Diagnostics.CodeAnalysis;

namespace nUpdate.Platform;

/// <summary>Terminates the process with exit code 0.</summary>
[ExcludeFromCodeCoverage] // Calling Environment.Exit would end the test host.
internal sealed class EnvironmentExitTerminator : IApplicationTerminator
{
    public void Terminate() => Environment.Exit(0);
}
