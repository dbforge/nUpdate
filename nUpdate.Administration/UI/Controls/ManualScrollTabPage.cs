using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    internal class ManualScrollTabPage : TabPage
    {
        protected override Point ScrollToControl(Control activeControl)
        {
            return AutoScrollPosition;
        }
    }
}