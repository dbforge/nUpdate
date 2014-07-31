using nUpdate.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nUpdate.Internal
{
    internal class UpdateResult
    {
        private List<UpdateConfiguration> newUpdatePackages = new List<UpdateConfiguration>();
        private UpdateConfiguration newestPackage;

        private bool updatesFound = false;
        private int newPackagesAmount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateResult"/> class.
        /// </summary>
        public UpdateResult(List<UpdateConfiguration> packageConfigurations, UpdateVersion currentVersion, bool isAlphaWished, bool isBetaWished)
        {
            if (packageConfigurations != null)
            {
                bool is64bit = Environment.Is64BitOperatingSystem;
                if (packageConfigurations != null)
                {
                    foreach (UpdateConfiguration config in packageConfigurations.Where(item => new UpdateVersion(item.Version) > currentVersion))
                    {
                        // Check if the version does have any things that will stop it from being available
                        if (new UpdateVersion(config.Version).DevelopmentalStage == DevelopmentalStage.Release || (isAlphaWished && new UpdateVersion(config.Version).DevelopmentalStage == DevelopmentalStage.Alpha ||
                                (isBetaWished && new UpdateVersion(config.Version).DevelopmentalStage == DevelopmentalStage.Beta)))
                        {
                            if (config.UnsupportedVersions != null)
                            {
                                bool containsUnsupported = false;
                                foreach (string unsupportedVersion in config.UnsupportedVersions)
                                {
                                    if (new UpdateVersion(unsupportedVersion) == currentVersion)
                                    {
                                        containsUnsupported = true;
                                    }
                                }

                                if (containsUnsupported)
                                {
                                    continue;
                                }
                            }

                            if (config.Architecture != null)
                            {
                                bool isNotCurrentArchitecture = false;
                                if (config.Architecture == "x86" && is64bit)
                                {
                                    isNotCurrentArchitecture = true;
                                }
                                else if (config.Architecture == "x64" && !is64bit)
                                {
                                    isNotCurrentArchitecture = true;
                                }

                                if (isNotCurrentArchitecture)
                                {
                                    continue;
                                }
                            }

                            this.newUpdatePackages.Add(config);
                        }
                    }
                }
            }

            if (this.NewestConfigurations.Count != 0)
            {
                this.updatesFound = true;
            }
        }

        /// <summary>
        /// Returns 'true' if there were updates found.
        /// </summary>
        public bool UpdatesFound {
            get
            {
                return this.updatesFound;
            }
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
        /// Returns all new configurations.
        /// </summary>
        internal List<UpdateConfiguration> NewestConfigurations
        {
            get
            {
                return this.newUpdatePackages;
            }
        }

        /// <summary>
        /// Returns the newest update configuration.
        /// </summary>
        public UpdateConfiguration NewestConfiguration
        {
            get
            {
                var allVersions = new List<UpdateVersion>();
                foreach (var config in this.NewestConfigurations)
                {
                    allVersions.Add(new UpdateVersion(config.Version));
                }
                return NewestConfigurations.Where(item => item.Version == UpdateVersion.GetHighestUpdateVersion(allVersions).ToString()).First();
            }
        }
    }
}
