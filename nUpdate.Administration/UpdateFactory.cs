// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.Logging;

namespace nUpdate.Administration
{
    internal class UpdateFactory
    {
        private readonly UpdateProject _project;

        internal UpdateFactory(UpdateProject project)
        {
            _project = project;
        }

        internal async Task<bool> CheckUpdateChannels(IEnumerable<UpdateChannel> masterChannel)
        {
            if (!await ProjectSession.TransferManager.Exists("channels"))
                return false;
            return
                await
                    masterChannel.AllAsync(
                        c => ProjectSession.TransferManager.Exists($"channels/{c.Name.ToLowerInvariant()}.json"));
        }

        internal IEnumerable<UpdatePackage> LoadPackageData()
        {
            return _project.Packages;
        }

        internal async Task PushPackageData(UpdatePackage updatePackage, CancellationToken cancellationToken,
            IProgress<ITransferProgressData> progress)
        {
            var masterChannel =
                await
                    UpdateChannel.GetMasterChannel(new Uri(_project.UpdateDirectory, "masterchannel.json"),
                        _project.ProxyData.Proxy);
            var destinationChannel = masterChannel.FirstOrDefault(c => c.Name == updatePackage.ChannelName);
            if (destinationChannel == null)
                throw new InvalidOperationException("Invalid update channel.");

            var updatePackages =
                (await UpdatePackage.GetRemotePackageData(destinationChannel.Uri, _project.ProxyData.Proxy)).ToList();
            updatePackages.Add(updatePackage);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(updatePackages))))
            {
                // TODO: Upload the package data
                await
                    ProjectSession.TransferManager.UploadFile(stream,
                        $"channels/{destinationChannel.Name.ToLowerInvariant()}.json", progress);
            }
        }

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
            await PushPackageData(factoryPackage.PackageData, cancellationToken, progress);

            ProjectSession.Logger.AppendEntry(PackageActionType.UploadPackage,
                $"{factoryPackage.PackageData.Version} ({factoryPackage.PackageData.ChannelName})");
        }

        internal async Task RemovePackageData(Version updateVersion, UpdateChannel updateChannel,
            CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            await RemovePackageData(updateVersion, updateChannel.Name, cancellationToken, progress);
        }

        internal async Task RemovePackageData(Version updateVersion, string updateChannelName,
            CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            var masterChannel =
                await
                    UpdateChannel.GetMasterChannel(new Uri(_project.UpdateDirectory, "masterchannel.json"),
                        _project.ProxyData.Proxy);
            var destinationChannel = masterChannel.FirstOrDefault(c => c.Name == updateChannelName);
            if (destinationChannel == null)
                throw new InvalidOperationException("Invalid update channel.");

            var updatePackages =
                (await UpdatePackage.GetRemotePackageData(destinationChannel.Uri, _project.ProxyData.Proxy)).ToList();
            var destinationPackage =
                updatePackages.FirstOrDefault(item => item.Version.Equals(updateVersion));
            if (destinationPackage != null)
                updatePackages.Remove(destinationPackage);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(updatePackages))))
            {
                // TODO: Upload the package data
                await
                    ProjectSession.TransferManager.UploadFile(stream,
                        $"/channels/{destinationChannel.Name.ToLowerInvariant()}.json", progress);
            }
        }

        internal async Task RemoveUpdate(Version updateVersion, string updateChannelName,
            CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            // First, remove the package data from the relating file. Background: If removing the package file fails, we can still be sure that the package won't be downloaded any longer.
            await RemovePackageData(updateVersion, updateChannelName, cancellationToken, progress);

            if (await ProjectSession.TransferManager.Exists(updateVersion.ToString()))
                await ProjectSession.TransferManager.DeleteDirectory(updateVersion.ToString());

            var destinationPackage =
                _project.Packages.FirstOrDefault(
                    item => item.Version.Equals(updateVersion) && item.ChannelName.Equals(updateChannelName));
            if (destinationPackage != null)
            {
                _project.Packages.Remove(destinationPackage);
                _project.Save();
            }
        }

        internal async Task SynchronizeMasterChannel()
        {
            // If the master channel is also no longer available inside the local project file, we just use the default one.
            if (ProjectSession.ActiveProject.MasterChannel == null)
            {
                ProjectSession.ActiveProject.MasterChannel =
                    UpdateChannel.GetDefaultMasterChannel(ProjectSession.ActiveProject.UpdateDirectory).ToList();
            }

            using (
                var stream =
                    new MemoryStream(
                        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ProjectSession.ActiveProject.MasterChannel))))
            {
                await
                    ProjectSession.TransferManager.UploadFile(stream, "masterchannel.json", null);
            }
        }

        internal async Task SynchronizeUpdateChannels(IEnumerable<UpdateChannel> masterChannel)
        {
            var masterChannelArray = masterChannel.ToArray();

            // Create the "channels" folder, if it does not yet exist.
            if (!await ProjectSession.TransferManager.Exists("channels"))
                await ProjectSession.TransferManager.MakeDirectory("channels");

            // Upload all the packages to the associated update channel, if it is not yet existing.
            await masterChannelArray.ForEachAsync(async channel =>
            {
                if (
                    (await
                        ProjectSession.TransferManager.Exists($"channels/{channel.Name.ToLowerInvariant()}.json")))
                    return;

                var channelPackages = ProjectSession.ActiveProject.Packages.Where(p => p.ChannelName == channel.Name);
                using (
                    var stream =
                        new MemoryStream(
                            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(channelPackages))))
                {
                    await
                        ProjectSession.TransferManager.UploadFile(stream, $"channels/{channel.Name.ToLowerInvariant()}.json",
                            null);
                }
            });
        }
    }
}