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
            UpdateChannelFilter channelFilter, CancellationToken cancellationToken, List<KeyValuePair<string, string>> clientConfiguration = null)
        {
            return Task.Run(() =>
            {
                if (applicationVersion == null)
                    throw new ArgumentNullException(nameof(applicationVersion));

                bool IsSuitablePackage(UpdatePackage package)
                {
                    if (channelFilter != UpdateChannelFilter.None && channelFilter.Mode ==
                        UpdateChannelFilter.ChannelFilterMode.ExcludeSpecifiedFromSearch &&
                        channelFilter.Channels.Contains(package.ChannelName) ||
                        channelFilter.Mode == UpdateChannelFilter.ChannelFilterMode.SearchOnlyInSpecified &&
                        !channelFilter.Channels.Contains(package.ChannelName))
                        return false;

                    if (package.UnsupportedVersions != null &&
                        package.UnsupportedVersions.Any(
                            unsupportedVersion => unsupportedVersion.Equals(applicationVersion)))
                        return false;

                    if (!CheckConditions(clientConfiguration, package))
                        return false;

                    var is64Bit = Environment.Is64BitOperatingSystem;
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

        internal bool CheckConditions(List<KeyValuePair<string, string>> clientConfiguration,
            UpdatePackage package)
        {
            if (package.RolloutConditions == null || !package.RolloutConditions.Any())
                return true;

            if (clientConfiguration != null && clientConfiguration.Any())
            {
                // If any negative condition is met, this update does not interest us.
                if (clientConfiguration.Any(x => package.RolloutConditions.Where(n => n.IsNegativeCondition)
                    .Any(c =>
                        c.Key == x.Key &&
                        string.Equals(c.Value, x.Value,
                            StringComparison.CurrentCultureIgnoreCase))))
                    return false;

                switch (package.RolloutConditionMode)
                {
                    // If no positive condition is met, this update does not interest us.
                    case UpdateRolloutConditionMode.AtLeastOne:
                        if (package.RolloutConditions.Where(n => !n.IsNegativeCondition).All(x =>
                            !clientConfiguration.Any(c => c.Key == x.Key &&
                                                       string.Equals(c.Value, x.Value,
                                                           StringComparison.CurrentCultureIgnoreCase))))
                            return false;
                        break;

                    // If not all positive clientConfiguration are met, this update does not interest us.
                    case UpdateRolloutConditionMode.All:
                        if (package.RolloutConditions.Where(n => !n.IsNegativeCondition).Any(x =>
                            !clientConfiguration.Any(c => c.Key == x.Key && string.Equals(c.Value, x.Value,
                                                           StringComparison.CurrentCultureIgnoreCase))))
                            return false;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(package),
                            "Invalid rollout condition mode.");
                }
            }
            else
            {
                // This should be discussed again as it may be senseful that a user with no local key for a positive condition may eventually also receive the update.
                if (package.RolloutConditions.Any(c => !c.IsNegativeCondition))
                    return false;
            }

            return true;
        }
    }
}