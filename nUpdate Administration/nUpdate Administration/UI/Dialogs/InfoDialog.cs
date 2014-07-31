using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class InfoDialog : BaseDialog
    {
        public InfoDialog()
        {
            InitializeComponent();
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            copyrightLabel.Text += DateTime.Now.Year.ToString();
        }

        private void iconPackLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://p.yusukekamiyamane.com/"); 
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InfoForm_Shown(object sender, EventArgs e)
        {
            this.closeButton.Focus();
        }

        private void timSchieweLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/timmi31061");
        }


        private void artentusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Artentus");
        }

        private void ll_github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/ProgTrade/nUpdate");
        }

        private void websiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.nupdate.net");
        }

        private void licenseInfoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://creativecommons.org/licenses/by-nd/2.0/");
        }
    }
}
