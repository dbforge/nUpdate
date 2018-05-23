// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Localization;
using nUpdate.Internal.Core.Operations;
using nUpdate.Internal.Core.Win32;
using nUpdate.Internal.UI.Popups;
using nUpdate.Updating;

namespace nUpdate.UI.Dialogs
{
    internal partial class NewUpdateDialog : BaseDialog
    {
        private readonly Icon _appIcon = IconHelper.ExtractAssociatedIcon(Application.ExecutablePath);
        private bool _allowCancel;
        private LocalizationProperties _lp;

        internal NewUpdateDialog()
        {
            InitializeComponent();
        }

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

        private void installButton_Click(object sender, EventArgs e)
        {
            double necessarySpaceToFree;
            if (!SizeHelper.HasEnoughSpace(UpdateManager.TotalSize, out necessarySpaceToFree))
            {
                var packageSizeString = SizeHelper.ConvertSize((long) UpdateManager.TotalSize);
                var spaceToFreeString = SizeHelper.ConvertSize((long) necessarySpaceToFree);
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
            _lp = LocalizationHelper.GetLocalizationProperties(UpdateManager.LanguageCulture,
                UpdateManager.CultureFilePaths);

            headerLabel.Text =
                string.Format(
                    UpdateManager.PackageConfigurations.Count() > 1
                        ? _lp.NewUpdateDialogMultipleUpdatesHeader
                        : _lp.NewUpdateDialogSingleUpdateHeader, UpdateManager.PackageConfigurations.Count());
            infoLabel.Text = string.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);
            var availableVersions =
                UpdateManager.PackageConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)).ToArray();
            newestVersionLabel.Text = string.Format(_lp.NewUpdateDialogAvailableVersionsText,
                UpdateManager.PackageConfigurations.Count() <= 2
                    ? string.Join(", ", availableVersions.Select(item => item.FullText))
                    : $"{UpdateVersion.GetLowestUpdateVersion(availableVersions).FullText} - {UpdateVersion.GetHighestUpdateVersion(availableVersions).FullText}");
            currentVersionLabel.Text = string.Format(_lp.NewUpdateDialogCurrentVersionText,
                UpdateManager.CurrentVersion.FullText);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;

            var size = SizeHelper.ConvertSize((long) UpdateManager.TotalSize);
            updateSizeLabel.Text = $"{string.Format(_lp.NewUpdateDialogSizeText, size)}";

            Icon = _appIcon;
            Text = Application.ProductName;
            iconPictureBox.Image = _appIcon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            foreach (var updateConfiguration in UpdateManager.PackageConfigurations)
            {
                var versionText = new UpdateVersion(updateConfiguration.LiteralVersion).FullText;
                var changelogText = updateConfiguration.Changelog.ContainsKey(UpdateManager.LanguageCulture)
                    ? updateConfiguration.Changelog.First(item => Equals(item.Key, UpdateManager.LanguageCulture)).Value
                    : updateConfiguration.Changelog.First(item => item.Key.Name == "en").Value;

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
    }
}