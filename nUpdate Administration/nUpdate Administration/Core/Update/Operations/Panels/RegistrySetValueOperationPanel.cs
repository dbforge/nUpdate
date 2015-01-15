// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class RegistrySetValueOperationPanel : UserControl, IOperationPanel
    {
        public RegistrySetValueOperationPanel()
        {
            InitializeComponent();
        }

        public string KeyPath
        {
            get
            {
                return String.Format("{0}\\{1}", mainKeyComboBox.GetItemText(mainKeyComboBox.SelectedIndex),
                    subKeyTextBox.Text);
            }
            set
            {
                var pathParts = value.Split('\\');
                foreach (var pathPart in pathParts)
                {
                    if (pathPart == pathParts[0])
                    {
                        mainKeyComboBox.SelectedValue = pathParts[0];
                    }
                    else
                    {
                        subKeyTextBox.Text += String.Format("\\{0}", pathPart);
                    }
                }
            }
        }

        public List<Tuple<string, object, RegistryValueKind>> NameValuePairs
        {
            get
            {
                return (from ListViewItem listViewItem in nameValuePairListView.Items
                    select
                        new Tuple<string, object, RegistryValueKind>(listViewItem.SubItems[0].Text,
                            listViewItem.SubItems[1].Text,
                            (RegistryValueKind) Enum.Parse(typeof (RegistryValueKind), listViewItem.SubItems[2].Text)))
                    .ToList();
            }
            set
            {
                foreach (var tupleItem in value)
                {
                    var item = new ListViewItem(tupleItem.Item1);
                    item.SubItems.Add(tupleItem.Item2.ToString());
                    item.SubItems.Add(tupleItem.Item3.ToString());
                }
            }
        }

        public Operation Operation
        {
            get { return new Operation(OperationArea.Registry, OperationMethod.SetValue, KeyPath, NameValuePairs); }
        }

        private void RegistryEntrySetValueOperationPanel_Load(object sender, EventArgs e)
        {
            mainKeyComboBox.SelectedIndex = 0;
            valueKindComboBox.DataSource = Enum.GetValues(typeof (RegistryValueKind));
            valueKindComboBox.SelectedIndex = 0;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(nameTextBox.Text) || String.IsNullOrEmpty(valueTextBox.Text))
                return;
            var item = new ListViewItem(nameTextBox.Text);
            item.SubItems.Add(valueTextBox.Text);
            item.SubItems.Add(valueKindComboBox.GetItemText(valueKindComboBox.SelectedItem));
            nameValuePairListView.Items.Add(item);
            nameTextBox.Clear();
            valueTextBox.Clear();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in nameValuePairListView.SelectedItems)
            {
                nameValuePairListView.Items.Remove(item);
            }
        }
    }
}