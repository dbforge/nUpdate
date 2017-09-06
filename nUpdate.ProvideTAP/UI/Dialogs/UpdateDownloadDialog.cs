// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.UI.Popups;

namespace nUpdate.UI.Dialogs
{
    public partial class UpdateDownloadDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        public UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the progress percentage.
        /// </summary>
        public int ProgressPercentage
        {
            get => downloadProgressBar.Value;
            set
            {
                try
                {
                    Invoke(new Action(() =>
                    {
                        downloadProgressBar.Value = value;
                        infoLabel.Text = string.Format(
                            _lp.UpdateDownloadDialogLoadingInfo, value);
                    }));
                }
                catch (InvalidOperationException)
                {
                    // Prevent race conditions
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            OnCancelButtonClicked();
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        ///     Occurs when the cancel button is clicked.
        /// </summary>
        public event EventHandler<EventArgs> CancelButtonClicked;

        public void CloseDialog(object state)
        {
            Close();
        }

        protected virtual void OnCancelButtonClicked()
        {
            CancelButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            if (dialogResultReference != null)
                ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
            else
                ShowDialog();
        }

        private void UpdateDownloadDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(Updater.LanguageCulture, Updater.CultureFilePaths);

            headerLabel.Text = _lp.UpdateDownloadDialogLoadingHeader;
            infoLabel.Text = string.Format(
                _lp.UpdateDownloadDialogLoadingInfo, "0");
            cancelButton.Text = _lp.CancelButtonText;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        public void Fail(Exception ex)
        {
            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
                "Error while downloading the update package.",
                ex.InnerException ?? ex, PopupButtons.Ok)));
        }

        public void StatisticsEntryFail(Exception ex)
        {
            Invoke(new Action(() =>
                Popup.ShowPopup(this, SystemIcons.Warning,
                    "Error while adding a new statistics entry.",
                    ex, PopupButtons.Ok)));
        }
    }
}