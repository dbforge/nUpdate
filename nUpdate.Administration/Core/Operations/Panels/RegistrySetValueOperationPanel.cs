// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
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
                return $"{mainKeyComboBox.GetItemText(mainKeyComboBox.SelectedItem)}\\{subKeyTextBox.Text}";
            }
            set
            {
                var pathParts = value.Split('\\');
                mainKeyComboBox.SelectedValue = pathParts[0];
                subKeyTextBox.Text = string.Join("\\", pathParts.Skip(1));
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
                    nameValuePairListView.Items.Add(item);
                }
            }
        }

        public bool IsValid => !string.IsNullOrEmpty(subKeyTextBox.Text) && nameValuePairListView.Items.Count > 0;

        public Operation Operation
            => new Operation(OperationArea.Registry, OperationMethod.SetValue, KeyPath, NameValuePairs);

        private void RegistryEntrySetValueOperationPanel_Load(object sender, EventArgs e)
        {
            mainKeyComboBox.SelectedIndex = 0;
            valueKindComboBox.DataSource = Enum.GetValues(typeof (RegistryValueKind));
            valueKindComboBox.SelectedIndex = 0;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text) || string.IsNullOrEmpty(valueTextBox.Text))
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

        private void subKeyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!subKeyTextBox.Text.Contains("/"))
                return;
            subKeyTextBox.Text = subKeyTextBox.Text.Replace('/', '\\');
            subKeyTextBox.SelectionStart = subKeyTextBox.Text.Length;
            subKeyTextBox.SelectionLength = 0;
        }
    }
}