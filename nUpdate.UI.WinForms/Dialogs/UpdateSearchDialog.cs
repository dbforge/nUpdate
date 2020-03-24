// UpdateSearchDialog.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Properties;
using nUpdate.UI.WinForms.Popups;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class UpdateSearchDialog : BaseDialog
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        private void Cancel()
        {
            _cancellationTokenSource.Cancel();
            DialogResult = DialogResult.Cancel;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = UpdateProvider.LanguageCulture;

            cancelButton.Text = strings.CancelButtonText;
            headerLabel.Text = strings.UpdateSearchDialogHeader;

            Text = Application.ProductName;
        }

        private async void SearchDialog_Shown(object sender, EventArgs e)
        {
            try
            {
                UpdateCheckResult = await UpdateProvider.CheckForUpdates(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, strings.UpdateSearchErrorCaption, ex,
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
    }
}