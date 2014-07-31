using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Dialogs
{
    public partial class FileDeleteOperationPanel : UserControl
    {
        /// <summary>
        /// The path of the files that should be deleted.
        /// </summary>
        public string FilesPath { get; set; }

        /// <summary>
        /// The list of names of the files that should be deleted in the path set.
        /// </summary>
        public List<string> FileNames { get; set; }

        public FileDeleteOperationPanel()
        {
            InitializeComponent();
        }

        private void FileDeleteOperationPanel_Load(object sender, EventArgs e)
        {
            this.FileNames = new List<string>();
        }

        private void pathTextBox_Leave(object sender, EventArgs e)
        {
            this.FilesPath = this.pathTextBox.Text;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            this.FileNames.Add(fileNameTextBox.Text);
            this.filesToDeleteListBox.Items.Add(this.fileNameTextBox.Text);
            this.fileNameTextBox.Clear();
        }

        private void filesToDeleteListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.filesToDeleteListBox.SelectedItem != null)
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    this.FileNames.Remove(this.filesToDeleteListBox.SelectedItem.ToString());
                    this.filesToDeleteListBox.Items.Remove(this.filesToDeleteListBox.SelectedItem);
                }
            }
        }

        private void fileNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.addButton.PerformClick();
            }
        }
    }
}
