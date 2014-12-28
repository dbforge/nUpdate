using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Tool Bar Element state
    /// </summary>
    public enum ToolBarElementState
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
        /// The checked state
        /// </summary>
        Checked = 5,
        /// <summary>
        /// The hot/hover checked state
        /// </summary>
        HotChecked = 6,
        /// <summary>
        /// The near-hot state
        /// </summary>
        NearHot = 7,
        /// <summary>
        /// The other-side-hot state
        /// </summary>
        OtherSideHot = 8
    }
}
