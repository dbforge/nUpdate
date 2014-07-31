using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core.Language;
using nUpdate.Dialogs;

namespace nUpdate.UI.Dialogs
{
    public partial class UpdateSearchDialog : BaseForm
    {
        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

        private void SearchDialog_Load(object sender, EventArgs e)
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
            headerLabel.Text = lang.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            Icon = AppIcon;
        }

        public void SearchFailedEventHandler(Exception ex)
        {
            var errorDialog = new UpdateErrorDialog();
            if (ex.GetType() == typeof (WebException))
            {
                HttpWebResponse response = null;
                response = (HttpWebResponse) (ex as WebException).Response;
                errorDialog.ErrorCode = (int) response.StatusCode;
            }
            else
                errorDialog.ErrorCode = 0;

            errorDialog.InfoMessage = "Error while searching for updates.";
            errorDialog.Error = ex;
            Invoke(new Action(() =>
            {
                if (errorDialog.ShowDialog(this) == DialogResult.OK)
                    DialogResult = DialogResult.Cancel;
            }));
        }

        public void SearchFinishedEventHandler(bool updateFound)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}