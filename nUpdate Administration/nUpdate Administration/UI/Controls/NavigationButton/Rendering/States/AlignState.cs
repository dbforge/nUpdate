using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Align state
    /// </summary>
    public enum AlignState
        : int
    {
        /// <summary>
        /// The right state
        /// </summary>
        RightAlign = 1,
        /// <summary>
        /// The left state
        /// </summary>
        LeftAlign = 2,

        /// <summary>
        /// The top right state
        /// </summary>
        TopRightAlign = 3,
        /// <summary>
        /// The top left state
        /// </summary>
        TopLeftAlign = 4,

        /// <summary>
        /// The half bottom right state
        /// </summary>
        HalfBottomRightAlign = 5,
        /// <summary>
        /// The half bottom left state
        /// </summary>
        HalfBottomLeftAlign = 6,

        /// <summary>
        /// The half top right state
        /// </summary>
        HalfTopRightAlign = 7,
        /// <summary>
        /// The half top left state
        /// </summary>
        HalfTopLeftAlign = 8,
    }
}
