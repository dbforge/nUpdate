using System;
using System.Collections.Generic;

namespace nUpdate.Administration.Core
{
    [Serializable]
    public class HelpContent
    {
        /// <summary>
        ///     Gets or sets the help entries included in the whole content.
        /// </summary>
        public IEnumerable<HelpEntry> HelpEntries { get; set; } 
    }
}
