// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.Win32;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class NewUpdateDialog : BaseDialog
    {
        private bool _allowCancel;
        private LocalizationProperties _lp;

        public NewUpdateDialog()
        {
            InitializeComponent();
        }

        internal static void AddShieldToButton(Button button)
        {
            const int bcmSetshield = 0x160C;

            button.FlatStyle = FlatStyle.System;
            NativeMethods.SendMessage(button.Handle, bcmSetshield, new IntPtr(0), new IntPtr(1));
        }

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            _lp = LocalizationHelper.GetLocalizationProperties(InteractionUpdater.LanguageCulture, InteractionUpdater.LocalizationFilePaths);

            headerLabel.Text =
                string.Format(
                    InteractionUpdater.FilteredUpdatePackageCollection.Count() > 1
                        ? _lp.NewUpdateDialogMultipleUpdatesHeader
                        : _lp.NewUpdateDialogSingleUpdateHeader, InteractionUpdater.FilteredUpdatePackageCollection.Count());
            infoLabel.Text = string.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);

            var availableVersions =
                InteractionUpdater.FilteredUpdatePackageCollection.Select(item => new UpdateVersion(item.LiteralVersion)).ToArray();
            newestVersionLabel.Text = string.Format(_lp.NewUpdateDialogAvailableVersionsText,
                InteractionUpdater.FilteredUpdatePackageCollection.Count() <= 2
                    ? string.Join(", ", availableVersions.Select(item => item.Description))
                    : $"{UpdateVersion.GetLowestUpdateVersion(availableVersions).Description} - {UpdateVersion.GetHighestUpdateVersion(availableVersions).Description}");
            currentVersionLabel.Text = string.Format(_lp.NewUpdateDialogCurrentVersionText, InteractionUpdater.CurrentVersion.Description);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;
            updateSizeLabel.Text = string.Format(_lp.NewUpdateDialogSizeText, SizeHelper.ToAdequateSizeString((long)InteractionUpdater.TotalSize));
            
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
            if (Icon != null)
                iconPictureBox.Image = Icon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;
            AddShieldToButton(installButton);

            foreach (var updateData in InteractionUpdater.FilteredUpdatePackageCollection)
            {
                var versionText = new UpdateVersion(updateData.LiteralVersion).Description;
                var changelogText = updateData.Changelog.ContainsKey(InteractionUpdater.LanguageCulture)
                    ? updateData.Changelog.First(item => Equals(item.Key, InteractionUpdater.LanguageCulture)).Value
                    : updateData.Changelog.First(item => Equals(item.Key, new CultureInfo("en"))).Value;

                changelogTextBox.Text +=
                    string.Format(string.IsNullOrEmpty(changelogTextBox.Text) ? "{0}:\n{1}" : "\n\n{0}:\n{1}",
                        versionText, changelogText);
            }

            var operationAreas =
                InteractionUpdater.FilteredUpdatePackageCollection.Select(item => item.Operations.Select(op => op.Area)).ToList();
            if (!operationAreas.Any())
            {
                accessLabel.Text = $"{_lp.NewUpdateDialogAccessText} -";
                _allowCancel = true;
                return;
            }

            accessLabel.Text =
                $"{_lp.NewUpdateDialogAccessText} {string.Join(", ", LocalizationHelper.GetLocalizedEnumerationValues(_lp, operationAreas.Cast<object>().GroupBy(item => item).Select(item => item.First()).ToArray()))}";
            _allowCancel = true;
        }

        public void ShowModalDialog(object dialogResultReference)
        {
            ((DialogResultReference) dialogResultReference).DialogResult = ShowDialog();
        }

        public void CloseDialog(object state)
        {
            Close();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            _allowCancel = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void NewUpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_allowCancel;
        }

        private void changelogTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}