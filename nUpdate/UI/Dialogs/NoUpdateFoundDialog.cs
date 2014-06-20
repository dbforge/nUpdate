using nUpdate.Core.Language;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace nUpdate.Dialogs
{
    public partial class NoUpdateFoundDialog : BaseForm
    {
        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

        public NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
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
                        this.headerLabel.Location = new Point(19, 12);
                        this.infoLabel.Location = new Point(36, 34);
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
                if (File.Exists(this.LanguageFilePath))
                {
                    lang = LanguageSerializer.ReadXml(this.LanguageFilePath);
                }
            }

            this.closeButton.Text = lang.CloseButtonText;
            this.headerLabel.Text = lang.NoUpdateDialogHeader;
            this.infoLabel.Text = lang.NoUpdateDialogInfoText;

            this.Icon = this.AppIcon;
            this.Text = Application.ProductName;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
