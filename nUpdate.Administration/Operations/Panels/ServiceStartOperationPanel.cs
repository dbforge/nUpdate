// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;
using nUpdate.Operations;

namespace nUpdate.Administration.Operations.Panels
{
    public partial class ServiceStartOperationPanel : UserControl, IOperationPanel
    {
        public ServiceStartOperationPanel()
        {
            InitializeComponent();
        }

        public string ServiceName
        {
            get { return serviceNameTextBox.Text; }
            set { serviceNameTextBox.Text = value; }
        }

        public string[] Arguments
        {
            get { return argumentTextBox.Text.Split(','); }
            set { argumentTextBox.Text = string.Join(",", value); }
        }

        public bool IsValid => !string.IsNullOrEmpty(serviceNameTextBox.Text);

        public Operation Operation => new Operation(OperationArea.Services, OperationMethod.Start, ServiceName, Arguments);
    }
}