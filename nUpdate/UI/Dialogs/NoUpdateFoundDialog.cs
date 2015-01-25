// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;

namespace nUpdate.UI.Dialogs
{
    public partial class NoUpdateFoundDialog : BaseDialog
    {
        private LocalizationProperties _lp;
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        public NoUpdateFoundDialog()
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

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
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

            closeButton.Text = _lp.CloseButtonText;
            headerLabel.Text = _lp.NoUpdateDialogHeader;
            infoLabel.Text = _lp.NoUpdateDialogInfoText;

            Icon = _appIcon;
            Text = Application.ProductName;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}