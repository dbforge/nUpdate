using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a Balloon Stem state
    /// </summary>
    public enum BalloonStemState
        : int
    {
        /// <summary>
        /// The pointing up left state
        /// </summary>
        PointingUpLeftWall = 1,
        /// <summary>
        /// The pointing up center state
        /// </summary>
        PointingUpCentered = 2,
        /// <summary>
        /// The pointing up right state
        /// </summary>
        PointingUpRightWall = 3,

        /// <summary>
        /// The pointing down right state
        /// </summary>
        PointingDownRightWall = 4,
        /// <summary>
        /// The pointing down center state
        /// </summary>
        PointingDownCentered = 5,
        /// <summary>
        /// The pointing down left state
        /// </summary>
        PointingDownLeftWall = 6,
    }
}
