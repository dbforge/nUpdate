using System.IO.Compression;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Integration.Support;

/// <summary>Puts the files of a project published by nUpdate Administration 3.x or 4.x onto the server: <c>updates.json</c> and <c>&lt;version&gt;/&lt;project id&gt;.zip</c>.</summary>
public static class LegacyServer
{
    public const string LegacyOperations = """
        [
          {"Area":0,"Method":1,"Value":"%program%","Value2":["obsolete.dll"],"ExecuteBeforeReplacingFiles":false},
          {"Area":2,"Method":6,"Value":"helper","Value2":null,"ExecuteBeforeReplacingFiles":true}
        ]
        """;

    /// <summary>A package zip as the old administration built it: the four root folders, the files and operations.json.</summary>
    public static byte[] Zip(params (string Name, string Content)[] files)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var root in new[] { "Program/", "AppData/", "Temp/", "Desktop/" })
                archive.CreateEntry(root);
            foreach (var (name, content) in files)
            {
                using var writer = new StreamWriter(archive.CreateEntry(name).Open());
                writer.Write(content);
            }

            using var operations = new StreamWriter(archive.CreateEntry("operations.json").Open());
            operations.Write(LegacyOperations);
        }

        return stream.ToArray();
    }

    /// <summary>The signature nUpdate 4 stored in updates.json: RSA PKCS#1 v1.5 with SHA-512 over the zip, with the key pair of the test projects.</summary>
    public static string Sign(byte[] zip)
    {
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportFromPem(nUpdate.Tests.Support.TestKeys.PrivateKey);
        return Convert.ToBase64String(rsa.SignData(zip, System.Security.Cryptography.HashAlgorithmName.SHA512, System.Security.Cryptography.RSASignaturePadding.Pkcs1));
    }

    /// <summary>The updates.json of nUpdate 4 for one package.</summary>
    public static string Feed(string baseUrl, Guid projectId, string literalVersion, string signature) => $$"""
        [
          {"LiteralVersion":"{{literalVersion}}","Architecture":2,"Changelog":{"en":"Legacy release","de-DE":"Alte Version"},"NecessaryUpdate":true,
           "RolloutConditionMode":0,"RolloutConditions":null,"UnsupportedVersions":null,"Signature":"{{signature}}",
           "UpdatePackageUri":"{{baseUrl}}{{literalVersion}}/{{projectId}}.zip","UpdatePhpFileUri":null,"UseStatistics":false,"ProjectId":"{{projectId}}"}
        ]
        """;

    /// <summary>What answers at statistics.php for clients of nUpdate 4 in these tests; the migration must leave it alone.</summary>
    public const string StatisticsScriptOutput = "statistics of nUpdate 4";

    /// <summary>Uploads a stand-in for the statistics.php of nUpdate 4.</summary>
    public static Task PublishStatisticsScriptAsync(ITransferProvider transfer, IntegrationContext context) =>
        transfer.UploadFileAsync(context.WriteFile("legacy/statistics.php", $"<?php echo '{StatisticsScriptOutput}';"), "statistics.php");

    /// <summary>Uploads the legacy feed and the package through the given connected provider.</summary>
    public static async Task PublishAsync(ITransferProvider transfer, IntegrationContext context, Guid projectId, string literalVersion, byte[] zip)
    {
        await transfer.CreateDirectoryAsync(literalVersion);
        await transfer.UploadFileAsync(context.WriteBytes($"legacy/{literalVersion}/{projectId}.zip", zip), $"{literalVersion}/{projectId}.zip");
        await transfer.UploadFileAsync(context.WriteFile("legacy/updates.json", Feed(context.Server.HttpBaseUrl, projectId, literalVersion, Sign(zip))), "updates.json");
    }
}
