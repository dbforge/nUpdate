using System.IO.Compression;
using System.Text;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Tests.Support;

/// <summary>Builds package zips the way nUpdate Administration does: a payload file below <c>Program/</c> plus a <c>manifest.json</c>.</summary>
public static class TestPackages
{
    /// <summary>Every entry gets this time, so building the same package twice gives the same bytes.</summary>
    private static readonly DateTimeOffset EntryTime = new(2026, 10, 7, 12, 0, 0, TimeSpan.Zero);

    /// <summary>A zip whose manifest names the version, project and platform, carrying the payload as <c>Program/payload.bin</c>.</summary>
    public static byte[] Build(string version, Guid projectId, byte[] payload, string? manifestJson = null,
        string platform = PackagePlatform.Any)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry("Program/payload.bin", CompressionLevel.NoCompression);
            entry.LastWriteTime = EntryTime;
            using (var output = entry.Open())
                output.Write(payload, 0, payload.Length);
            var manifest = archive.CreateEntry(PackageLayout.ManifestFileName, CompressionLevel.NoCompression);
            manifest.LastWriteTime = EntryTime;
            using var writer = new StreamWriter(manifest.Open(), new UTF8Encoding(false));
            writer.Write(manifestJson ?? Serializer.Serialize(new PackageManifest
            {
                ProjectId = projectId,
                Version = new UpdateVersion(version),
                Platform = platform,
                CreatedAt = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero)
            }));
        }

        return stream.ToArray();
    }

    /// <summary>A zip without a manifest, as nUpdate 4 built them.</summary>
    public static byte[] WithoutManifest(byte[] payload)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry("Program/payload.bin", CompressionLevel.NoCompression);
            entry.LastWriteTime = EntryTime;
            using var output = entry.Open();
            output.Write(payload, 0, payload.Length);
        }

        return stream.ToArray();
    }
}
