using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Text Edit state
    /// </summary>
    public enum TextEditState
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
        Focused = 5,
        /// <summary>
        /// The read-only state
        /// </summary>
        ReadOnly = 6,
        /// <summary>
        /// The assist state
        /// </summary>
        Assist = 7
    }
}
