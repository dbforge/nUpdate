using System.Security.Cryptography;

namespace nUpdate.Administration.Core.Security;

/// <summary>Creates random secrets and hashes them for the statistics script.</summary>
public static class SecretGenerator
{
    /// <summary>A URL-safe random secret of 32 bytes.</summary>
    public static string CreateSecret() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>The lowercase hex SHA-256 of the secret, which is what the PHP script stores.</summary>
    public static string HashSecret(string secret)
    {
        ArgumentException.ThrowIfNullOrEmpty(secret);
        return Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(secret))).ToLowerInvariant();
    }
}
