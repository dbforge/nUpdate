// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.Application;

namespace nUpdate.Administration.UI.Dialogs
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
            Name = "BaseDialog";
            BackColor = SystemColors.Window;
            Font = new Font("Segoe UI", 8);
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
    }
}