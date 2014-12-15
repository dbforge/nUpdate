// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 22-08-2014 20:36

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class RegistryEntryDeleteOperationPanel : UserControl, IOperationPanel
    {
        private BindingList<string> _itemList = new BindingList<string>();

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

        public BindingList<string> ItemList
        {
            get { return _itemList; }
            set { _itemList = value; }
        }

        public RegistryEntryDeleteOperationPanel()
        {
            InitializeComponent();
        }

        private void RegistryEntryDeleteOperationPanel_Load(object sender, EventArgs e)
        {
            entriesToDeleteListBox.DataSource = _itemList;
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
            _itemList.RemoveAt(entriesToDeleteListBox.SelectedIndex);
        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Registry, OperationMethods.Delete, KeyPath, ItemList.ToList());
            }
        }
    }
}