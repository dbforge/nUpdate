// Copyright © Dominic Beger 2018

using System;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class StatisticsServerAddDialog : BaseDialog
    {
        public StatisticsServerAddDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The name of the database to use.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     The username for the SQL-connection.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The url of the SQL-host.
        /// </summary>
        public string WebUrl { get; set; }

        private void saveButton_Click(object sender, EventArgs e)
        {
            WebUrl = hostTextBox.Text;
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