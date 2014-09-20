using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class RegistryEntrySetValueOperationPanel : UserControl, IOperationPanel
    {
        public RegistryEntrySetValueOperationPanel()
        {
            InitializeComponent();
        }

        private void RegistryEntrySetValueOperationPanel_Load(object sender, EventArgs e)
        {
            dataTypeComboBox.DataSource = Enum.GetValues(typeof (RegistryValueKind));
            dataTypeComboBox.SelectedIndex = 0;
        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Registry, OperationMethods.SetValue, appKeyTextBox.Text, new Tuple<string, string>(dataTypeComboBox.GetItemText(dataTypeComboBox.SelectedItem), valueTextBox.Text));
            }
        }
    }
}
