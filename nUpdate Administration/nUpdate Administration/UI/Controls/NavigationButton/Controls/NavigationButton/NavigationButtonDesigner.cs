using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace VisualStyleControls.Controls
{
    public class NavigationButtonDesigner
        : ControlDesigner
    {
        private DesignerActionListCollection actionLists;
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                return this.actionLists != null ?
                    this.actionLists :
                    this.actionLists = new DesignerActionListCollection(new DesignerActionList[] { new NavigationButtonDesignerActionList(this.Component) });
            }
        }

        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.Moveable;
            }
        }
    }
}
