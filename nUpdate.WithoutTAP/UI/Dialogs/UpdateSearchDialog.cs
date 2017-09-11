// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.Internal.UI.Popups;
using nUpdate.UpdateEventArgs;

namespace nUpdate.UI.Dialogs
{
    internal partial class UpdateSearchDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        internal UpdateSearchDialog()
        {
            InitializeComponent();
        }

        internal bool UpdatesFound { get; set; }

        private void Cancel()
        {
            UpdateManager.CancelSearchAsync();
            DialogResult = DialogResult.Cancel;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public void Failed(object sender, FailedEventArgs e)
        {
            Invoke(new Action(() =>
                Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption,
                    e.Exception.InnerException ?? e.Exception,
                    PopupButtons.Ok)));
            DialogResult = DialogResult.Cancel;
        }

        public void Finished(object sender, UpdateSearchFinishedEventArgs e)
        {
            UpdatesFound = e.UpdatesAvailable;
            DialogResult = DialogResult.OK;
        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            cancelButton.Text = _lp.CancelButtonText;
            headerLabel.Text = _lp.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            Icon = _appIcon;

            UpdateManager.UpdateSearchFailed += Failed;
            UpdateManager.UpdateSearchFinished += Finished;
        }

        private void UpdateSearchDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                /* Important to unsubscribe the events, otherwise any calls on the UI thread (Invoke) 
                   will cause an InvalidOperationException as the window handle is no longer available. */

                UpdateManager.UpdateSearchFailed -= Failed;
                UpdateManager.UpdateSearchFinished -= Finished;

                return;
            }

            e.Cancel = true;
            Cancel();
        }

        private void UpdateSearchDialog_Shown(object sender, EventArgs e)
        {
            UpdateManager.SearchForUpdatesAsync();
        }
    }
}