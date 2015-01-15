// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class ProcessStartOperationPanel : UserControl, IOperationPanel
    {
        public ProcessStartOperationPanel()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return pathTextBox.Text; }
            set { pathTextBox.Text = value; }
        }

        public IEnumerable<string> Arguments
        {
            get { return argumentTextBox.Text.Split(','); }
            set { argumentTextBox.Text = String.Join(",", value); }
        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Processes, OperationMethod.Start, pathTextBox.Text,
                    Arguments.ToList());
            }
        }

        private void ProcessStartOperationPanel_Load(object sender, EventArgs e)
        {
        }

        private void environmentVariablesButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Environment variables.",
                "%appdata%: AppData\n%temp%: Temp\n%program%: Program's directory\n%desktop%: Desktop directory",
                PopupButtons.Ok);
        }
    }
}