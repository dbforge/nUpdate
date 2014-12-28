using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Expand Button state
    /// </summary>
    public enum ExpandButtonState
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
        /// The expanded normal state
        /// </summary>
        ExpandedNormal = 4,
        /// <summary>
        /// The expanded hot/hover state
        /// </summary>
        ExpandedHot = 5,
        /// <summary>
        /// The expanded pressed state
        /// </summary>
        ExpandedPressed = 6
    }
}
