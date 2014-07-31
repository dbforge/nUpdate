using nUpdate.Administration.Core.Update;
using System;

namespace nUpdate.Administration.Core.Update
{
    [Serializable()]
    internal class UpdatePackage
    {
        /// <summary>
        /// The version of the package.
        /// </summary>
        public UpdateVersion Version { get; set; }

        /// <summary>
        /// The description of the package.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The option if the package is released.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        /// The local path of the package.
        /// </summary>
        public string LocalPackagePath { get; set; }
    }
}
