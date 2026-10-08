using System.IO.Abstractions;
using System.Security.Cryptography;
using nUpdate.Administration.Core.Security;

namespace nUpdate.Administration.Core.Projects;

/// <summary>Remembers project passwords per user and machine so a project asks for its password once.</summary>
public interface IProjectPasswordStore
{
    /// <summary>The remembered password, or <c>null</c> when there is none or it was saved by another user or machine.</summary>
    Task<string?> GetAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task SetAsync(Guid projectId, string password, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid projectId, CancellationToken cancellationToken = default);
}

/// <summary>Keeps the passwords in <c>passwords.json</c>, each protected with the credential protector.</summary>
public sealed class ProjectPasswordStore : IProjectPasswordStore
{
    private readonly IFileSystem _fileSystem;
    private readonly AdministrationPaths _paths;
    private readonly ICredentialProtector _protector;

    public ProjectPasswordStore(IFileSystem fileSystem, AdministrationPaths paths, ICredentialProtector protector)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _paths = paths ?? throw new ArgumentNullException(nameof(paths));
        _protector = protector ?? throw new ArgumentNullException(nameof(protector));
    }

    public async Task<string?> GetAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entries = await ReadAsync(cancellationToken).ConfigureAwait(false);
        if (!entries.TryGetValue(projectId.ToString(), out var protectedPassword))
            return null;
        try
        {
            return _protector.Unprotect(protectedPassword);
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException)
        {
            return null;
        }
    }

    public async Task SetAsync(Guid projectId, string password, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        var entries = await ReadAsync(cancellationToken).ConfigureAwait(false);
        entries[projectId.ToString()] = _protector.Protect(password);
        await WriteAsync(entries, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entries = await ReadAsync(cancellationToken).ConfigureAwait(false);
        if (entries.Remove(projectId.ToString()))
            await WriteAsync(entries, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Dictionary<string, string>> ReadAsync(CancellationToken cancellationToken)
    {
        if (!_fileSystem.File.Exists(_paths.PasswordsFile))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var content = await _fileSystem.File.ReadAllTextAsync(_paths.PasswordsFile, cancellationToken).ConfigureAwait(false);
        Dictionary<string, string>? entries = null;
        try
        {
            entries = Serializer.Deserialize<Dictionary<string, string>>(content);
        }
        catch (Newtonsoft.Json.JsonException)
        {
            // A damaged file only loses the remembered passwords; they are asked for again.
        }

        return new Dictionary<string, string>(entries ?? [], StringComparer.OrdinalIgnoreCase);
    }

    private async Task WriteAsync(Dictionary<string, string> entries, CancellationToken cancellationToken)
    {
        _fileSystem.Directory.CreateDirectory(_paths.Root);
        await _fileSystem.File.WriteAllTextAsync(_paths.PasswordsFile, Serializer.Serialize(entries, indented: true), cancellationToken).ConfigureAwait(false);
    }
}
