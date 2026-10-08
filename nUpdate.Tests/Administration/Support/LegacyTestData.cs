using System.IO.Compression;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Administration.Support;

/// <summary>Files in the formats nUpdate 4 wrote: operations.json, package zips, their signatures and updates.json.</summary>
public static class LegacyTestData
{
    /// <summary>An operations.json of nUpdate 4, including entries nUpdate 5 cannot convert.</summary>
    public const string LegacyOperations = """
        [
          {"Area":0,"Method":1,"Value":"%program%","Value2":["old.dll"],"ExecuteBeforeReplacingFiles":false},
          {"Area":0,"Method":2,"Value":"%program%\\keep.txt","Value2":"kept.txt","ExecuteBeforeReplacingFiles":false},
          {"Area":1,"Method":0,"Value":"HKEY_CURRENT_USER\\Software\\App","Value2":["Sub"]},
          {"Area":1,"Method":1,"Value":"HKEY_CURRENT_USER\\Software\\App","Value2":["Old"]},
          {"Area":1,"Method":3,"Value":"HKEY_CURRENT_USER\\Software\\App","Value2":[
             {"Item1":"Installed","Item2":42,"Item3":4}, {"Item1":"Big","Item2":"7","Item3":11}, {"Item1":"Text","Item2":"hello","Item3":1},
             {"Item1":"Expand","Item2":"%TEMP%","Item3":2}, {"Item1":"Multi","Item2":"a, b","Item3":7}, {"Item1":"MultiArray","Item2":["x"],"Item3":"MultiString"},
             {"Item1":"Bytes","Item2":"1,2,3","Item3":3}, {"Item1":"Base64","Item2":"AQID","Item3":3}, {"Item1":"ByteArray","Item2":[9],"Item3":3},
             {"Item1":"Bad","Item2":"x","Item3":4}, {"Item2":"nameless"}, {"Item1":"Default","Item2":null,"Item3":99}]},
          {"Area":1,"Method":4,"Value":"HKEY_CURRENT_USER\\Software\\App","Value2":["Gone"]},
          {"Area":2,"Method":5,"Value":"%program%\\tool.exe","Value2":"--flag"},
          {"Area":2,"Method":6,"Value":"helper","Value2":null,"ExecuteBeforeReplacingFiles":true},
          {"Area":"Services","Method":"Start","Value":"svc","Value2":["-a","-b"]},
          {"Area":3,"Method":6,"Value":"svc"},
          {"Area":4,"Method":7,"Value":"class P {}"},
          {"Area":4,"Method":0,"Value":"unknown combination"},
          {"Area":9,"Method":0,"Value":"unknown area"},
          "not an object"
        ]
        """;

    /// <summary>A package zip of nUpdate 4: the four root folders, the <paramref name="files" /> and, if wanted, <see cref="LegacyOperations" /> as operations.json.</summary>
    public static byte[] LegacyZip(bool withOperations, params (string Name, string Content)[] files)
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

            if (withOperations)
            {
                using var writer = new StreamWriter(archive.CreateEntry("operations.json").Open());
                writer.Write(LegacyOperations);
            }
        }

        return stream.ToArray();
    }

    /// <summary>The signature nUpdate 4 wrote into updates.json: RSA PKCS#1 v1.5 with SHA-512 over the zip.</summary>
    public static string Sign(byte[] zip)
    {
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportFromPem(TestKeys.PrivateKey);
        return Convert.ToBase64String(rsa.SignData(zip, System.Security.Cryptography.HashAlgorithmName.SHA512, System.Security.Cryptography.RSASignaturePadding.Pkcs1));
    }

    /// <summary>An updates.json with 1.0.0.0 and 1.1.0.0b2 below https://updates.example.com/demo/, signed with <paramref name="first" /> and <paramref name="second" />.</summary>
    public static string LegacyFeedJson(string statistics = "false", string first = "old", string second = "old") => $$"""
        [
          {"LiteralVersion":"1.0.0.0","Architecture":2,"Changelog":{"en":"First","de-DE":"Erste"},"NecessaryUpdate":false,"RolloutConditionMode":0,"RolloutConditions":null,
           "UpdatePackageUri":"https://updates.example.com/demo/1.0.0.0/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip","UpdatePhpFileUri":null,"UseStatistics":{{statistics}},"Signature":"{{first}}","UnsupportedVersions":null},
          {"LiteralVersion":"1.1.0.0b2","Architecture":1,"Changelog":{"en":"Beta"},"NecessaryUpdate":true,"RolloutConditionMode":1,
           "RolloutConditions":[{"Key":"R","Value":"east","IsNegativeCondition":true},{"Key":"","Value":"x"}],"UnsupportedVersions":["0.9.0.0","garbage"],
           "UpdatePackageUri":"https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip","UseStatistics":true,"Signature":"{{second}}",
           "Operations":[{"Area":2,"Method":6,"Value":"helper","ExecuteBeforeReplacingFiles":true}]}
        ]
        """;
}
