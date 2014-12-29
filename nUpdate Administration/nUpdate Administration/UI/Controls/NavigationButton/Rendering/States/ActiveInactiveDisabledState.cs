﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents an Active/Inactive/Disabled state
    /// </summary>
    public enum ActiveInactiveDisabledState
        : int
    {
        /// <summary>
        /// The active state
        /// </summary>
        Active = 1,
        /// <summary>
        /// The inactive state
        /// </summary>
        Inactive = 2,
        /// <summary>
        /// The disabled state
        /// </summary>
        Disabled = 2
    }
}