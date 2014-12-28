using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Task Link state
    /// </summary>
    public enum TaskLinkState
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
        /// The disabled state
        /// </summary>
        Disabled = 4,
        /// <summary>
        /// The page state
        /// </summary>
        Page = 5
    }
}
