// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    internal partial class StatisticalServerEditDialog : BaseDialog
    {
        public StatisticalServerEditDialog()
        {
            InitializeComponent();
        }

        public Uri WebUri { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }

        private void StatisticsServerEditDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
            hostTextBox.Text = WebUri.ToString();
            databaseTextBox.Text = DatabaseName;
            usernameTextBox.Text = Username;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            WebUri = new Uri(hostTextBox.Text);
            DatabaseName = databaseTextBox.Text;
            Username = usernameTextBox.Text;
            DialogResult = DialogResult.OK;
        }
    }
}