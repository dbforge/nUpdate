using System;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class StatisticsServerEditDialog : BaseDialog
    {
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

        public StatisticsServerEditDialog()
        {
            InitializeComponent();
        }

        private void StatisticsServerEditDialog_Load(object sender, EventArgs e)
        {
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
