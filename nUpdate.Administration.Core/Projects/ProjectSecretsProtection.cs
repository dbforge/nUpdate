using System.Security.Cryptography;
using System.Text;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Projects;

/// <summary>Encrypts the secrets of a project under its project password for the <c>secrets</c> field of the project file.</summary>
public static class ProjectSecretsProtection
{
    /// <summary>The Base64 blob stored in the project file.</summary>
    public static string Protect(ProjectSecrets secrets, string password)
    {
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentException.ThrowIfNullOrEmpty(password);
        return Convert.ToBase64String(
            PasswordProtectedData.Encrypt(Encoding.UTF8.GetBytes(Serializer.Serialize(secrets)), password));
    }

    /// <exception cref="CryptographicException">The password is wrong.</exception>
    /// <exception cref="InvalidDataException">The blob is not a secrets blob.</exception>
    public static ProjectSecrets Unprotect(string blob, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(blob);
        ArgumentException.ThrowIfNullOrEmpty(password);
        byte[] data;
        try
        {
            data = Convert.FromBase64String(blob);
        }
        catch (FormatException ex)
        {
            throw new InvalidDataException("The secrets of the project file are not valid.", ex);
        }

        var json = Encoding.UTF8.GetString(PasswordProtectedData.Decrypt(data, password));
        try
        {
            return Serializer.Deserialize<ProjectSecrets>(json) ?? new ProjectSecrets();
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidDataException("The secrets of the project file are not valid.", ex);
        }
    }

    /// <summary>True when every secret the project needs is present.</summary>
    public static bool IsComplete(UpdateProject project, ProjectSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        if (string.IsNullOrEmpty(secrets.PrivateKey))
            return false;
        if (project.Transfer.Protocol != TransferProtocol.Sftp ||
            string.IsNullOrEmpty(project.Transfer.SftpPrivateKeyPath))
        {
            if (string.IsNullOrEmpty(secrets.TransferPassword))
                return false;
        }

        return !project.Statistics.Enabled || !string.IsNullOrEmpty(secrets.StatisticsAdminSecret);
    }
}
