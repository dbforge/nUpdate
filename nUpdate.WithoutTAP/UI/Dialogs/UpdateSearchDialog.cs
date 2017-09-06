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
    public partial class UpdateSearchDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private LocalizationProperties _lp;

        public UpdateSearchDialog()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            OnCancelButtonClicked();
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        ///     Occurs when the cancel button is clicked.
        /// </summary>
        public event EventHandler<EventArgs> CancelButtonClicked;

        protected virtual void OnCancelButtonClicked()
        {
            if (CancelButtonClicked != null)
                CancelButtonClicked(this, EventArgs.Empty);
        }

        private void SearchDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(Updater.LanguageCulture, Updater.CultureFilePaths);

            cancelButton.Text = _lp.CancelButtonText;
            headerLabel.Text = _lp.UpdateSearchDialogHeader;

            Text = Application.ProductName;
            Icon = _appIcon;
        }

        #region TAP

        public void Fail(Exception ex)
        {
            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption, ex,
                PopupButtons.Ok)));
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            if (dialogResultReference != null)
                ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
            else
                ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        #endregion

        #region EAP

        public void Failed(object sender, FailedEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        Popup.ShowPopup(this, SystemIcons.Error, _lp.UpdateSearchErrorCaption,
                            e.Exception.InnerException ?? e.Exception,
                            PopupButtons.Ok)));
            DialogResult = DialogResult.Cancel;
        }

        public void Finished(object sender, UpdateSearchFinishedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}