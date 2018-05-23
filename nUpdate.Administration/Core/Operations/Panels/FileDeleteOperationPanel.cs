// Copyright © Dominic Beger 2018

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class FileDeleteOperationPanel : UserControl, IOperationPanel
    {
        public FileDeleteOperationPanel()
        {
            InitializeComponent();
        }

        public BindingList<string> ItemList { get; set; } = new BindingList<string>();

        public string Path
        {
            get => pathTextBox.Text.EndsWith("\\") ? pathTextBox.Text : pathTextBox.Text += "\\";
            set => pathTextBox.Text = value;
        }

        public bool IsValid => ItemList.Any();

        public Operation Operation
            => new Operation(OperationArea.Files, OperationMethod.Delete, Path, ItemList.ToList());

        private void addButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileNameTextBox.Text))
                return;
            ItemList.Add(fileNameTextBox.Text);
            fileNameTextBox.Clear();
        }

        private void environmentVariablesButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Environment variables.",
                "%appdata%: AppData\n%temp%: Temp\n%program%: Program's directory\n%desktop%: Desktop directory",
                PopupButtons.Ok);
        }

        private void FileDeleteOperationPanel_Load(object sender, EventArgs e)
        {
            filesToDeleteListBox.DataSource = ItemList;
        }

        private void fileNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addButton.PerformClick();
        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!pathTextBox.Text.Contains("/"))
                return;
            pathTextBox.Text = pathTextBox.Text.Replace('/', '\\');
            pathTextBox.SelectionStart = pathTextBox.Text.Length;
            pathTextBox.SelectionLength = 0;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            ItemList.RemoveAt(filesToDeleteListBox.SelectedIndex);
        }
    }
}