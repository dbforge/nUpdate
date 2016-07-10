// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class StatisticalServerAddDialog : BaseDialog
    {
        public StatisticalServerAddDialog()
        {
            InitializeComponent();
        }

        public Uri WebUri { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }

        private void saveButton_Click(object sender, EventArgs e)
        {
            WebUri = new Uri(hostTextBox.Text);
            DatabaseName = databaseTextBox.Text;
            Username = usernameTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void StatisticsServerAddDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
        }
    }
}