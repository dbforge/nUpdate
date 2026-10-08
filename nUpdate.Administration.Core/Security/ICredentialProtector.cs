namespace nUpdate.Administration.Core.Security;

/// <summary>Protects secrets for storage in project files, bound to the current user and machine.</summary>
public interface ICredentialProtector
{
    /// <summary>Returns an opaque string that only this user on this machine can unprotect.</summary>
    string Protect(string plainText);

    /// <exception cref="System.Security.Cryptography.CryptographicException">The value was not protected by this user and machine.</exception>
    string Unprotect(string protectedText);
}
