// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Windows.Forms;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class ServiceStopOperationPanel : UserControl, IOperationPanel
    {
        public ServiceStopOperationPanel()
        {
            InitializeComponent();
        }

        public string ServiceName
        {
            get { return serviceNameTextBox.Text; }
            set { serviceNameTextBox.Text = value; }
        }

        public bool IsValid => !string.IsNullOrEmpty(serviceNameTextBox.Text);
        public Operation Operation => new Operation(OperationArea.Services, OperationMethod.Stop, ServiceName);
    }
}