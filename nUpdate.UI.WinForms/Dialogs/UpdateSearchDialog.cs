// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using nUpdate.UI.WinForms.Popups;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class UpdateSearchDialog : BaseDialog
    {
        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        public UpdateResult Result { get; set; }

        private void Cancel()
        {
            UpdateProvider.CancelUpdateCheck();
            DialogResult = DialogResult.Cancel;
        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = UpdateProvider.LanguageCulture;

            cancelButton.Text = Properties.strings.CancelButtonText;
            headerLabel.Text = Properties.strings.UpdateSearchDialogHeader;

            Text = Application.ProductName;
        }

        private async void SearchDialog_Shown(object sender, EventArgs e)
        {
            try
            {
                Result = await UpdateProvider.BeginUpdateCheck();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, Properties.strings.UpdateSearchErrorCaption, ex,
                    PopupButtons.Ok);
                DialogResult = DialogResult.Cancel;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void UpdateSearchDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            e.Cancel = true;
            Cancel();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}