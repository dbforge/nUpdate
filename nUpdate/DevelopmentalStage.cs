// Copyright © Dominic Beger 2017

using System.ComponentModel;

namespace nUpdate
{
    /// <summary>
    ///     Represents the different developmental stages of a version.
    /// </summary>
    public enum DevelopmentalStage
    {
        /// <summary>
        ///     Represents a Release version.
        /// </summary>
        Release,

        /// <summary>
        ///     Represents a ReleaseCandidate version.
        /// </summary>
        [Description("rc")] ReleaseCandidate,

        /// <summary>
        ///     Represents a Beta version.
        /// </summary>
        [Description("b")] Beta,

        /// <summary>
        ///     Represents an Alpha version.
        /// </summary>
        [Description("a")] Alpha
    }
}