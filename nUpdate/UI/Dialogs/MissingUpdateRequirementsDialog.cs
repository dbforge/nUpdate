// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Localization;

namespace nUpdate.UI.Dialogs
{
    public partial class MissingUpdateRequirementsDialog : BaseDialog
    {
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        public MissingUpdateRequirementsDialog()
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

        private void MissingUpdateRequirementsDialog_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LanguageFilePath))
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
            else if (string.IsNullOrEmpty(LanguageFilePath) && LanguageName != "en")
            {
                string resourceName = $"nUpdate.Localization.{LanguageName}.json";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (string.IsNullOrEmpty(LanguageFilePath) && LanguageName == "en")
            {
                _lp = new LocalizationProperties();
            }

            closeButton.Text = _lp.CloseButtonText;
            headerLabel.Text = _lp.MissingRequirementDialogHeader;
            infoLabel.Text = _lp.MissingRequirementDialogInfoText;

            Icon = _appIcon;
            Text = Application.ProductName;
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            ((DialogResultReference)dialogResultReference).DialogResult = ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}