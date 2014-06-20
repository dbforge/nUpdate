using nUpdate.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nUpdate.Internal
{
    internal class UpdateResult
    {
        private Stack<UpdateConfiguration> newUpdatePackages = new Stack<UpdateConfiguration>();
        private UpdateConfiguration newestPackage;

        private bool updatesFound = false;
        private int newPackagesAmount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateResult"/> class.
        /// </summary>
        public UpdateResult(Stack<UpdateConfiguration> packageConfigurations, Version currentVersion, bool isAlphaWished, bool isBetaWished)
        {
            bool is64bit = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"));
            if (packageConfigurations != null)
            {
                foreach (UpdateConfiguration package in packageConfigurations.Where(item => new Version(item.UpdateVersion) > currentVersion))
                {
                    // Check if the version does have any things that will stop it from being available
                    if (((DevelopmentalStage)(Enum.Parse(typeof(DevelopmentalStage), package.DevelopmentalStage)) == DevelopmentalStage.Release) || (isAlphaWished && ((DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage), package.DevelopmentalStage)) == DevelopmentalStage.Alpha) ||
                            (isBetaWished && ((DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage), package.DevelopmentalStage)) == DevelopmentalStage.Beta))
                    {
                        if (package.UnsupportedVersions != null)
                        {
                            bool containsUnsupported = false;
                            foreach (string unsupportedVersion in package.UnsupportedVersions)
                            {
                                if (new Version(unsupportedVersion) == currentVersion)
                                {
                                    containsUnsupported = true;
                                }
                            }

                            if (containsUnsupported)
                            {
                                continue;
                            }
                        }

                        if (package.Environment != null)
                        {
                            bool isNotCurrentArchitecture = false;
                            if (package.Environment == "x86" && is64bit)
                            {
                                isNotCurrentArchitecture = true;
                            }
                            else if (package.Environment == "x64" && !is64bit)
                            {
                                isNotCurrentArchitecture = true;
                            }

                            if (isNotCurrentArchitecture)
                            {
                                continue;
                            }
                        }

                        this.newUpdatePackages.Push(package); 
                    }
                }
            }

            if (this.NewestPackages.Count != 0)
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
        /// Returns all new packages.
        /// </summary>
        internal Stack<UpdateConfiguration> NewestPackages
        {
            get
            {
                return this.newUpdatePackages;
            }
        }

        /// <summary>
        /// Returns the newest update package.
        /// </summary>
        public UpdateConfiguration NewestPackage
        {
            get
            {
                return this.newUpdatePackages.OrderBy(package => package.UpdateVersion).Last();
            }
        }
    }
}
