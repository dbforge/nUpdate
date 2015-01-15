// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Linq;
using nUpdate.Core;

namespace nUpdate.Internal
{
    internal class UpdateResult
    {
        private UpdateConfiguration _newestConfiguration;
        private readonly List<UpdateConfiguration> _newUpdateConfigurations = new List<UpdateConfiguration>();
        private readonly bool _updatesFound;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        public UpdateResult(IEnumerable<UpdateConfiguration> packageConfigurations, UpdateVersion currentVersion,
            bool isAlphaWished, bool isBetaWished)
        {
            if (packageConfigurations != null)
            {
                var is64Bit = Environment.Is64BitOperatingSystem;
                foreach (
                    var config in
                        packageConfigurations.Where(item => new UpdateVersion(item.LiteralVersion) > currentVersion)
                            .Where(
                                config =>
                                    new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                    DevelopmentalStage.Release ||
                                    ((isAlphaWished &&
                                      new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                      DevelopmentalStage.Alpha) ||
                                     (isBetaWished &&
                                      new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                      DevelopmentalStage.Beta)))
                    )
                {
                    if (config.UnsupportedVersions != null)
                    {
                        if (
                            config.UnsupportedVersions.Any(
                                unsupportedVersion => new UpdateVersion(unsupportedVersion) == currentVersion))
                            continue;
                    }

                    if (config.Architecture == Architecture.x86 && is64Bit ||
                        config.Architecture == Architecture.x64 && !is64Bit)
                        continue;

                    _newUpdateConfigurations.Add(config);
                }
            }

            if (_newUpdateConfigurations.Count != 0)
                _updatesFound = true;
        }

        /// <summary>
        ///     Gets a value indicating whether updates where found or not.
        /// </summary>
        public bool UpdatesFound
        {
            get { return _updatesFound; }
        }

        /// <summary>
        ///     Returns all new configurations.
        /// </summary>
        internal IEnumerable<UpdateConfiguration> NewestConfigurations
        {
            get { return _newUpdateConfigurations; }
        }

        /// <summary>
        ///     Returns the newest update configuration.
        /// </summary>
        public UpdateConfiguration NewestConfiguration
        {
            get
            {
                var allVersions =
                    NewestConfigurations.Select(config => new UpdateVersion(config.LiteralVersion)).ToList();
                return
                    NewestConfigurations.First(
                        item => item.LiteralVersion == UpdateVersion.GetHighestUpdateVersion(allVersions).ToString());
            }
        }
    }
}