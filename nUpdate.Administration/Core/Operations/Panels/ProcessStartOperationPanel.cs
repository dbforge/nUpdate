// Copyright © Dominic Beger 2018

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class ProcessStartOperationPanel : UserControl, IOperationPanel
    {
        public ProcessStartOperationPanel()
        {
            InitializeComponent();
        }

        public string Arguments
        {
            get => argumentTextBox.Text;
            set => argumentTextBox.Text = value;
        }

        public string Path
        {
            get => pathTextBox.Text;
            set => pathTextBox.Text = value;
        }

        public bool IsValid
            =>
                !string.IsNullOrEmpty(pathTextBox.Text) && pathTextBox.Text.Contains("\\") &&
                pathTextBox.Text.Split(new[] {"\\"}, StringSplitOptions.RemoveEmptyEntries).Length >= 2;

        public Operation Operation => new Operation(OperationArea.Processes, OperationMethod.Start, pathTextBox.Text,
            Arguments);

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