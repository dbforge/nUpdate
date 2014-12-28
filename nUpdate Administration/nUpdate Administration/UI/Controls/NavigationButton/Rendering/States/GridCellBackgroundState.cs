using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Grid Cell Background state
    /// </summary>
    public enum GridCellBackgroundState
        : int
    {
        /// <summary>
        /// The selected state
        /// </summary>
        Selected = 1,
        /// <summary>
        /// The hot/hover state
        /// </summary>
        Hot = 2,
        /// <summary>
        /// The selected hot/hover state
        /// </summary>
        SelectedHot = 3,
        /// <summary>
        /// The not selected focused state
        /// </summary>
        SelectedNotFocused = 4,
        /// <summary>
        /// The today state
        /// </summary>
        Today = 5
    }
}
