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
            bool isAlphaWished, bool isBetaWished, List<KeyValuePair<string, string>> conditions = null)
        {
            if (packageConfigurations != null)
            {
                var is64Bit = Environment.Is64BitOperatingSystem;
                foreach (
                    var config in
                    packageConfigurations.Where(
                            item => new UpdateVersion(item.LiteralVersion) > currentVersion)
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

                    if (config.RolloutConditions != null && config.RolloutConditions.Any())
                    {
                        if (conditions != null && conditions.Any())
                        {
                            // If any negative condition is met, this update does not interest us.
                            if (conditions.Any(x => config.RolloutConditions.Where(n => n.IsNegativeCondition)
                                .Any(c =>
                                    c.Key == x.Key &&
                                    string.Equals(c.Value, x.Value,
                                        StringComparison.CurrentCultureIgnoreCase))))
                                continue;

                            switch (config.RolloutConditionMode)
                            {
                                // If no positive condition is met, this update does not interest us.
                                case RolloutConditionMode.AtLeastOne:
                                    if (conditions.All(x => !config.RolloutConditions
                                        .Where(n => !n.IsNegativeCondition)
                                        .Any(c =>
                                            c.Key == x.Key &&
                                            string.Equals(c.Value, x.Value,
                                                StringComparison.CurrentCultureIgnoreCase))))
                                        continue;
                                    break;

                                // If not all positive conditions are met, this update does not interest us.
                                case RolloutConditionMode.All:
                                    if (conditions.Any(x => !config.RolloutConditions
                                        .Where(n => !n.IsNegativeCondition)
                                        .All(c =>
                                            c.Key == x.Key &&
                                            string.Equals(c.Value, x.Value,
                                                StringComparison.CurrentCultureIgnoreCase))))
                                        continue;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(packageConfigurations), "Invalid rollout condition mode.");
                            }
                        }
                        else
                        {
                            // This should be discussed again as it may be senseful that a user with no local key for a positive condition may eventually also receive the update.
                            if (config.RolloutConditions.Any(c => !c.IsNegativeCondition))
                                continue;
                        }

                    }

                    _newUpdateConfigurations.Add(config);
                }
            }

            var highestVersion =
                UpdateVersion.GetHighestUpdateVersion(
                    _newUpdateConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)));
            _newUpdateConfigurations.RemoveAll(
                item => new UpdateVersion(item.LiteralVersion) < highestVersion && !item.NecessaryUpdate);
            _newUpdateConfigurations.Sort(
                (x, y) => new UpdateVersion(x.LiteralVersion).CompareTo(new UpdateVersion(y.LiteralVersion)));
            
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