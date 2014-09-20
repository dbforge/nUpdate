using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core.Language;

namespace nUpdate.Dialogs
{
    public partial class UpdateDownloadDialog : BaseForm
    {
        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        private string finishedHeader;
        private string finishedInfoText;
        private string finishedStatusLabelText;

        public UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

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
                    case Language.Russian:
                        resourceName += "ru.xml";
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
                    lang = LanguageSerializer.ReadXml(LanguageFilePath);
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

            Text = Application.ProductName;
            Icon = AppIcon;

            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;
            iconPictureBox.Image = AppIcon.ToBitmap();
        }

        public void furtherButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public void ProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            int sizeDividor = 1024;
            string sizeString = "KB";
            if (e.TotalBytesToReceive >= 104857.6)
            {
                sizeDividor = 1048576;
                sizeString = "MB";
            }

            Invoke(new Action(() =>
            {
                downloadProgressBar.Value = e.ProgressPercentage;
                percentLabel.Text = String.Format("{0} %", e.ProgressPercentage);
                statusLabel.Text = String.Format("{0} {2} of {1} {2}",
                    Math.Round((double) e.BytesReceived/sizeDividor, 1),
                    Math.Round((double) e.TotalBytesToReceive/sizeDividor, 1), sizeString);
            }));
        }

        public void DownloadFinishedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var errorDialog = new UpdateErrorDialog();
                if (e.Error.GetType() == typeof (WebException))
                {
                    HttpWebResponse response = null;
                    response = (HttpWebResponse) (e.Error as WebException).Response;
                    errorDialog.ErrorCode = (int) response.StatusCode;
                }
                else
                    errorDialog.ErrorCode = 0;

                errorDialog.InfoMessage = "Error while downloading the package.";
                errorDialog.Error = e.Error;
                Invoke(new Action(() =>
                {
                    if (errorDialog.ShowDialog(this) == DialogResult.OK)
                        DialogResult = DialogResult.Cancel;
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
            if (exception.GetType() == typeof (WebException))
                errorDialog.ErrorCode = (int) ((HttpWebResponse) (exception as WebException).Response).StatusCode;
            else
                errorDialog.ErrorCode = 0;
            errorDialog.InfoMessage = "Error while downloading.";
            errorDialog.Error = exception;
            if (errorDialog.ShowDialog(this) == DialogResult.OK)
                DialogResult = DialogResult.Cancel;
        }
    }
}