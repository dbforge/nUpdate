using System;
using System.Collections.Generic;
using System.Linq;
using nUpdate.Core;

namespace nUpdate.Internal
{
    internal class UpdateResult
    {
        private readonly List<UpdateConfiguration> _newUpdatePackages = new List<UpdateConfiguration>();

        private readonly bool _updatesFound;
        private int _newPackagesAmount = 0;
        private UpdateConfiguration _newestPackage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        public UpdateResult(IEnumerable<UpdateConfiguration> packageConfigurations, UpdateVersion currentVersion,
            bool isAlphaWished, bool isBetaWished)
        {
            if (packageConfigurations != null)
            {
                bool is64Bit = Environment.Is64BitOperatingSystem;
                foreach (
                    UpdateConfiguration config in
                        packageConfigurations.Where(item => new UpdateVersion(item.Version) > currentVersion))
                {
                    // Check if the version does have any things that will stop it from being available
                    if (new UpdateVersion(config.Version).DevelopmentalStage == DevelopmentalStage.Release ||
                        (isAlphaWished &&
                         new UpdateVersion(config.Version).DevelopmentalStage == DevelopmentalStage.Alpha ||
                         (isBetaWished &&
                          new UpdateVersion(config.Version).DevelopmentalStage == DevelopmentalStage.Beta)))
                    {
                        if (config.UnsupportedVersions != null)
                        {
                            bool containsUnsupported = false;
                            foreach (string unsupportedVersion in config.UnsupportedVersions)
                            {
                                if (new UpdateVersion(unsupportedVersion) == currentVersion)
                                    containsUnsupported = true;
                            }

                            if (containsUnsupported)
                                continue;
                        }

                        if (config.Architecture != null)
                        {
                            bool isNotCurrentArchitecture = false;
                            if (config.Architecture == "x86" && is64Bit)
                                isNotCurrentArchitecture = true;
                            else if (config.Architecture == "x64" && !is64Bit)
                                isNotCurrentArchitecture = true;

                            if (isNotCurrentArchitecture)
                                continue;
                        }

                        _newUpdatePackages.Add(config);
                    }
                }
            }

            if (NewestConfigurations.Count != 0)
                _updatesFound = true;
        }

        /// <summary>
        ///     Returns 'true' if there were updates found.
        /// </summary>
        public bool UpdatesFound
        {
            get { return _updatesFound; }
        }

        ///// <summary>
        ///// Returns the amount of new packages found.
        ///// </summary>
        //public int NewPackages {
        //    get {
        //        return newPackagesAmount;
        //    }
        //}

        /// <summary>
        ///     Returns all new configurations.
        /// </summary>
        internal List<UpdateConfiguration> NewestConfigurations
        {
            get { return _newUpdatePackages; }
        }

        /// <summary>
        ///     Returns the newest update configuration.
        /// </summary>
        public UpdateConfiguration NewestConfiguration
        {
            get
            {
                var allVersions = NewestConfigurations.Select(config => new UpdateVersion(config.Version)).ToList();
                return
                    NewestConfigurations.First(item => item.Version == UpdateVersion.GetHighestUpdateVersion(allVersions).ToString());
            }
        }
    }
}