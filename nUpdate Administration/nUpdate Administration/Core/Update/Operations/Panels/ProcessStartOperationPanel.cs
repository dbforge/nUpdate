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
            get { return argumentTextBox.Text.Split(new char[] {','}); }
            set { argumentTextBox.Text = String.Join(",", value); }
        }

        private void ProcessStartOperationPanel_Load(object sender, EventArgs e)
        {

        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Processes, OperationMethods.Start, pathTextBox.Text, Arguments.ToList());
            }
        }
    }
}
