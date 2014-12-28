using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Represents a CheckBox state
    /// </summary>
    public enum CheckBoxState
        : int
    {
        /// <summary>
        /// The unchecked normal state
        /// </summary>
        UncheckedNormal = 1,
        /// <summary>
        /// The unchecked hot/hover state
        /// </summary>
        UncheckedHot = 2,
        /// <summary>
        /// The unchecked pressed state
        /// </summary>
        UncheckedPressed = 3,
        /// <summary>
        /// The unchecked disabled state
        /// </summary>
        UncheckedDisabled = 4,

        /// <summary>
        /// The checked normal state
        /// </summary>
        CheckedNormal = 5,
        /// <summary>
        /// The checked hot/hover state
        /// </summary>
        CheckedHot = 6,
        /// <summary>
        /// The checked pressed state
        /// </summary>
        CheckedPressed = 7,
        /// <summary>
        /// The checked disabled state
        /// </summary>
        CheckedDisabled = 8,

        /// <summary>
        /// The mixed normal state
        /// </summary>
        MixedNormal = 9,
        /// <summary>
        /// The mixed hot/hover state
        /// </summary>
        MixedHot = 10,
        /// <summary>
        /// The mixed pressed state
        /// </summary>
        MixedPressed = 11,
        /// <summary>
        /// The mixed disabled state
        /// </summary>
        MixedDisabled = 12,

        /// <summary>
        /// The implicit normal state
        /// </summary>
        ImplicitNormal = 13,
        /// <summary>
        /// The implicit hot/hover state
        /// </summary>
        ImplicitHot = 14,
        /// <summary>
        /// The implicit pressed state
        /// </summary>
        ImplicitPressed = 15,
        /// <summary>
        /// The implicit disabled state
        /// </summary>
        ImplicitDisabled = 16,

        /// <summary>
        /// The excluded normal state
        /// </summary>
        ExcludedNormal = 17,
        /// <summary>
        /// The excluded hot/hover state
        /// </summary>
        ExcludedHot = 18,
        /// <summary>
        /// The excluded pressed state
        /// </summary>
        ExcludedPressed = 19,
        /// <summary>
        /// The excluded disabled state
        /// </summary>
        ExcludedDisabled = 20,
    }
}
