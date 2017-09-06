// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.UI.Popups;
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            UpdateManager.CancelSearch();
            DialogResult = DialogResult.Cancel;
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

        private void UpdateSearchDialog_Shown(object sender, EventArgs e)
        {
            UpdateManager.SearchForUpdatesAsync();
        }
    }
}