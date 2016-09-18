// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.UI.Dialogs
{
    public partial class UpdateDownloadDialog : BaseDialog
    {
        private LocalizationProperties _lp;
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);

        public UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the name of the language file in the resources to use, if no own file is used.
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        ///     Gets or sets the path of the file which contains the specific language content a user added on its own.
        /// </summary>
        public string LanguageFilePath { get; set; }

        /// <summary>
        ///     Gets or sets the packages amount.
        /// </summary>
        public int PackagesCount { get; set; }

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
                        infoLabel.Text = String.Format(
                            _lp.UpdateDownloadDialogLoadingInfo, value);
                    }));
                }
                catch (InvalidOperationException)
                {
                    // Prevent race conditions
                }
            }
        }

        /// <summary>
        ///     Occurs when the cancel button is clicked.
        /// </summary>
        public event EventHandler<EventArgs> CancelButtonClicked;

        protected virtual void OnCancelButtonClicked()
        {
            if (CancelButtonClicked != null)
                CancelButtonClicked(this, EventArgs.Empty);
        }

        private void UpdateDownloadDialog_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(LanguageFilePath))
            {
                try
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(LanguageFilePath));
                }
                catch (Exception)
                {
                    _lp = new LocalizationProperties();
                }
            }
            else if (String.IsNullOrEmpty(LanguageFilePath) && LanguageName != "en")
            {
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json", LanguageName);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (String.IsNullOrEmpty(LanguageFilePath) && LanguageName == "en")
            {
                _lp = new LocalizationProperties();
            }

            headerLabel.Text = _lp.UpdateDownloadDialogLoadingHeader;
            infoLabel.Text = String.Format(
                        _lp.UpdateDownloadDialogLoadingInfo, "0");
            cancelButton.Text = _lp.CancelButtonText;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            if (dialogResultReference != null)
                ((DialogResultReference)dialogResultReference).DialogResult = ShowDialog();
            else
                ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        #region TAP

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

        #endregion

        #region EAP

        public void ProgressChanged(object sender, UpdateDownloadProgressChangedEventArgs e)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    downloadProgressBar.Value = (int)e.Percentage;
                    infoLabel.Text = String.Format(
                        _lp.UpdateDownloadDialogLoadingInfo, (int)e.Percentage);
                }));
            }
            catch (InvalidOperationException)
            {
                // Prevent race conditions
            }
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

        #endregion

        private void cancelButton_Click(object sender, EventArgs e)
        {
            OnCancelButtonClicked();
            DialogResult = DialogResult.Cancel;
        }
    }
}