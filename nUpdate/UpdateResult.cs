// Copyright © Dominic Beger 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    public class UpdateResult
    {
        private readonly List<DefaultUpdatePackage> _newUpdatePackages = new List<DefaultUpdatePackage>();

        /// <summary>
        ///     Gets all available <see cref="DefaultUpdatePackage" />s.
        /// </summary>
        public IEnumerable<DefaultUpdatePackage> Packages => _newUpdatePackages;

        /// <summary>
        ///     Gets a value indicating whether updates were found, or not.
        /// </summary>
        public bool UpdatesFound => _newUpdatePackages.Count > 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        internal Task Initialize(IEnumerable<DefaultUpdatePackage> packages, SemanticVersion applicationVersion,
            bool includePreRelease, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (applicationVersion == null)
                    throw new ArgumentNullException(nameof(applicationVersion));

                bool IsSuitablePackage(DefaultUpdatePackage package)
                {
                    var is64Bit = Environment.Is64BitOperatingSystem;
                    if (package.UnsupportedVersions != null &&
                        package.UnsupportedVersions.Any(
                            unsupportedVersion => unsupportedVersion.Equals(applicationVersion)))
                        return false;

                    return (package.Architecture != Architecture.X86 || !is64Bit) &&
                           (package.Architecture != Architecture.X64 || is64Bit);
                }

                var latestPackage = default(DefaultUpdatePackage);
                var latestVersion = applicationVersion;
                foreach (var package in packages.Where(IsSuitablePackage))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (package.Version > latestVersion)
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
            });
        }
    }
}