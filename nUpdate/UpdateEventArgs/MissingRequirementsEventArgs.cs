using System;
using System.Collections.Generic;
using nUpdate.Updating;

namespace nUpdate.UpdateEventArgs
{
    /// <summary>
    ///     Provides data for missing requirements.
    /// </summary>
    public class MissingRequirementsEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MissingRequirementsEventArgs" />-class.
        /// </summary>
        /// <param name="requirements">The requirements, that are missing</param>
        internal MissingRequirementsEventArgs(Dictionary<UpdateVersion, List<UpdateRequirement>> requirements)
        {
             Requirements = requirements;
        }

        /// <summary>
        ///     Gets or sets the requirements, that are missing.
        /// </summary>
        public Dictionary<UpdateVersion, List<UpdateRequirement>> Requirements;
    }
}