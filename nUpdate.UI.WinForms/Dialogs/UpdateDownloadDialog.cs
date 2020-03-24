// UpdateDownloadDialog.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Properties;
using nUpdate.UI.WinForms.Popups;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class UpdateDownloadDialog : BaseDialog
    {
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the progress percentage.
        /// </summary>
        public float ProgressPercentage
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
                            strings.UpdateDownloadDialogLoadingInfo, value);
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
            _cancellationTokenSource.Cancel();
            DialogResult = DialogResult.Cancel;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void UpdateDownloadDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            e.Cancel = true;
            Cancel();
        }

        private void UpdateDownloadDialog_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = UpdateProvider.LanguageCulture;

            headerLabel.Text = strings.UpdateDownloadDialogLoadingHeader;
            infoLabel.Text = string.Format(
                strings.UpdateDownloadDialogLoadingInfo, "0");
            cancelButton.Text = strings.CancelButtonText;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        private async void UpdateDownloadDialog_Shown(object sender, EventArgs e)
        {
            var progress = new Progress<UpdateProgressData>();
            progress.ProgressChanged += (o, value) => ProgressPercentage = value.Percentage;

            try
            {
                await UpdateProvider.DownloadUpdates(UpdateCheckResult, _cancellationTokenSource.Token, progress);
            }
            catch (OperationCanceledException)
            {
                return;
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
    }
}