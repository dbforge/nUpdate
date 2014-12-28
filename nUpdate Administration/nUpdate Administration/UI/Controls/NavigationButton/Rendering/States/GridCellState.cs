using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Grid Cell state
    /// </summary>
    public enum GridCellState
        : int
    {
        /// <summary>
        /// The hot/hover state
        /// </summary>
        Hot = 1,
        /// <summary>
        /// The "has state" state
        /// </summary>
        HasState = 2,
        /// <summary>
        /// The hot "has state" state
        /// </summary>
        HasStateHot = 3,
        /// <summary>
        /// The today state
        /// </summary>
        Today = 4
    }
}
