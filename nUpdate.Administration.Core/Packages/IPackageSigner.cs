namespace nUpdate.Administration.Core.Packages;

/// <summary>Signs package files and verifies signatures with the project's RSA keys (PEM, RSA-PSS with SHA-512).</summary>
public interface IPackageSigner
{
    /// <returns>The Base64 signature stored in the feed.</returns>
    string Sign(string packagePath, string privateKeyPem);

    bool Verify(string packagePath, string publicKeyPem, string base64Signature);

    /// <summary>The Base64 SHA-512 of the file, as the feed carries it.</summary>
    string Hash(string packagePath);
}
