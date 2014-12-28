using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Normal/Hot/Pressed/Focused/Disabled state
    /// </summary>
    public enum NormalHotPressedFocusedDisabledState
        : int
    {
        /// <summary>
        /// The normal state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The hot/hover state
        /// </summary>
        Hot = 2,
        /// <summary>
        /// The pressed state
        /// </summary>
        Pressed = 3,
        /// <summary>
        /// The focused state
        /// </summary>
        Focused = 4,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 5
    }
}
