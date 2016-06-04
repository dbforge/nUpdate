// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Linq;

namespace nUpdate
{
    internal class UpdateResult
    {
        private readonly List<UpdatePackage> _newUpdatePackages = new List<UpdatePackage>();
        // TODO: Requirements
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        public UpdateResult(IEnumerable<UpdatePackage> packages, IUpdateVersion currentVersion,
            bool includeAlpha, bool includeBeta)
        {
            if (packages != null)
            {
                var is64Bit = Environment.Is64BitOperatingSystem;
                foreach (
                    var config in
                        packages.Where(
                            item => new UpdateVersion(item.LiteralVersion).IsNewerThan(currentVersion) || item.NecessaryUpdate)
                            .Where(
                                config =>
                                    new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                    DevelopmentalStage.Release ||
                                    new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                    DevelopmentalStage.ReleaseCandidate ||
                                    ((includeAlpha &&
                                      new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                      DevelopmentalStage.Alpha) ||
                                     (includeBeta &&
                                      new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                      DevelopmentalStage.Beta)))
                    )
                {
                    if (config.UnsupportedVersions != null)
                    {
                        if (
                            config.UnsupportedVersions.Any(
                                unsupportedVersion =>
                                    new UpdateVersion(unsupportedVersion).BasicVersion == currentVersion.BasicVersion))
                            continue;
                    }

                    if (config.Architecture == Architecture.X86 && is64Bit ||
                        config.Architecture == Architecture.X64 && !is64Bit)
                        continue;

                    if (new UpdateVersion(config.LiteralVersion).IsOlderOrEqualTo(currentVersion))
                        continue;

                    if (config.UpdateRequirements != null &&
                        config.UpdateRequirements.Any(req => !req.CheckRequirement()))
                    {
                        UnfulfilledRequirements.Add(new UpdateVersion(config.LiteralVersion),
                            config.UpdateRequirements.Where(req => !req.CheckRequirement()).ToList());
                        continue;
                    }

                    _newUpdatePackages.Add(config);
                }

                var highestVersion =
                    UpdateVersion.GetHighestUpdateVersion(
                        _newUpdatePackages.Select(item => new UpdateVersion(item.LiteralVersion)));
                var ignoredVersions = _newUpdatePackages.Where(
                    item => new UpdateVersion(item.LiteralVersion).IsOlderThan(highestVersion) && !item.NecessaryUpdate).Select(item => new UpdateVersion(item.LiteralVersion));
                foreach (var ignoredVersion in ignoredVersions)
                {
                    _newUpdatePackages.Remove(
                        _newUpdatePackages.First(item => new UpdateVersion(item.LiteralVersion).IsEqualTo(ignoredVersion)));
                    UnfulfilledRequirements.Remove(ignoredVersion);
                }
            }

            UpdatesFound = _newUpdatePackages.Count > 0;
        }

        /// <summary>
        ///     Gets a value indicating whether updates were found, or not.
        /// </summary>
        public bool UpdatesFound { get; }

        /// <summary>
        ///     Gets the <see cref="UpdateRequirement"/>s of their relating <see cref="UpdateVersion"/> that have not been fulfilled.
        /// </summary>
        public Dictionary<UpdateVersion, List<UpdateRequirement>> UnfulfilledRequirements { get; } = new Dictionary<UpdateVersion, List<UpdateRequirement>>();

        /// <summary>
        ///     Gets all new <see cref="UpdatePackage"/>s.
        /// </summary>
        public IEnumerable<UpdatePackage> NewestPackages => _newUpdatePackages;
    }
}