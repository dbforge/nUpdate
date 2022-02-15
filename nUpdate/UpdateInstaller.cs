using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    public class UpdateInstaller
    {
        private readonly string _installerPublicKey = "";
        private readonly string _installerFilePath = Path.Combine(Globals.ApplicationFilesDirectory, "nUpdate.UpdateInstaller.exe");
        private readonly Uri _updateInstallerUri = new Uri("https://www.nupdate.net/installer");

        public bool IsAvailable => File.Exists(_installerFilePath);

        public Task<SemanticVersion> GetCurrentVersion()
        {
            throw new NotImplementedException();
        }

        public async Task<(bool, UpdateInstallerBinary)> CheckForUpdate()
        {
            var configurationFileUri = new Uri(_updateInstallerUri, "versions.json");
            var checkerClient = new CustomWebClient(5000);
            var binaries = await checkerClient.DownloadFromJson<IEnumerable<UpdateInstallerBinary>>(configurationFileUri);
            var binaryDictionary = binaries.ToDictionary(binary => binary.Version);

            IEnumerable<SemanticVersion> versions = binaryDictionary.Keys;
            if (versions == null)
            {
                throw new InvalidOperationException(
                    "The update information for nUpdate.UpdateInstaller could not be fetched.");
            }

            var newestVersion = versions.Max();
            var currentVersion = await GetCurrentVersion();

            if (newestVersion != null)
            {
                bool newerVersionAvailable = newestVersion > currentVersion;
                return (newerVersionAvailable, binaryDictionary[newestVersion]);
            }
            else
            {
                return (false, null);
            }
        }

        public async Task DownloadInstaller(UpdateInstallerBinary binary, IProgress<UpdateProgressData> progress,
            CancellationToken cancellationToken)
        {
            var version = binary.Version.ToString();
            var installerFileUri = new Uri(_updateInstallerUri, Path.Combine(version, "nUpdate.UpdateInstaller.exe"));
            await DownloadManager.DownloadFile(installerFileUri, _installerFilePath, cancellationToken, progress);

            // TODO: Verify signature
        }
    }
}