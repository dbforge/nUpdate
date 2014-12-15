using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.Dialogs;
using nUpdate.UI.Popups;

namespace nUpdate.UI.Dialogs
{
    public partial class UpdateSearchDialog : BaseForm
    {
        private LocalizationProperties _lp;
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets the name of the languguage file in the resources to use, if no own file is used.
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        ///     Sets the path of the file which contains the specific _lpuage content a user added on its own.
        /// </summary>
        public string LanguageFilePath { get; set; }

        private void SearchDialog_Load(object sender, EventArgs e)
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

            cancelButton.Text = _lp.CancelButtonText;
            headerLabel.Text = _lp.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        public void SearchFailedEventHandler(Exception ex)
        {
            Invoke(new Action(() =>
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while searching for updates.", ex, PopupButtons.Ok);
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