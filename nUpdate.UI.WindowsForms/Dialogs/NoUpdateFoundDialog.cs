// NoUpdateFoundDialog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Localization;

namespace nUpdate.UI.WindowsForms.Dialogs
{
    internal partial class NoUpdateFoundDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        internal NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            closeButton.Text = _lp.CloseButtonText;
            headerLabel.Text = _lp.NoUpdateDialogHeader;
            infoLabel.Text = _lp.NoUpdateDialogInfoText;

            Icon = _appIcon;
            Text = Application.ProductName;
        }
    }
}