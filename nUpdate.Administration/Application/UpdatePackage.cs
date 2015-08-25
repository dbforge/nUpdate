// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Administration.Application
{
    /// <summary>
    ///     Represents a local update package.
    /// </summary>
    [Serializable]
    public class UpdatePackage
    {
        /// <summary>
        ///     Gets or sets the literal version of the package.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Gets or sets the description of the package.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the package is released, or not.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        ///     Gets or sets the root <see cref="PackageItem"/> of the package.
        /// </summary>
        public PackageItem RootItem { get; set; }
    }
}