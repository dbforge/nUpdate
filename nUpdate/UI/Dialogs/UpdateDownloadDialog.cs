// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Net;
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
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        public UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets the name of the _lpuage file in the resources to use, if no own file is used.
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        ///     Sets the path of the file which contains the specific _lpuage content a user added on its own.
        /// </summary>
        public string LanguageFilePath { get; set; }

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

        public void ProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            // Race condition, that's why there is a catch that suppresses the error.

            try
            {
                Invoke(new Action(() =>
                {
                    downloadProgressBar.Value = e.ProgressPercentage;
                    infoLabel.Text = String.Format(
                        _lp.UpdateDownloadDialogLoadingInfo, e.ProgressPercentage);
                }));
            }
            catch (InvalidOperationException)
            {
                // Suppress it
            }
        }

        public void DownloadFinishedEventHandler(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public void DownloadFailedEventHandler(object sender, FailedEventArgs e)
        {
            var ex = e.Exception;
            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
                "Error while downloading the update package.",
                ex, PopupButtons.Ok)));

            DialogResult = DialogResult.Cancel;
        }

        public void StatisticsEntryFailedEventHandler(object sender, FailedEventArgs e)
        {
            Invoke(new Action(() =>
                    Popup.ShowPopup(this, SystemIcons.Warning,
                        "Error while adding a new statistics entry.",
                        e.Exception, PopupButtons.Ok)));
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}