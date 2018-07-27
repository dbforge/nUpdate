// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Linq;
using nUpdate.Internal.Core;

namespace nUpdate.Updating
{
    internal class UpdateResult
    {
        private readonly List<UpdateConfiguration> _newUpdateConfigurations = new List<UpdateConfiguration>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        public UpdateResult(IEnumerable<UpdateConfiguration> packageConfigurations, UpdateVersion currentVersion,
            bool isAlphaWished, bool isBetaWished, List<KeyValuePair<string,string>> conditions = null)
        {
            if (packageConfigurations != null)
            {
                var is64Bit = Environment.Is64BitOperatingSystem;
                foreach (
                    var config in
                    packageConfigurations.Where(
                            item => new UpdateVersion(item.LiteralVersion) > currentVersion || item.NecessaryUpdate)
                        .Where(
                            config =>
                                new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                DevelopmentalStage.Release ||
                                new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                DevelopmentalStage.ReleaseCandidate || isAlphaWished &&
                                new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                DevelopmentalStage.Alpha || isBetaWished &&
                                new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                DevelopmentalStage.Beta)
                )
                {
                    if (config.UnsupportedVersions != null)
                        if (
                            config.UnsupportedVersions.Any(
                                unsupportedVersion =>
                                    new UpdateVersion(unsupportedVersion).BasicVersion == currentVersion.BasicVersion))
                            continue;

                    if (config.Architecture == Architecture.X86 && is64Bit ||
                        config.Architecture == Architecture.X64 && !is64Bit)
                        continue;


                    //Only check conditions if the remote conditions are not null or collection is empty
                    if (config.Rolloutconditions != null && config.Rolloutconditions.Count != 0)
                        //If the local conditions are null or the collection is empty then continue
                        if (conditions == null || !conditions.Any())
                        {
                            continue;
                        }
                        else
                        {
                            //Check if key exist
                            if (config.Rolloutconditions.Any(cond => conditions.Select(s => s.Key).Contains(cond.Key)))

                            {
                                bool doUpdate = false;
                                foreach (var localCondition in conditions)
                                {
                                    //If a positive condition exist then do update
                                    doUpdate = config.Rolloutconditions.Where(n => !n.IsNegativeCondition).Any(c =>
                                        c.Key == localCondition.Key &&
                                        String.Equals(c.Value, localCondition.Value, StringComparison.CurrentCultureIgnoreCase));
                                    if (doUpdate) break;
                                }

                                foreach (var localCondition in conditions)
                                {
                                    //If a negative condition exist then refuse update
                                    doUpdate = !config.Rolloutconditions.Where(n => n.IsNegativeCondition).Any(c =>
                                        c.Key == localCondition.Key &&
                                        String.Equals(c.Value, localCondition.Value, StringComparison.CurrentCultureIgnoreCase));
                                    if (!doUpdate) break;
                                }

                                if (!doUpdate) continue;
                            }
                        }
                    {


                    }


                    if (new UpdateVersion(config.LiteralVersion) <= currentVersion)
                        continue;

                    _newUpdateConfigurations.Add(config);
                }

                var highestVersion =
                    UpdateVersion.GetHighestUpdateVersion(
                        _newUpdateConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)));
                _newUpdateConfigurations.RemoveAll(
                    item => new UpdateVersion(item.LiteralVersion) < highestVersion && !item.NecessaryUpdate);
                _newUpdateConfigurations.Sort(
                    (x, y) => new UpdateVersion(x.LiteralVersion).CompareTo(new UpdateVersion(y.LiteralVersion)));
            }

            UpdatesFound = _newUpdateConfigurations.Count != 0;
        }

        /// <summary>
        ///     Returns all new configurations.
        /// </summary>
        public IEnumerable<UpdateConfiguration> NewestConfigurations => _newUpdateConfigurations;

        /// <summary>
        ///     Gets a value indicating whether updates were found, or not.
        /// </summary>
        public bool UpdatesFound { get; }
    }
}