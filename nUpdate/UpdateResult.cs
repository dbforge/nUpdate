// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nUpdate
{
    internal class UpdateResult
    {
        private readonly List<UpdatePackage> _newUpdatePackages = new List<UpdatePackage>();

        // TODO: Requirements
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        internal async Task Initialize(IEnumerable<UpdateChannel> masterChannel, Version applicationVersion, 
            string applicationChannelName, string updateChannelName)
        {
            if (masterChannel == null)
                throw new ArgumentNullException(nameof(masterChannel));
            if (applicationVersion == null)
                throw new ArgumentNullException(nameof(applicationVersion));
            if (applicationChannelName == null)
                throw new ArgumentNullException(nameof(applicationChannelName));

            // Filter the channels that we want to check out in our update search from our master channel...
            var shouldStop = false;
            var filteredChannels = masterChannel.Reverse().TakeWhile(x => !shouldStop && ((shouldStop = string.Equals(x.Name, updateChannelName)) || !shouldStop)).Reverse().ToList();
            var filteredChannelNames = filteredChannels.Select(x => x.Name).ToList();
            
            Func<Version, Version> getBaseVersion = version => 
                new Version(version.Major, version.Minor, version.Build);
            Func<UpdatePackage, bool> isSuitablePackage = package =>
            {
                var is64Bit = Environment.Is64BitOperatingSystem;
                if (package.UnsupportedVersions != null &&
                    package.UnsupportedVersions.Any(
                        unsupportedVersion =>
                            unsupportedVersion == applicationVersion.ToString()))
                    return false;

                return (package.Architecture != Architecture.X86 || !is64Bit) &&
                       (package.Architecture != Architecture.X64 || is64Bit);
            };

            var latestPackage = default(UpdatePackage);
            var latestVersion = applicationVersion;
            var latestChannelName = applicationChannelName;
            foreach (var channel in filteredChannels)
            {
                var packageData = await UpdatePackage.GetRemotePackageData(channel.Uri, null);
                foreach (var package in packageData.Where(package => isSuitablePackage(package)))
                {
                    // Check if the version is greater than the current one of the application and if it's a necessary update...
                    // If so, this version should definitely be installed, even if newer versions are available.
                    if ((getBaseVersion(package.Version) > getBaseVersion(applicationVersion) ||
                         (getBaseVersion(package.Version) == getBaseVersion(applicationVersion) &&
                          filteredChannelNames.IndexOf(channel.Name) > filteredChannelNames.IndexOf(applicationChannelName))) &&
                        package.NecessaryUpdate)
                    {
                        // We add it directly to the list.
                        _newUpdatePackages.Add(package);
                    }

                    // Readability
                    // ReSharper disable once InvertIf

                    // Check if the current version is greater than the currently newest one...
                    // If it's equal to it but its update channel has a higher priority (e.g. 0.1.0-beta is newer than 0.1.0-alpha), it's also a a newer version that we should handle.
                    if (getBaseVersion(package.Version) > getBaseVersion(latestVersion) ||
                        (getBaseVersion(package.Version) == getBaseVersion(latestVersion) &&
                         filteredChannelNames.IndexOf(channel.Name) > filteredChannelNames.IndexOf(latestChannelName)))
                    {
                        // This version is newer than the old one and needs to replace it.
                        latestVersion = package.Version;
                        latestPackage = package;
                        latestChannelName = channel.Name;
                    }
                }
            }

            // If the package was initialized and if it's not already in there
            if (latestPackage != null && !_newUpdatePackages.Contains(latestPackage))
                _newUpdatePackages.Add(latestPackage);
        }

        /// <summary>
        ///     Gets a value indicating whether updates were found, or not.
        /// </summary>
        public bool UpdatesFound => _newUpdatePackages.Count > 0;

        /// <summary>
        ///     Gets all new <see cref="UpdatePackage" />s.
        /// </summary>
        public IEnumerable<UpdatePackage> NewestPackages => _newUpdatePackages;
    }
}