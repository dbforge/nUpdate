// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Linq;

namespace nUpdate
{
    public class UpdateResult
    {
        private readonly List<UpdatePackage> _newUpdatePackages = new List<UpdatePackage>();

        internal UpdateResult(IEnumerable<UpdatePackage> newPackages)
        {
            _newUpdatePackages.AddRange(newPackages);
        }

        // TODO: Requirements
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        internal UpdateResult(IEnumerable<UpdatePackage> packages, IUpdateVersion currentVersion,
            bool includeAlpha, bool includeBeta)
        {
            if (packages == null)
                throw new ArgumentNullException(nameof(packages));

            var is64Bit = Environment.Is64BitOperatingSystem;
            foreach (
                var package in
                    packages.Where(
                        item => new UpdateVersion(item.LiteralVersion).IsNewerThan(currentVersion)))
            {
                var packageVersion = new UpdateVersion(package.LiteralVersion);

                // If it's an Alpha-version or Beta-version and we shouldn't include them...
                if ((!includeAlpha &&
                     packageVersion.DevelopmentalStage ==
                     DevelopmentalStage.Alpha) ||
                    (!includeBeta &&
                     packageVersion.DevelopmentalStage ==
                     DevelopmentalStage.Beta))
                    continue;

                if (package.UnsupportedVersions != null &&
                    package.UnsupportedVersions.Any(
                        unsupportedVersion =>
                            new UpdateVersion(unsupportedVersion).BasicVersion == currentVersion.BasicVersion))
                    continue;

                if (package.Architecture == Architecture.X86 && is64Bit ||
                    package.Architecture == Architecture.X64 && !is64Bit)
                    continue;

                if (package.UpdateRequirements != null &&
                    package.UpdateRequirements.Any(req => !req.CheckRequirement()))
                {
                    UnfulfilledRequirements.Add(packageVersion,
                        package.UpdateRequirements.Where(req => !req.CheckRequirement()).ToList());
                    continue;
                }

                _newUpdatePackages.Add(package);
            }

            var highestVersion =
                UpdateVersion.GetHighestUpdateVersion(
                    _newUpdatePackages.Select(item => new UpdateVersion(item.LiteralVersion)));
            var ignoredVersions = _newUpdatePackages.Where(
                item => new UpdateVersion(item.LiteralVersion).IsOlderThan(highestVersion) && !item.NecessaryUpdate)
                .Select(item => new UpdateVersion(item.LiteralVersion));
            foreach (var ignoredVersion in ignoredVersions)
            {
                _newUpdatePackages.Remove(
                    _newUpdatePackages.First(
                        item => new UpdateVersion(item.LiteralVersion).IsEqualTo(ignoredVersion)));
                UnfulfilledRequirements.Remove(ignoredVersion);
            }
        }

        internal static UpdateResult Empty()
        {
            return new UpdateResult(Enumerable.Empty<UpdatePackage>());
        }

        /// <summary>
        ///     Gets a value indicating whether updates were found, or not.
        /// </summary>
        public bool UpdatesFound => _newUpdatePackages.Count > 0;

        /// <summary>
        ///     Gets the <see cref="UpdateRequirement" />s of their relating <see cref="UpdateVersion" /> that have not been
        ///     fulfilled.
        /// </summary>
        public Dictionary<UpdateVersion, List<UpdateRequirement>> UnfulfilledRequirements { get; } =
            new Dictionary<UpdateVersion, List<UpdateRequirement>>();

        /// <summary>
        ///     Gets all new <see cref="UpdatePackage" />s.
        /// </summary>
        public IEnumerable<UpdatePackage> NewestPackages => _newUpdatePackages;
    }
}