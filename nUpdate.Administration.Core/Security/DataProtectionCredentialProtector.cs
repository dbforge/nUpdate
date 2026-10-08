using Microsoft.AspNetCore.DataProtection;

namespace nUpdate.Administration.Core.Security;

/// <summary>Protects secrets with ASP.NET Core Data Protection (DPAPI-wrapped keys on Windows).</summary>
public sealed class DataProtectionCredentialProtector : ICredentialProtector
{
    private const string Purpose = "nUpdate.Administration.ProjectSecrets";
    private readonly IDataProtector _protector;

    public DataProtectionCredentialProtector(IDataProtectionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        _protector = provider.CreateProtector(Purpose);
    }

    /// <summary>Creates a protector whose key ring lives in the given directory, DPAPI-wrapped on Windows.</summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Platform-dependent branch; the Windows CI job exercises the DPAPI path.")]
    public static DataProtectionCredentialProtector CreateForDirectory(string keyRingDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyRingDirectory);
        var directory = new DirectoryInfo(keyRingDirectory);
        return new DataProtectionCredentialProtector(OperatingSystem.IsWindows() ? CreateWindowsProvider(directory) : DataProtectionProvider.Create(directory));
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "DPAPI is only available on Windows; exercised by the Windows CI job.")]
    private static IDataProtectionProvider CreateWindowsProvider(DirectoryInfo directory) =>
        DataProtectionProvider.Create(directory, builder => builder.ProtectKeysWithDpapi());

    public string Protect(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        return _protector.Protect(plainText);
    }

    public string Unprotect(string protectedText)
    {
        ArgumentNullException.ThrowIfNull(protectedText);
        return _protector.Unprotect(protectedText);
    }
}
