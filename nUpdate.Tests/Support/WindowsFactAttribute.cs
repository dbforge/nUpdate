using System.Runtime.CompilerServices;

namespace nUpdate.Tests.Support;

/// <summary>A fact that only runs on Windows; elsewhere it is reported as skipped.</summary>
public sealed class WindowsFactAttribute : FactAttribute
{
    public WindowsFactAttribute([CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
        if (!OperatingSystem.IsWindows())
            Skip = "This test exercises a Windows-only adapter.";
    }
}
