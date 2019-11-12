// Copyright © Dominic Beger 2019

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    public class UpdateCheckResult
    {
        private readonly List<UpdatePackage> _newUpdatePackages = new List<UpdatePackage>();

        /// <summary>
        ///     Gets all available <see cref="UpdatePackage" />s.
        /// </summary>
        public IEnumerable<UpdatePackage> Packages => _newUpdatePackages;

        /// <summary>
        ///     Gets a value indicating whether updates were found, or not.
        /// </summary>
        public bool UpdatesFound => _newUpdatePackages.Count > 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateCheckResult" /> class.
        /// </summary>
        internal Task Initialize(IEnumerable<UpdatePackage> packages, IVersion applicationVersion,
            UpdateChannelFilter channelFilter, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (applicationVersion == null)
                    throw new ArgumentNullException(nameof(applicationVersion));

                bool IsSuitablePackage(UpdatePackage package)
                {
                    var is64Bit = Environment.Is64BitOperatingSystem;
                    if (channelFilter != UpdateChannelFilter.None)
                        return channelFilter.Mode == UpdateChannelFilter.ChannelFilterMode.SearchOnlyInSpecified &&
                               channelFilter.Channels.Contains(package.ChannelName) ||
                               channelFilter.Mode == UpdateChannelFilter.ChannelFilterMode.ExcludeSpecifiedFromSearch &&
                               !channelFilter.Channels.Contains(package.ChannelName);

                    if (package.UnsupportedVersions != null &&
                        package.UnsupportedVersions.Any(
                            unsupportedVersion => unsupportedVersion.Equals(applicationVersion)))
                        return false;

                    return (package.Architecture != Architecture.X86 || !is64Bit) &&
                           (package.Architecture != Architecture.X64 || is64Bit);
                }

                var latestPackage = default(UpdatePackage);
                var latestVersion = applicationVersion;
                foreach (var package in packages.Where(IsSuitablePackage))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (package.Version.CompareTo(latestVersion).Equals(-1))
                    {
                        // package.Version > applicationVersion is always true in here as we initialize latestVersion with applicationVersion and > is a strict order and thus, transitive
                        if (package.Compulsory)
                            _newUpdatePackages.Add(package);
                        latestPackage = package;
                        latestVersion = package.Version;
                    }
                }

                // If the package was initialized and if it's not already in there
                if (latestPackage != null && !_newUpdatePackages.Contains(latestPackage))
                    _newUpdatePackages.Add(latestPackage);
            }, cancellationToken);
        }
    }
}