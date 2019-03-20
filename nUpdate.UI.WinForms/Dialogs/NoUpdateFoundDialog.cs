// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class NoUpdateFoundDialog : BaseDialog
    {
        public NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = UpdateProvider.LanguageCulture;

            closeButton.Text = Properties.strings.CloseButtonText;
            headerLabel.Text = Properties.strings.NoUpdateDialogHeader;
            infoLabel.Text = Properties.strings.NoUpdateDialogInfoText;

            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}