// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Windows.Forms;
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

        /// <summary>
        ///     Gets or sets the progress percentage.
        /// </summary>
        internal float ProgressPercentage
        {
            get => downloadProgressBar.Value;
            set
            {
                try
                {
                    Invoke(new Action(() =>
                    {
                        downloadProgressBar.Value = (int) value;
                        infoLabel.Text = string.Format(
                            _lp.UpdateDownloadDialogLoadingInfo, Math.Round(value, 1));
                    }));
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

        public void Failed(object sender, FailedEventArgs e)
        {
            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
                "Error while downloading the update package.",
                e.Exception.InnerException ?? e.Exception, PopupButtons.Ok)));
            DialogResult = DialogResult.Cancel;
        }

        public void Finished(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public void ProgressChanged(object sender, UpdateDownloadProgressChangedEventArgs e)
        {
            ProgressPercentage = e.Percentage;
        }

        public void StatisticsEntryFailed(object sender, FailedEventArgs e)
        {
            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Warning,
                "Error while adding a new statistics entry.",
                e.Exception, PopupButtons.Ok)));
            DialogResult = DialogResult.OK;
        }

        private void UpdateDownloadDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                /* Important to unsubscribe the events, otherwise any calls on the UI thread (Invoke) 
                will cause an InvalidOperationException as the window handle is no longer available. */

                UpdateManager.PackagesDownloadProgressChanged -= ProgressChanged;
                UpdateManager.PackagesDownloadFailed -= Failed;
                UpdateManager.StatisticsEntryFailed -= StatisticsEntryFailed;
                UpdateManager.PackagesDownloadFinished -= Finished;
                return;
            }
            e.Cancel = true;
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

            UpdateManager.PackagesDownloadProgressChanged += ProgressChanged;
            UpdateManager.PackagesDownloadFailed += Failed;
            UpdateManager.StatisticsEntryFailed += StatisticsEntryFailed;
            UpdateManager.PackagesDownloadFinished += Finished;
        }

        private void UpdateDownloadDialog_Shown(object sender, EventArgs e)
        {
            UpdateManager.DownloadPackagesAsync();
        }
    }
}