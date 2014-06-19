using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Administration.Core.Application
{
    [Serializable()]
    internal class UpdatePackage
    {
        /// <summary>
        /// The version of the package.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The description of the package.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The option if the package is released.
        /// </summary>
        public string IsReleased { get; set; }

        /// <summary>
        /// The local path of the package if it is not released yet.
        /// </summary>
        public string LocalPackagePath { get; set; }
    }
}
