// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class ServiceStartOperationPanel : UserControl, IOperationPanel
    {
        public ServiceStartOperationPanel()
        {
            InitializeComponent();
        }

        public bool IsValid
        {
            get { return !String.IsNullOrEmpty(serviceNameTextBox.Text) ; }
        }

        public string ServiceName
        {
            get { return serviceNameTextBox.Text; }
            set { serviceNameTextBox.Text = value; }
        }

        public string[] Arguments
        {
            get { return argumentTextBox.Text.Split(','); }
            set { argumentTextBox.Text = String.Join(",", value); }
        }

        public Operation Operation
        {
            get { return new Operation(OperationArea.Services, OperationMethod.Start, ServiceName, Arguments); }
        }
    }
}