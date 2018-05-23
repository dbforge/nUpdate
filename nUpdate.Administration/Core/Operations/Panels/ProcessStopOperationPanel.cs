// Copyright © Dominic Beger 2018

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class ProcessStopOperationPanel : UserControl, IOperationPanel
    {
        public ProcessStopOperationPanel()
        {
            InitializeComponent();
        }

        public string ProcessName
        {
            get => processNameTextBox.Text;
            set => processNameTextBox.Text = value;
        }

        public bool IsValid => !string.IsNullOrEmpty(processNameTextBox.Text);
        public Operation Operation => new Operation(OperationArea.Processes, OperationMethod.Stop, ProcessName);

        private void environmentVariablesButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Environment variables.",
                "%appdata%: AppData\n%temp%: Temp\n%program%: Program's directory\n%desktop%: Desktop directory",
                PopupButtons.Ok);
        }
    }
}