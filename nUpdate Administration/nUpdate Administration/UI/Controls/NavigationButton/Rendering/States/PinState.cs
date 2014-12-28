using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Pin state
    /// </summary>
    public enum PinState
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
        /// The pressed state
        /// </summary>
        Pressed = 3,
        
        /// <summary>
        /// The selected normal state
        /// </summary>
        SelectedNormal = 4,
        /// <summary>
        /// The selected hot state
        /// </summary>
        SelectedHot = 5,
        /// <summary>
        /// The selected pressed state
        /// </summary>
        SelectedPressed = 6
    }
}
