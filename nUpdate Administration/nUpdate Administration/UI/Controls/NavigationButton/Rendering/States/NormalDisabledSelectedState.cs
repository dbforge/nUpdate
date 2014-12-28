using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Normal/Disabled/Selected state
    /// </summary>
    public enum NormalDisabledSelectedState
        : int
    {
        /// <summary>
        /// The normal state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 2,
        /// <summary>
        /// The selected state
        /// </summary>
        Selected = 2,
    }
}
