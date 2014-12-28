using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Header Item state
    /// </summary>
    public enum HeaderItemState
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
        /// The normal sorted state
        /// </summary>
        SortedNormal = 4,
        /// <summary>
        /// The hot sorted state
        /// </summary>
        SortedHot = 5,
        /// <summary>
        /// The pressed sorted state
        /// </summary>
        SortedPressed = 6,

        /// <summary>
        /// The icon normal state
        /// </summary>
        IconNormal = 7,
        /// <summary>
        /// The icon hot state
        /// </summary>
        IconHot = 8,
        /// <summary>
        /// The icon pressed state
        /// </summary>
        IconPressed = 9,

        /// <summary>
        /// The icon normal sorted state
        /// </summary>
        IconSortedNormal = 10,
        /// <summary>
        /// The icon hot sorted state
        /// </summary>
        IconSortedHot = 11,
        /// <summary>
        /// The icon pressed sorted state
        /// </summary>
        IconSortedPressed = 12,
    }
}
