// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;
using nUpdate.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class ProcessStopOperationPanel : UserControl, IOperationPanel
    {
        public ProcessStopOperationPanel()
        {
            InitializeComponent();
        }

        public bool IsValid
        {
            get { return !String.IsNullOrEmpty(processNameTextBox.Text); }
        }

        public string ProcessName
        {
            get { return processNameTextBox.Text; }
            set { processNameTextBox.Text = value; }
        }

        public Operation Operation
        {
            get { return new Operation(OperationArea.Processes, OperationMethod.Stop, ProcessName); }
        }

        private void environmentVariablesButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Environment variables.",
                "%appdata%: AppData\n%temp%: Temp\n%program%: Program's directory\n%desktop%: Desktop directory",
                PopupButtons.Ok);
        }
    }
}