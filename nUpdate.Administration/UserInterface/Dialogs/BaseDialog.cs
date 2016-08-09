// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal class BaseDialog : Form
    {
        internal bool AllowCancel { get; set; } = true;

        internal Panel LoadingPanel { get; set; }

        internal BaseDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // BaseDialog
            // 
            BackColor = SystemColors.Window;
            ClientSize = new Size(284, 261);
            Font = new Font("Segoe UI", 8F);
            Name = "BaseDialog";
            FormClosing += BaseDialog_FormClosing;
            ResumeLayout(false);
        }

        internal void AdjustControlsForAction(Action action, bool showLoadingOverlay)
        {
            DisableControls(showLoadingOverlay);
            action();
            EnableControls();
        }

        private void SetControls(bool enabled)
        {
            Invoke(new Action(() =>
            {
                foreach (var c in from Control c in Controls where c.Visible select c)
                    c.Enabled = enabled;
                AllowCancel = enabled;
            }));
        }

        internal void EnableControls()
        {
            SetControls(true);
            Invoke(new Action(() =>
            {
                if (LoadingPanel == null)
                    return;
                LoadingPanel.Visible = false;
            }));
        }

        internal void DisableControls(bool showLoadingOverlay)
        {
            SetControls(false);
            Invoke(new Action(() =>
            {
                if (LoadingPanel == null || !showLoadingOverlay)
                    return;

                LoadingPanel.Visible = true;
                LoadingPanel.Location = new Point((Width - LoadingPanel.Width) / 2,
                    (Height - LoadingPanel.Height) / 2);
                LoadingPanel.BringToFront();
            }));
        }

        private void BaseDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowCancel)
                e.Cancel = true;
        }
    }
}