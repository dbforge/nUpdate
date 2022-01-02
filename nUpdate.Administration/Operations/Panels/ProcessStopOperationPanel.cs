// ProcessStopOperationPanel.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;
using nUpdate.Operations;

namespace nUpdate.Administration.Operations.Panels
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