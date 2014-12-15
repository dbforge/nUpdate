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
        private BindingList<string> _itemList = new BindingList<string>(); 

        public FileDeleteOperationPanel()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return pathTextBox.Text; }
            set { pathTextBox.Text = value; }
        }

        public BindingList<string> ItemList
        {
            get { return _itemList; }
            set { _itemList = value; }
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
                return new Operation(OperationArea.Files, OperationMethods.Delete, Path, ItemList.ToList());
            }
        }
    }
}