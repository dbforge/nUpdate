// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.UI.WinForms.Popups;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class UpdateSearchDialog : BaseDialog
    {
        private LocalizationProperties _lp;

        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            cancelButton.Text = _lp.CancelButtonText;
            headerLabel.Text = _lp.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            _lp = LocalizationHelper.GetLocalizationProperties(Updater.LanguageCulture, Updater.LocalizationFilePaths);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public void Fail(Exception ex)
        {
            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                PopupButtons.Ok)));
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }
    }
}