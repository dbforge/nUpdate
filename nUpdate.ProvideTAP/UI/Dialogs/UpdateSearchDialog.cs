// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.Internal.UI.Popups;

namespace nUpdate.UI.Dialogs
{
    internal partial class UpdateSearchDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        internal UpdateSearchDialog()
        {
            InitializeComponent();
        }

        internal bool UpdatesFound { get; set; }

        private void Cancel()
        {
            UpdateManager.CancelSearchAsync();
            DialogResult = DialogResult.Cancel;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            cancelButton.Text = _lp.CancelButtonText;
            headerLabel.Text = _lp.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        private async void UpdateSearchDialog_Shown(object sender, EventArgs e)
        {
            try
            {
                UpdatesFound = await UpdateManager.SearchForUpdatesAsync();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                    PopupButtons.Ok);
                DialogResult = DialogResult.Cancel;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void UpdateSearchDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            e.Cancel = true;
            Cancel();
        }
    }
}