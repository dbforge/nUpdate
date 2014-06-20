using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations
{
    public partial class FileDeleteOperationDialog : BaseForm
    {
        public FileDeleteOperationDialog()
        {
            InitializeComponent();
        }

        private void FileDeleteOperationDialog_Load(object sender, EventArgs e)
        {

        }

        private void environmentLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Environment variables", String.Format("AppData: %appdata%{0}Temp: %temp%{0}Desktop: %desktop%{0}", Environment.NewLine), PopupButtons.OK);
        }
    }
}
