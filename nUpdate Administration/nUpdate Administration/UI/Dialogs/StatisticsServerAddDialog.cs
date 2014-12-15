// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
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
        ///     The url of the SQL-host.
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        ///     The name of the database to use.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     The username for the connection.
        /// </summary>
        public string Username { get; set; }

        private void StatisticsServerAddDialog_Load(object sender, EventArgs e)
        {
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