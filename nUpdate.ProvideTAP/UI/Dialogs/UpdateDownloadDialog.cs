// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft;
using nUpdate.Exceptions;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.Internal.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.UI.Dialogs
{
    internal partial class UpdateDownloadDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        internal UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        internal float ProgressPercentage
        {
            get => downloadProgressBar.Value;
            set
            {
                try
                {
                    downloadProgressBar.Value = (int) value;
                    infoLabel.Text = string.Format(CultureInfo.CurrentCulture,
                        _lp.UpdateDownloadDialogLoadingInfo, Math.Round(value, 1));
                }
                catch (InvalidOperationException)
                {
                    // Prevent race conditions
                }
            }
        }

        private void Cancel()
        {
            UpdateManager.CancelDownloadAsync();
            DialogResult = DialogResult.Cancel;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void UpdateDownloadDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            headerLabel.Text = _lp.UpdateDownloadDialogLoadingHeader;
            infoLabel.Text = string.Format(
                _lp.UpdateDownloadDialogLoadingInfo, "0");
            cancelButton.Text = _lp.CancelButtonText;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        private async void UpdateDownloadDialog_Shown(object sender, EventArgs e)
        {
            var progress = new Progress<UpdateDownloadProgressChangedEventArgs>();
            progress.ProgressChanged += (o, value) => ProgressPercentage = value.Percentage;

            try
            {
                await UpdateManager.DownloadPackagesAsync(progress);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (StatisticsException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Warning,
                    "Error while adding a new statistics entry.",
                    ex, PopupButtons.Ok);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error,
                    "Error while downloading the update package.",
                    ex.InnerException ?? ex, PopupButtons.Ok);
                DialogResult = DialogResult.Cancel;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void UpdateDownloadDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            e.Cancel = true;
            Cancel();
        }
    }
}