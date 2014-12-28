using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Normal/Hot/Focused/Disabled state
    /// </summary>
    public enum NormalHotFocusedDisabledState
        : int
    {
        /// <summary>
        /// The normal state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The hot state
        /// </summary>
        Hot = 2,
        /// <summary>
        /// The focused state
        /// </summary>
        Focused = 3,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 4
    }
}
