// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.UI.WinForms.Popups;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class UpdateDownloadDialog : BaseDialog
    {
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
            get { return downloadProgressBar.Value; }
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

        private void UpdateDownloadDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(Updater.LanguageCulture, Updater.LocalizationFilePaths);

            headerLabel.Text = _lp.UpdateDownloadDialogLoadingHeader;
            infoLabel.Text = string.Format(
                _lp.UpdateDownloadDialogLoadingInfo, "0");
            cancelButton.Text = _lp.CancelButtonText;

            Text = Application.ProductName;
            Icon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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