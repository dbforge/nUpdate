using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStyleControls.Controls.Helpers
{
    /// <summary>
    /// A generic DesignerActionList.
    /// </summary>
    public abstract class DesignerActionListBase<T>
        : DesignerActionList
        where T : class, IComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VisualStyleControls.Controls.Helpers.DesignerActionListBase"/> class.
        /// </summary>
        /// <param name="component">A component related to this <see cref="T:VisualStyleControls.Controls.DesignerActionListBase"/>.</param>
        public DesignerActionListBase(IComponent component)
            : base(component)
        {
            this.TargetComponent = component as T;
            this.DesignerActionUIService = this.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }

        /// <summary>
        /// Returns the <see cref="T:System.ComponentModel.IComponent"/> this <see cref="T:VisualStyleControls.Controls.Helpers.DesignerActionListBase"/> is targeted at.
        /// </summary>
        /// <value>
        /// The target component.
        /// </value>
        protected T TargetComponent { get; private set; }

        /// <summary>
        /// Returns the <see cref="T:System.ComponentModel.Design.DesignerActionUIService"/>.
        /// </summary>
        /// <value>
        /// The designer action UI service.
        /// </value>
        protected DesignerActionUIService DesignerActionUIService { get; private set; }

        /// <summary>
        /// Returns a list of <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> representing the DesignerActionList items.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.DesignerActionItem"/>-Array.
        /// </returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            throw new NotImplementedException();
        }
    }
}
