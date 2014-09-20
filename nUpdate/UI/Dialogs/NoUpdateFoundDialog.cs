using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core.Language;

namespace nUpdate.Dialogs
{
    public partial class NoUpdateFoundDialog : BaseForm
    {
        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        public NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
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
                        headerLabel.Location = new Point(19, 12);
                        infoLabel.Location = new Point(36, 34);
                        break;
                    case Language.Russian:
                        resourceName += "ru.xml";
                        break;
                        //case Language.French:
                        //    resourceName += "fr.xml";
                        //    headerLabel.Location = new Point(19, 12);
                        //    infoLabel.Location = new Point(36, 34);
                        //    break;
                        //case Language.Spanish:
                        //    resourceName += "es.xml";
                        //    headerLabel.Location = new Point(19, 12);
                        //    infoLabel.Location = new Point(36, 34);
                        //    break;
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

            closeButton.Text = lang.CloseButtonText;
            headerLabel.Text = lang.NoUpdateDialogHeader;
            infoLabel.Text = lang.NoUpdateDialogInfoText;

            Icon = AppIcon;
            Text = Application.ProductName;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}