using System.Runtime.CompilerServices;
using Xunit;

namespace nUpdate.Tests.Support;

/// <summary>A fact that only runs on Linux and macOS; on Windows it is reported as skipped.</summary>
public sealed class UnixFactAttribute : FactAttribute
{
    public UnixFactAttribute([CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
        if (OperatingSystem.IsWindows())
            Skip = "This test exercises Unix file permissions.";
    }
}
