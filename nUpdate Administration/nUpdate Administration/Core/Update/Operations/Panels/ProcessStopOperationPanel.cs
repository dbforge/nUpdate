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
    public partial class ProcessStopOperationPanel : UserControl, IOperationPanel
    {
        public ProcessStopOperationPanel()
        {
            InitializeComponent();
        }

        public string ProcessName
        {
            get { return processNameTextBox.Text; }
            set { processNameTextBox.Text = value; }
        }

        private void ProcessStopOperationPanel_Load(object sender, EventArgs e)
        {

        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Processes, OperationMethods.Stop, ProcessName);
            }
        }
    }
}
