using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Border state
    /// </summary>
    public enum BorderState
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
        /// The focused state
        /// </summary>
        Focused = 3,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 4
    }
}
