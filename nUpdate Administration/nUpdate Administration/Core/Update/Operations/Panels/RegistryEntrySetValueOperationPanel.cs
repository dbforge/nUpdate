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

        public string KeyPath
        {
            get
            {
                return String.Format("{0}\\{1}", mainKeyComboBox.GetItemText(mainKeyComboBox.SelectedIndex),
                    appKeyTextBox.Text);
            }
            set
            {
                string[] pathParts = value.Split(new char[] { '\\' });
                foreach (string pathPart in pathParts)
                {
                    if (pathPart == pathParts[0])
                    {
                        mainKeyComboBox.SelectedValue = pathParts[0];
                    }
                    else
                    {
                        appKeyTextBox.Text += String.Format("\\{0}", pathPart);
                    }
                }
            }
        }

        public Tuple<string, string> Value
        {
            get
            {
                return new Tuple<string, string>(dataTypeComboBox.GetItemText(dataTypeComboBox.SelectedItem), valueTextBox.Text);
            }
            set
            {
                dataTypeComboBox.SelectedValue = value.Item1;
                valueTextBox.Text = value.Item2;
            }
        }

        private void RegistryEntrySetValueOperationPanel_Load(object sender, EventArgs e)
        {
            mainKeyComboBox.SelectedIndex = 0;
            dataTypeComboBox.DataSource = Enum.GetValues(typeof (RegistryValueKind));
            dataTypeComboBox.SelectedIndex = 0;
        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Registry, OperationMethods.SetValue, KeyPath, Value);
            }
        }
    }
}
