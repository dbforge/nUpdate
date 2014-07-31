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
    public partial class FileRenameOperationPanel : UserControl
    {
        /// <summary>
        /// The path of the file to rename.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The new name of the file.
        /// </summary>
        public string NewName { get; set; }

        public FileRenameOperationPanel()
        {
            InitializeComponent();
        }

        private void pathTextBox_Leave(object sender, EventArgs e)
        {
            this.FilePath = this.pathTextBox.Text;
        }

        private void newNameTextBox_Leave(object sender, EventArgs e)
        {
            this.NewName = this.newNameTextBox.Text;
        }
    }
}
