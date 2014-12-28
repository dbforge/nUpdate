using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Item state
    /// </summary>
    public enum ItemState
        : int
    {
        /// <summary>
        /// The hot state
        /// </summary>
        Hot = 1,
        /// <summary>
        /// The hot selected state
        /// </summary>
        HotSelected = 2,
        /// <summary>
        /// The selected state
        /// </summary>
        Selected = 3,
        /// <summary>
        /// The selected without focus state
        /// </summary>
        SelectedNotFocus = 4
    }
}
