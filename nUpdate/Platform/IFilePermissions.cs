namespace nUpdate.Platform;

/// <summary>File permissions as Linux and macOS know them.</summary>
public interface IFilePermissions
{
    /// <summary>Whether the current user may create and delete files in the directory.</summary>
    bool CanWrite(string directory);

    /// <summary>Sets the Unix permission bits of the file, for example 0644. Does nothing on Windows.</summary>
    void SetMode(string path, int mode);
}
