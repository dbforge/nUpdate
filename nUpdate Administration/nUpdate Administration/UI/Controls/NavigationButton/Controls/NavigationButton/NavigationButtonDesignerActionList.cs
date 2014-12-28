using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStyleControls.Controls.Helpers;

namespace VisualStyleControls.Controls
{
    /// <summary>
    /// Provides a DesignerActionList for the <see cref="T:VisualStyleControls.Controls.NavigationButton"/> Control.
    /// </summary>
    public class NavigationButtonDesignerActionList
        : DesignerActionListBase<NavigationButton>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationButtonDesignerActionList"/> class.
        /// </summary>
        /// <param name="component">The component.</param>
        public NavigationButtonDesignerActionList(IComponent component)
            : base(component)
        { }

        /// <summary>
        /// Returns a list of <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> representing the DesignerActionList items.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.DesignerActionItem"/>-Array.
        /// </returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection aItems = new DesignerActionItemCollection();
            aItems.Add(new DesignerActionPropertyItem("ButtonType", "Type", "Appearance", "Indicates the Type of this Button."));
            return aItems;
        }

        /// <summary>
        /// Indicates the Type of the targeted <see cref="T:VisualStyleControls.Controls.NavigationButton"/>.
        /// </summary>
        /// <value>
        /// The current Type.
        /// </value>
        public NavigationButtonType ButtonType
        {
            get
            {
                return this.TargetComponent.ButtonType;
            }
            set
            {
                this.TargetComponent.ButtonType = value;
                this.DesignerActionUIService.Refresh(this.Component);
            }
        }
    }
}
