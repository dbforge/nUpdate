using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    public partial class ChangelogPanel : UserControl
    {
        public ChangelogPanel()
        {
            InitializeComponent();
        }

        public void Paste(string text)
        {
            changelogTextBox.Paste(text);
        }

        /// <summary>
        ///     Returns the changelog set.
        /// </summary>
        public string Changelog
        {
            get { return changelogTextBox.Text; }
            set { changelogTextBox.Text = value; }
        }

        private void changelogTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
                changelogTextBox.SelectAll();
            else if (e.Control & e.KeyCode == Keys.Back)
                SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
        }
    }
}
