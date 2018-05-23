// Copyright © Dominic Beger 2018

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class RegistryDeleteValueOperationPanel : UserControl, IOperationPanel
    {
        public RegistryDeleteValueOperationPanel()
        {
            InitializeComponent();
        }

        public BindingList<string> ItemList { get; set; } = new BindingList<string>();

        public string KeyPath
        {
            get => $"{mainKeyComboBox.GetItemText(mainKeyComboBox.SelectedItem)}\\{subKeyTextBox.Text}";
            set
            {
                var pathParts = value.Split('\\');
                mainKeyComboBox.SelectedValue = pathParts[0];
                subKeyTextBox.Text = string.Join("\\", pathParts.Skip(1));
            }
        }

        public bool IsValid => !string.IsNullOrEmpty(subKeyTextBox.Text) && ItemList.Any();

        public Operation Operation
            => new Operation(OperationArea.Registry, OperationMethod.DeleteValue, KeyPath, ItemList.ToList());

        private void addButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(valueNameTextBox.Text))
                return;
            ItemList.Add(valueNameTextBox.Text);
            valueNameTextBox.Clear();
        }

        private void InputChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox) sender;
            if (!textBox.Text.Contains("/"))
                return;
            textBox.Text = textBox.Text.Replace('/', '\\');
            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
        }

        private void RegistryEntryDeleteValueOperationPanel_Load(object sender, EventArgs e)
        {
            nameValuePairsToDeleteListBox.DataSource = ItemList;
            mainKeyComboBox.SelectedIndex = 0;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            ItemList.RemoveAt(nameValuePairsToDeleteListBox.SelectedIndex);
        }

        private void valueNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addButton.PerformClick();
        }
    }
}