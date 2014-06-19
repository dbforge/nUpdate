using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nUpdate.Internal.UpdateEventArgs;
using nUpdate.Dialogs;
using System.Net;
using System.Reflection;
using System.IO;
using nUpdate.Core.Language;

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
            headerLabel.Text = lang.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            Icon = AppIcon; 
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
                        DialogResult = DialogResult.Cancel;
                    }
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
