// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.UI.Dialogs
{
    public partial class UpdateSearchDialog : BaseDialog
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
                    _lp = new LocalizationProperties();
                }
            }
            else if (String.IsNullOrEmpty(LanguageFilePath) && LanguageName != "en")
            {
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json", LanguageName);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (String.IsNullOrEmpty(LanguageFilePath) && LanguageName == "en")
            {
                _lp = new LocalizationProperties();
            }

            cancelButton.Text = _lp.CancelButtonText;
            headerLabel.Text = _lp.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        public void SearchFailedEventHandler(object sender, FailedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption, e.Exception,
                    PopupButtons.Ok);
                DialogResult = DialogResult.Cancel;
            }));
        }

        public void SearchFinishedEventHandler(object sender, UpdateSearchFinishedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}