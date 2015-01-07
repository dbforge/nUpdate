using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class FileRenameOperationPanel : UserControl, IOperationPanel
    {
        public FileRenameOperationPanel()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return pathTextBox.Text; }
            set { pathTextBox.Text = value; }
        }

        public string NewName
        {
            get { return newNameTextBox.Text; }
            set { newNameTextBox.Text = value; }
        }

        public Operation Operation
        {
            get { return new Operation(OperationArea.Files, OperationMethods.Rename, Path, NewName); }
        }

        private void FileRenameOperationPanel_Load(object sender, EventArgs e)
        {
            // Language initializing follows here
        }

        private void environmentVariablesButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Environment variables.",
                "%appdata%: AppData\n%temp%: Temp\n%program%: Program's directory\n%desktop%: Desktop directory",
                PopupButtons.Ok);
        }
    }
}