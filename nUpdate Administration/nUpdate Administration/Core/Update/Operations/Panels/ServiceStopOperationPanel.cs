using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class ServiceStopOperationPanel : UserControl, IOperationPanel
    {
        public ServiceStopOperationPanel()
        {
            InitializeComponent();
        }

        private void ServiceStopOperationPanel_Load(object sender, EventArgs e)
        {

        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Services, OperationMethods.Stop, serviceNameTextBox.Text);
            }
        }
    }
}
