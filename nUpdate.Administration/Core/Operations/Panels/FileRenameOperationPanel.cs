// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
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

        public bool IsValid
        {
            get
            {
                return !Controls.OfType<CueTextBox>().Any(item => string.IsNullOrEmpty(item.Text)) &&
                       Path.Contains("\\") &&
                       Path.Split(new[] {"\\"}, StringSplitOptions.RemoveEmptyEntries).Length >= 2;
            }
        }

        public Operation Operation => new Operation(OperationArea.Files, OperationMethod.Rename, Path, NewName);

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

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!pathTextBox.Text.Contains("/"))
                return;
            pathTextBox.Text = pathTextBox.Text.Replace('/', '\\');
            pathTextBox.SelectionStart = pathTextBox.Text.Length;
            pathTextBox.SelectionLength = 0;
        }
    }
}