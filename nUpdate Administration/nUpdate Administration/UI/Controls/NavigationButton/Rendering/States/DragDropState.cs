using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Drag Drop state
    /// </summary>
    public enum DragDropState
        : int
    {
        /// <summary>
        /// The highlight state
        /// </summary>
        Highlight = 1,
        /// <summary>
        /// The non-highlight state
        /// </summary>
        NonHighlight = 2,
    }
}
