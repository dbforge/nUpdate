// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class FileDeleteOperationPanel : UserControl, IOperationPanel
    {
        private readonly BindingList<string> _itemList = new BindingList<string>(); 

        public FileDeleteOperationPanel()
        {
            InitializeComponent();
        }

        private void FileDeleteOperationPanel_Load(object sender, EventArgs e)
        {
            filesToDeleteListBox.DataSource = _itemList;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(fileNameTextBox.Text))
                return;
            _itemList.Add(fileNameTextBox.Text);
            fileNameTextBox.Clear();
        }

        private void fileNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addButton.PerformClick();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            _itemList.RemoveAt(filesToDeleteListBox.SelectedIndex);
        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Files, OperationMethods.Delete, pathTextBox.Text, _itemList.ToList());
            }
        }
    }
}