using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Normal/Hot/Selected/Disabled/Focused state
    /// </summary>
    public enum NormalHotSelectedDisabledFocusedState
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
        /// The selected state
        /// </summary>
        Selected = 3,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 4,
        /// <summary>
        /// The focused state
        /// </summary>
        Focused = 5
    }
}