using nUpdate.Core.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace nUpdate.Dialogs
{
    public partial class UpdateDownloadDialog : BaseForm
    {
        private string finishedHeader = null;
        private string finishedInfoText = null;
        private string finishedStatusLabelText = null;

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

            if (this.Language != Language.Custom)
            {
                switch (this.Language)
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
                if (File.Exists(this.LanguageFilePath))
                {
                    lang = LanguageSerializer.ReadXml(this.LanguageFilePath);
                }
            }

            this.cancelButton.Text = lang.CancelButtonText;
            this.furtherButton.Text = lang.ContinueButtonText;
            this.headerLabel.Text = lang.UpdateDownloadDialogLoadingHeader;
            this.infoLabel.Text = lang.UpdateDownloadDialogLoadingInfo;
            this.statusLabel.Text = lang.UpdateDownloadDialogLoadingPackageText;
            this.finishedHeader = lang.UpdateDownloadDialogFinishedHeader;
            this.finishedInfoText = lang.UpdateDownloadDialogFinishedInfoText;
            this.finishedStatusLabelText = lang.UpdateDownloadDialogFinishedPackageText;

            this.hookPictureBox.Visible = false;
            this.cancelButton.Enabled = false;
            this.furtherButton.Enabled = false;

            this.Text = Application.ProductName;
            this.Icon = this.AppIcon;

            this.iconPictureBox.BackgroundImageLayout = ImageLayout.Center;
            this.iconPictureBox.Image = this.AppIcon.ToBitmap();
        }

        public void furtherButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
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
                this.downloadProgressBar.Value = e.ProgressPercentage;
                this.percentLabel.Text = String.Format("{0} %", e.ProgressPercentage);
                this.statusLabel.Text = String.Format("{0} {2} of {1} {2}", Math.Round((double)e.BytesReceived / sizeDividor, 1), Math.Round((double)e.TotalBytesToReceive / sizeDividor, 1), sizeString);
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
                errorDialog.Error = e.Error;
                Invoke(new Action(() =>
                {
                    if (errorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }
                }));
            }
            else
            {
                Invoke(new Action(() =>
                    {
                        this.cancelButton.Enabled = true;
                        this.furtherButton.Enabled = true;

                        this.headerLabel.Text = this.finishedHeader;
                        this.infoLabel.Text = this.finishedInfoText;
                        this.statusLabel.Text = this.finishedStatusLabelText;
                    }));
            }
        }

        public void DownloadFailedEventHandler(Exception exception)
        {
            var errorDialog = new UpdateErrorDialog();
            if (exception.GetType() == typeof(WebException))
            {
                errorDialog.ErrorCode = (int)((HttpWebResponse)(exception as WebException).Response).StatusCode;
            }
            else
            {
                errorDialog.ErrorCode = 0;
            }
            errorDialog.InfoMessage = "Error while downloading.";
            errorDialog.Error = exception;
            if (errorDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
