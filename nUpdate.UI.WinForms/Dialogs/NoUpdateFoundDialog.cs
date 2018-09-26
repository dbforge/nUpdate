// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;
using nUpdate.Localization;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class NoUpdateFoundDialog : BaseDialog
    {
        private LocalizationProperties _lp;

        public NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(Updater.LanguageCulture, Updater.LocalizationFilePaths);

            closeButton.Text = _lp.CloseButtonText;
            headerLabel.Text = _lp.NoUpdateDialogHeader;
            infoLabel.Text = _lp.NoUpdateDialogInfoText;

            Icon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}