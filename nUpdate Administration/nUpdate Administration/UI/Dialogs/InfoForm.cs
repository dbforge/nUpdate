using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class InfoForm : BaseForm
    {
        public InfoForm()
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
            Close();
        }

        private void InfoForm_Shown(object sender, EventArgs e)
        {
            closeButton.Focus();
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
