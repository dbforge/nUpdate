// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    public partial class ChangelogPanel : UserControl
    {
        public ChangelogPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Returns the changelog set.
        /// </summary>
        public string Changelog
        {
            get { return changelogTextBox.Text; }
            set { changelogTextBox.Text = value; }
        }

        public void Paste(string text)
        {
            changelogTextBox.Paste(text);
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