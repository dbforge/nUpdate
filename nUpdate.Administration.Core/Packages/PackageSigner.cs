using System.IO.Abstractions;
using System.Security.Cryptography;
using nUpdate.Security;

namespace nUpdate.Administration.Core.Packages;

public sealed class PackageSigner : IPackageSigner
{
    private readonly IFileSystem _fileSystem;

    public PackageSigner(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public string Sign(string packagePath, string privateKeyPem)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packagePath);
        using var signer = PackageSigning.FromPrivateKey(privateKeyPem);
        using var stream = _fileSystem.File.OpenRead(packagePath);
        return Convert.ToBase64String(signer.Sign(stream));
    }

    public bool Verify(string packagePath, string publicKeyPem, string base64Signature)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packagePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(base64Signature);
        using var verifier = PackageSigning.FromPublicKey(publicKeyPem);
        using var stream = _fileSystem.File.OpenRead(packagePath);
        return verifier.Verify(stream, Convert.FromBase64String(base64Signature));
    }

    public string Hash(string packagePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packagePath);
        using var stream = _fileSystem.File.OpenRead(packagePath);
        return Convert.ToBase64String(SHA512.HashData(stream));
    }
}
