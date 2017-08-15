// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;

namespace nUpdate.UI.Dialogs
{
    public partial class NoUpdateFoundDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        public NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(Updater.LanguageCulture, Updater.CultureFilePaths);

            closeButton.Text = _lp.CloseButtonText;
            headerLabel.Text = _lp.NoUpdateDialogHeader;
            infoLabel.Text = _lp.NoUpdateDialogInfoText;

            Icon = _appIcon;
            Text = Application.ProductName;
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            if (dialogResultReference != null)
                ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
            else
                ShowDialog();
        }
    }
}