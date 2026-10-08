namespace nUpdate.Administration.Core.Transfer;

/// <summary>Joins the project's base directory with relative remote paths.</summary>
public static class RemotePath
{
    /// <summary>Returns an absolute remote path with <c>/</c> separators and no trailing slash (except the root).</summary>
    public static string Combine(string baseDirectory, string relativePath)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);
        ArgumentNullException.ThrowIfNull(relativePath);

        var parts = new List<string>();
        foreach (var part in (baseDirectory + "/" + relativePath).Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (part == ".")
                continue;
            if (part == "..")
                throw new ArgumentException("Remote paths must not contain '..' segments.", nameof(relativePath));
            parts.Add(part);
        }

        return "/" + string.Join("/", parts);
    }

    /// <summary>The parent of a remote path, or <c>/</c> for the root.</summary>
    public static string Parent(string remotePath)
    {
        ArgumentNullException.ThrowIfNull(remotePath);
        var trimmed = remotePath.TrimEnd('/');
        var slash = trimmed.LastIndexOf('/');
        return slash <= 0 ? "/" : trimmed[..slash];
    }

    public static string FileName(string remotePath)
    {
        ArgumentNullException.ThrowIfNull(remotePath);
        var trimmed = remotePath.TrimEnd('/');
        return trimmed[(trimmed.LastIndexOf('/') + 1)..];
    }
}
