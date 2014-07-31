using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Dialogs
{
    public partial class FileDeleteOperationPanel : UserControl
    {
        public FileDeleteOperationPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The path of the files that should be deleted.
        /// </summary>
        public string FilesPath { get; set; }

        /// <summary>
        ///     The list of names of the files that should be deleted in the path set.
        /// </summary>
        public List<string> FileNames { get; set; }

        private void FileDeleteOperationPanel_Load(object sender, EventArgs e)
        {
            FileNames = new List<string>();
        }

        private void pathTextBox_Leave(object sender, EventArgs e)
        {
            FilesPath = pathTextBox.Text;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            FileNames.Add(fileNameTextBox.Text);
            filesToDeleteListBox.Items.Add(fileNameTextBox.Text);
            fileNameTextBox.Clear();
        }

        private void filesToDeleteListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (filesToDeleteListBox.SelectedItem != null)
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    FileNames.Remove(filesToDeleteListBox.SelectedItem.ToString());
                    filesToDeleteListBox.Items.Remove(filesToDeleteListBox.SelectedItem);
                }
            }
        }

        private void fileNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                addButton.PerformClick();
        }
    }
}