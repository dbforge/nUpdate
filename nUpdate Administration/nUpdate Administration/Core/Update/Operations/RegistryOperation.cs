using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Administration.Core.Update.Operations
{
    internal class RegistryOperation
    {
        /// <summary>
        /// Returns the whole registry path to the given key.
        /// </summary>
        public string KeyPath { get; set; }

        /// <summary>
        /// Returns the operation to execute.
        /// </summary>
        public RegistryOperations Operation { get; set; }

        /// <summary>
        /// Returns the value to set if the operation is "Edit".
        /// </summary>
        public string Value { get; set; }
    }
}
