// Copyright © Dominic Beger 2017

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.Core.Operations;
using nUpdate.Core.Win32;
using nUpdate.UI.Popups;
using nUpdate.Updating;

namespace nUpdate.UI.Dialogs
{
    public partial class NewUpdateDialog : BaseDialog
    {
        private const float GB = 1073741824;
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private bool _allowCancel;
        private LocalizationProperties _lp;

        public NewUpdateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets a list of areas for this update's operations.
        /// </summary>
        public List<OperationArea> OperationAreas { get; set; }

        internal static void AddShieldToButton(Button btn)
        {
            const int bcmSetshield = 0x160C;

            btn.FlatStyle = FlatStyle.System;
            NativeMethods.SendMessage(btn.Handle, bcmSetshield, new IntPtr(0), new IntPtr(1));
        }

        private void changelogTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            double necessarySpaceToFree;
            if (!SizeHelper.HasEnoughSpace(Updater.TotalSize, out necessarySpaceToFree))
            {
                var packageSizeString = SizeHelper.ConvertSize((long)Updater.TotalSize);
                var spaceToFreeString = SizeHelper.ConvertSize((long)necessarySpaceToFree);
                Popup.ShowPopup(this, SystemIcons.Warning, "Not enough disk space.",
                    $"You don't have enough disk space left on your drive and nUpdate is not able to download and install the available updates ({packageSizeString}). Please free a minimum of {spaceToFreeString} to make sure the updates can be downloaded and installed without any problems.",
                    PopupButtons.Ok);
                return;
            }

            _allowCancel = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void NewUpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(Updater.LanguageCulture, Updater.CultureFilePaths);

            headerLabel.Text =
                string.Format(
                    Updater.PackageConfigurations.Count() > 1
                        ? _lp.NewUpdateDialogMultipleUpdatesHeader
                        : _lp.NewUpdateDialogSingleUpdateHeader, Updater.PackageConfigurations.Count());
            infoLabel.Text = string.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);
            var availableVersions =
                Updater.PackageConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)).ToArray();
            newestVersionLabel.Text = string.Format(_lp.NewUpdateDialogAvailableVersionsText,
                Updater.PackageConfigurations.Count() <= 2
                    ? string.Join(", ", availableVersions.Select(item => item.FullText))
                    : $"{UpdateVersion.GetLowestUpdateVersion(availableVersions).FullText} - {UpdateVersion.GetHighestUpdateVersion(availableVersions).FullText}");
            currentVersionLabel.Text = string.Format(_lp.NewUpdateDialogCurrentVersionText,
                Updater.CurrentVersion.FullText);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;

            var size = SizeHelper.ConvertSize((long)Updater.TotalSize);
            updateSizeLabel.Text = $"{string.Format(_lp.NewUpdateDialogSizeText, size)}";

            Icon = _appIcon;
            Text = Application.ProductName;
            iconPictureBox.Image = _appIcon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            foreach (var updateConfiguration in Updater.PackageConfigurations)
            {
                var versionText = new UpdateVersion(updateConfiguration.LiteralVersion).FullText;
                var changelogText = updateConfiguration.Changelog.ContainsKey(Updater.LanguageCulture)
                    ? updateConfiguration.Changelog.First(item => Equals(item.Key, Updater.LanguageCulture)).Value
                    : updateConfiguration.Changelog.First(item => item.Key.Name == "en").Value;

                if (Updater.TotalSize > GB)
                    changelogTextBox.Text += _lp.NewUpdateDialogBigPackageWarning;

                changelogTextBox.Text +=
                    string.Format(string.IsNullOrEmpty(changelogTextBox.Text) ? "{0}:\n{1}" : "\n\n{0}:\n{1}",
                        versionText, changelogText);
            }
            AddShieldToButton(installButton);

            if (OperationAreas == null || OperationAreas.Count == 0)
            {
                accessLabel.Text = $"{_lp.NewUpdateDialogAccessText} -";
                _allowCancel = true;
                return;
            }

            accessLabel.Text =
                $"{_lp.NewUpdateDialogAccessText} {string.Join(", ", LocalizationHelper.GetLocalizedEnumerationValues(_lp, OperationAreas.Cast<object>().GroupBy(item => item).Select(item => item.First()).ToArray()))}";
            _allowCancel = true;
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            if (dialogResultReference != null)
                ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
            else
                ShowDialog();
        }
    }
}