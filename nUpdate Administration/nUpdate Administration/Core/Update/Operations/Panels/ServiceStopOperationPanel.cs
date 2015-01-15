// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Panels
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

        public Operation Operation
        {
            get { return new Operation(OperationArea.Services, OperationMethod.Stop, ServiceName); }
        }

        private void ServiceStopOperationPanel_Load(object sender, EventArgs e)
        {
        }
    }
}