using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Progress Bar state
    /// </summary>
    public enum ProgressBarState
        : int
    {
        /// <summary>
        /// The normal state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The error state
        /// </summary>
        Error = 2,
        /// <summary>
        /// The paused state
        /// </summary>
        Paused = 3,
        /// <summary>
        /// The partial state
        /// </summary>
        Partial = 4
    }
}
