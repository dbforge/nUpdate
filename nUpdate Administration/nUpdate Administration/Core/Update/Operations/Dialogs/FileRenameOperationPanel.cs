using System;
using System.Windows.Forms;

namespace nUpdate.Administration.Core.Update.Operations.Dialogs
{
    public partial class FileRenameOperationPanel : UserControl
    {
        public FileRenameOperationPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The path of the file to rename.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        ///     The new name of the file.
        /// </summary>
        public string NewName { get; set; }

        private void pathTextBox_Leave(object sender, EventArgs e)
        {
            FilePath = pathTextBox.Text;
        }

        private void newNameTextBox_Leave(object sender, EventArgs e)
        {
            NewName = newNameTextBox.Text;
        }
    }
}