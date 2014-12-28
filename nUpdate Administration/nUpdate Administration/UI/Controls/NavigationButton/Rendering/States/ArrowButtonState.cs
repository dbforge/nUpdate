using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Arrow Button state
    /// </summary>
    public enum ArrowButtonState
        : int
    {
        /// <summary>
        /// The up normal state
        /// </summary>
        UpNormal = 1,
        /// <summary>
        /// The up hot state
        /// </summary>
        UpHot = 2,
        /// <summary>
        /// The up pressed state
        /// </summary>
        UpPressed = 3,
        /// <summary>
        /// The up disabled state
        /// </summary>
        UpDisabled = 4,

        /// <summary>
        /// The down normal state
        /// </summary>
        DownNormal = 5,
        /// <summary>
        /// The down hot state
        /// </summary>
        DownHot = 6,
        /// <summary>
        /// The down pressed state
        /// </summary>
        DownPressed = 7,
        /// <summary>
        /// The down disabled state
        /// </summary>
        DownDisabled = 8,

        /// <summary>
        /// The left normal state
        /// </summary>
        LeftNormal = 9,
        /// <summary>
        /// The left hot state
        /// </summary>
        LeftHot = 10,
        /// <summary>
        /// The left pressed state
        /// </summary>
        LeftPressed = 11,
        /// <summary>
        /// The left disabled state
        /// </summary>
        LeftDisabled = 12,

        /// <summary>
        /// The right normal state
        /// </summary>
        RightNormal = 13,
        /// <summary>
        /// The right hot state
        /// </summary>
        RightHot = 14,
        /// <summary>
        /// The right pressed state
        /// </summary>
        RightPressed = 15,
        /// <summary>
        /// The right disabled state
        /// </summary>
        RightDisabled = 16,
    }
}
