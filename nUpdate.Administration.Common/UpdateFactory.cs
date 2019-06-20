// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.Common.Logging;

namespace nUpdate.Administration.Common
{
    internal class UpdateFactory
    {
        private readonly UpdateProject _project;

        internal UpdateFactory(UpdateProject project)
        {
            _project = project;
        }
        internal IEnumerable<DefaultUpdatePackage> LoadPackageData()
        {
            return _project.Packages;
        }

        /*internal Task PushPackageData(DefaultUpdatePackage updatePackage, CancellationToken cancellationToken,
            IProgress<ITransferProgressData> progress)
        {
            var masterChannel =
                await
                    UpdateChannel.GetMasterChannel(new Uri(_project.UpdateDirectory, "masterchannel.json"),
                        null);
            var destinationChannel = masterChannel.FirstOrDefault(c => c.Name == updatePackage.ChannelName);
            if (destinationChannel == null)
                throw new InvalidOperationException("Invalid update channel.");

            var updatePackages =
                (await DefaultUpdatePackage.GetPackageEnumerable(destinationChannel.Uri, null)).ToList();
            updatePackages.Add(updatePackage);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes((string) JsonSerializer.Serialize(updatePackages))))
            {
                // TODO: Upload the package data
                await
                    ProjectSession.TransferManager.UploadFile(stream,
                        $"channels/{destinationChannel.Name.ToLowerInvariant()}.json", progress);
            }
        }*/

        internal async Task PushUpdateLocally(UpdateFactoryPackage factoryPackage)
        {
            await Task.Run(() =>
            {
                _project.Packages.Add(factoryPackage.PackageData);
                _project.Save();

                File.WriteAllText(
                    Path.Combine(PathProvider.Path, "Projects", _project.Guid.ToString(),
                        factoryPackage.PackageData.Guid.ToString(), "updates.json"),
                    JsonSerializer.Serialize(factoryPackage.PackageData));
            });
        }

        internal async Task PushUpdate(UpdateFactoryPackage factoryPackage, CancellationToken cancellationToken,
            IProgress<ITransferProgressData> progress)
        {
            var package = factoryPackage.PackageData;
            package.IsReleased = true;
            _project.Packages.Add(package);
            _project.Save();

            // Upload the package
            await
                ProjectSession.TransferManager.UploadPackage(factoryPackage.ArchivePath, factoryPackage.PackageData.Guid,
                    cancellationToken, progress);
            //await PushPackageData(factoryPackage.PackageData, cancellationToken, progress);

            ProjectSession.Logger.AppendEntry(PackageActionType.UploadPackage,
                $"{factoryPackage.PackageData.Version} ({factoryPackage.PackageData.ChannelName})");
        }

        /*internal Task RemovePackageData(SemanticVersion packageVersion,
            CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            var masterChannel =
                await
                    UpdateChannel.GetMasterChannel(new Uri(_project.UpdateDirectory, "masterchannel.json"),
                        null);
            var destinationChannel = masterChannel.FirstOrDefault(c => c.Name == updateChannelName);
            if (destinationChannel == null)
                throw new InvalidOperationException("Invalid update channel.");

            var updatePackages =
                (await DefaultUpdatePackage.GetPackageEnumerable(destinationChannel.Uri, null)).ToList();
            var destinationPackage =
                updatePackages.FirstOrDefault(item => item.Version.Equals(SemanticVersion));
            if (destinationPackage != null)
                updatePackages.Remove(destinationPackage);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes((string) JsonSerializer.Serialize(updatePackages))))
            {
                // TODO: Upload the package data
                await
                    ProjectSession.TransferManager.UploadFile(stream,
                        $"/channels/{destinationChannel.Name.ToLowerInvariant()}.json", progress);
            }
        }*/

        internal async Task RemoveUpdate(SemanticVersion packageVersion, string updateChannelName,
            CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            // First, remove the package data from the relating file. Background: If removing the package file fails, we can still be sure that the package won't be downloaded any longer.
            //await RemovePackageData(SemanticVersion, updateChannelName, cancellationToken, progress);

            if (await ProjectSession.TransferManager.Exists(packageVersion.ToString()))
                await ProjectSession.TransferManager.DeleteDirectory(packageVersion.ToString());

            var destinationPackage =
                _project.Packages.FirstOrDefault(
                    item => item.Version.Equals(packageVersion) && item.ChannelName.Equals(updateChannelName));
            if (destinationPackage != null)
            {
                _project.Packages.Remove(destinationPackage);
                _project.Save();
            }
        }
    }
}