// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.UI.Popups;

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
                    /*string resourceName = "nUpdate.Core.Localization.en.json";
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                    {
                        _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                    }*/

                    _lp = new LocalizationProperties();
                }
            }
            else
            {
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json", LanguageName);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            //headerLabel.Text = _lp.UpdateDownloadDialogLoadingHeader;
            //infoLabel.Text = _lp.UpdateDownloadDialogLoadingInfo;

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
                        "Please wait while the available updates are\ndownloaded...  ({0}%)", e.ProgressPercentage);
                }));
            }
            catch (InvalidOperationException)
            {
                // Suppress it
            }
        }

        public void DownloadFinishedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null && e.Error.InnerException != null)
            {
                Invoke(new Action(() =>
                {
                    if (
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while downloading the update package.",
                            e.Error.InnerException, PopupButtons.Ok) == DialogResult.OK)
                        DialogResult = DialogResult.Cancel;
                }));
            }

            DialogResult = DialogResult.OK;
        }
    }
}