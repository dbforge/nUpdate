using System.Collections.Generic;
using System.Windows.Forms;

namespace nUpdate.Dialogs
{
    public class BaseForm : Form
    {
        public void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "BaseForm";
            this.ResumeLayout(false);

        }

        /// <summary>
        /// A list for controls that never should be locked
        /// </summary>
        private List<Control> protectedControls = new List<Control>();

        /// <summary>
        /// Disables or enables the given control
        /// </summary>
        /// <param name="control">The control to lock</param>
        /// <param name="enabled">A param to set if the control is locked</param>
        public void SetControlAccessiblity(Control control, bool enabled)
        {
            // Check if the control is protected of locking it
            if (!protectedControls.Contains(control))
            {
                control.Enabled = enabled;
            }

            // Check if the control has children and if so, we (un)lock them
            if (control.HasChildren)
            {
                foreach (Control ctrl in control.Controls)
                {
                    SetControlAccessiblity(ctrl, enabled);
                }
            }
        }

        /// <summary>
        /// Disables or enables all controls on the form
        /// </summary>
        /// <param name="enabled">A param to set if the controls are locked</param>
        public void SetAccessiblityForAllControls(bool enabled)
        {
            foreach (Control ctrl in Controls)
            {
                this.SetControlAccessiblity(ctrl, enabled);
            }
        }
    }
}
