// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Windows.Forms;
using nUpdate.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
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

        public Operation Operation
            => new Operation(OperationArea.Services, OperationMethod.Start, ServiceName, Arguments);
    }
}