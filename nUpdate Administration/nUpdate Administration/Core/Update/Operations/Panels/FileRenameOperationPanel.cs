// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public partial class FileRenameOperationPanel : UserControl, IOperationPanel
    {
        public FileRenameOperationPanel()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return pathTextBox.Text; }
            set { pathTextBox.Text = value; }
        }

        public string NewName
        {
            get { return newNameTextBox.Text; }
            set { newNameTextBox.Text = value; }
        }

        private void FileRenameOperationPanel_Load(object sender, EventArgs e)
        {
            // Language initializing follows here
        }

        public Operation Operation
        {
            get
            {
                return new Operation(OperationArea.Files, OperationMethods.Rename, Path, NewName);
            }
        }
    }
}