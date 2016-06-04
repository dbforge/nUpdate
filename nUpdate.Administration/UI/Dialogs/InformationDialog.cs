// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    internal partial class InformationDialog : BaseDialog
    {
        public InformationDialog()
        {
            InitializeComponent();
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
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

        private void donatePictureBox_Click(object sender, EventArgs e)
        {
            const string business = "dominic.beger@hotmail.de";
            const string description = "Will%20be%20used%20for%20code%20signing%20certificates%20for%20nUpdate";
            const string country = "DE";
            const string currency = "EUR";

            var url =
                $"https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business={business}&lc={country}&item_name={description}&currency_code={currency}&bn=PP%2dDonationsBF";

            Process.Start(url);
        }

        private void dotNetZipLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://dotnetzip.codeplex.com/");
        }

        private void jsonNetLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://james.newtonking.com/json");
        }

        private void fastColoredTextBoxLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting");
        }

        private void bikoLibraryLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://biko.codeplex.com/");
        }

        private void iconLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.github.com/stefan-baumann");
        }

        private void nafetsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.github.com/stefan-baumann");
        }
    }
}