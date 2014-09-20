// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class RegistryEntryCreateOperationPanel : UserControl, IOperationPanel
    {
        private readonly BindingList<string> _itemList = new BindingList<string>(); 

        public RegistryEntryCreateOperationPanel()
        {
            InitializeComponent();
        }

        private void RegistryEntryCreateOperationPanel_Load(object sender, EventArgs e)
        {
            entriesToCreateListBox.DataSource = _itemList;
            mainKeyComboBox.SelectedIndex = 0;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(keyNameTextBox.Text))
                return;
            _itemList.Add(keyNameTextBox.Text);
            keyNameTextBox.Clear();
        }

        private void keyNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addButton.PerformClick();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            _itemList.RemoveAt(entriesToCreateListBox.SelectedIndex);
        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Registry, OperationMethods.Create, String.Format("{0}\\{1}", mainKeyComboBox.GetItemText(mainKeyComboBox.SelectedIndex), appKeyTextBox.Text), keyNameTextBox.Text);
            }
        }
    }
}