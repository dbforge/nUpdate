using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Normal/SoftHot/Hot state
    /// </summary>
    public enum NormalSoftHotHotState
        : int
    {
        /// <summary>
        /// The normal state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The soft hot state
        /// </summary>
        SoftHot = 2,
        /// <summary>
        /// The hot state
        /// </summary>
        Hot = 3
    }
}
