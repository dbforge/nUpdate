using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Dialogs;
using nUpdate.Core.Language;
using System.Reflection;

namespace nUpdate.Dialogs
{
    public partial class UpdateDownloadDialog : BaseForm
    {
        string finishedHeader = null;
        string finishedInfoText = null;
        string finishedStatusLabelText = null;

        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

        public UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        private void UpdateDownloadDialog_Load(object sender, EventArgs e)
        {
            string resourceName = "nUpdate.Core.Language.";
            LanguageSerializer lang = null;

            if (Language != Language.Custom)
            {
                switch (Language)
                {
                    case Language.English:
                        resourceName += "en.xml";
                        break;
                    case Language.German:
                        resourceName += "de.xml";
                        break;
                    case Language.French:
                        resourceName += "fr.xml";
                        break;
                    case Language.Spanish:
                        resourceName += "es.xml";
                        break;
                }
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    lang = LanguageSerializer.ReadXml(stream);
                }
            }
            else
            {
                if (File.Exists(LanguageFilePath))
                {
                    lang = LanguageSerializer.ReadXml(LanguageFilePath);
                }
            }

            cancelButton.Text = lang.CancelButtonText;
            furtherButton.Text = lang.ContinueButtonText;
            headerLabel.Text = lang.UpdateDownloadDialogLoadingHeader;
            infoLabel.Text = lang.UpdateDownloadDialogLoadingInfo;
            statusLabel.Text = lang.UpdateDownloadDialogLoadingPackageText;
            finishedHeader = lang.UpdateDownloadDialogFinishedHeader;
            finishedInfoText = lang.UpdateDownloadDialogFinishedInfoText;
            finishedStatusLabelText = lang.UpdateDownloadDialogFinishedPackageText;

            hookPictureBox.Visible = false;
            cancelButton.Enabled = false;
            furtherButton.Enabled = false;

            this.Text = Application.ProductName;
            this.Icon = AppIcon;

            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;
            iconPictureBox.Image = AppIcon.ToBitmap();
        }

        public void furtherButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public void ProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                downloadProgressBar.Value = e.ProgressPercentage;
                percentLabel.Text = String.Format("{0} %", e.ProgressPercentage);
            }));
        }

        public void DownloadFinishedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var errorDialog = new UpdateErrorDialog();
                if (e.Error.GetType() == typeof(WebException))
                {
                    HttpWebResponse response = null;
                    response = (HttpWebResponse)(e.Error as WebException).Response;
                    errorDialog.ErrorCode = (int)response.StatusCode;
                }
                else
                {
                    errorDialog.ErrorCode = 0;
                }

                errorDialog.InfoMessage = "Error while downloading the package.";
                errorDialog.ErrorMessage = e.Error.Message;
                Invoke(new Action(() =>
                {
                    if (errorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        DialogResult = DialogResult.Cancel;
                    }
                }));
            }
            else
            {
                Invoke(new Action(() =>
                    {
                        cancelButton.Enabled = true;
                        furtherButton.Enabled = true;

                        headerLabel.Text = finishedHeader;
                        infoLabel.Text = finishedInfoText;
                        statusLabel.Text = finishedStatusLabelText;
                    }));
            }
        }

        public void DownloadFailedEventHandler(Exception exception)
        {
            var errorDialog = new UpdateErrorDialog();
            if (exception.GetType() == typeof(WebException))
                errorDialog.ErrorCode = (int)((HttpWebResponse)(exception as WebException).Response).StatusCode;
            else
                errorDialog.ErrorCode = 0;
            errorDialog.InfoMessage = "Error while downloading.";
            errorDialog.ErrorMessage = exception.Message;
            if (errorDialog.ShowDialog(this) == DialogResult.OK)
                DialogResult = DialogResult.Cancel;
        }
    }
}
