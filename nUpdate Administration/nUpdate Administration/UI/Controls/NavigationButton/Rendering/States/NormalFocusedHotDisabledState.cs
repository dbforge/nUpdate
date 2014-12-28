using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Normal/Focused/Hot/Disabled state
    /// </summary>
    public enum NormalFocusedHotDisabledState
        : int
    {
        /// <summary>
        /// The normal state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The focused state
        /// </summary>
        Focused = 2,
        /// <summary>
        /// The hot state
        /// </summary>
        Hot = 3,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 4
    }
}
