// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class StatisticsServerEditDialog : BaseDialog
    {
        public StatisticsServerEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The url of the SQL-host.
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        ///     The name of the database to use.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     The username for the SQL-connection.
        /// </summary>
        public string Username { get; set; }

        private void StatisticsServerEditDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
            hostTextBox.Text = WebUrl;
            databaseTextBox.Text = DatabaseName;
            usernameTextBox.Text = Username;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            WebUrl = hostTextBox.Text;
            DatabaseName = databaseTextBox.Text;
            Username = usernameTextBox.Text;
            DialogResult = DialogResult.OK;
        }
    }
}