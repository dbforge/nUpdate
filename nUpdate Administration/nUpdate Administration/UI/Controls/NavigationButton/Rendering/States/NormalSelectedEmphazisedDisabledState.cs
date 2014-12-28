using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Normal/Selected/Emphasized/Disabled state
    /// </summary>
    public enum NormalSelectedEmphasizedDisabledState
        : int
    {
        /// <summary>
        /// The normal state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The selected state
        /// </summary>
        Selected = 2,
        /// <summary>
        /// The emphasized state
        /// </summary>
        Emphasized = 3,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 4,
    }
}
