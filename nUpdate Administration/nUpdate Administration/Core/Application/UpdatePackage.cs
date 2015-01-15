// Author: Dominic Beger (Trade/ProgTrade)

using System;
using nUpdate.Administration.Core.Update;

namespace nUpdate.Administration.Core.Application
{
    /// <summary>
    ///     Represents a local update package.
    /// </summary>
    [Serializable]
    public class UpdatePackage
    {
        /// <summary>
        ///     The version of the package.
        /// </summary>
        public UpdateVersion Version { get; set; }

        /// <summary>
        ///     The description of the package.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The option if the package is released.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        ///     The local path of the package.
        /// </summary>
        public string LocalPackagePath { get; set; }
    }
}