using System.IO.Abstractions;

namespace nUpdate.Ui;

/// <summary>Checks whether the temp drive has room for the download plus the extracted copy.</summary>
internal static class UpdateSizeCheck
{
    /// <summary>The download and the extraction each need the package size, so twice the size must be free.</summary>
    public const int RequiredMultiple = 2;

    /// <summary>Returns <c>true</c> when enough space is free; otherwise the number of bytes that must be freed.</summary>
    public static bool HasEnoughSpace(IFileSystem fileSystem, long packageSize, out long bytesToFree)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));
        if (packageSize < 0)
            throw new ArgumentOutOfRangeException(nameof(packageSize));

        var tempPath = fileSystem.Path.GetTempPath();
        var root = fileSystem.Path.GetPathRoot(tempPath);
        if (string.IsNullOrEmpty(root))
            throw new InvalidOperationException($"The temp path \"{tempPath}\" has no drive root.");
        var available = fileSystem.DriveInfo.New(root!).AvailableFreeSpace;
        var required = packageSize * RequiredMultiple;
        bytesToFree = Math.Max(0, required - available);
        return bytesToFree == 0;
    }
}
