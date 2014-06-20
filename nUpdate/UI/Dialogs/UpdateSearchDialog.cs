using nUpdate.Core.Language;
using nUpdate.Dialogs;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace nUpdate.UI.Dialogs
{
    public partial class UpdateSearchDialog : BaseForm
    {
        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        private void SearchDialog_Load(object sender, EventArgs e)
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
            this.headerLabel.Text = lang.UpdateSearchDialogHeader;

            this.Text = Application.ProductName;
            this.Icon = AppIcon; 
        }

        public void SearchFailedEventHandler(Exception ex)
        {
            var errorDialog = new UpdateErrorDialog();
            if (ex.GetType() == typeof(WebException))
            {
                HttpWebResponse response = null;
                response = (HttpWebResponse)(ex as WebException).Response;
                errorDialog.ErrorCode = (int)response.StatusCode;
            }
            else
            {
                errorDialog.ErrorCode = 0;
            }

            errorDialog.InfoMessage = "Error while searching for updates.";
            errorDialog.ErrorMessage = ex.Message;
            Invoke(new Action(() =>
                {
                    if (errorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }
                }));
        }

        public void SearchFinishedEventHandler(bool updateFound)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
