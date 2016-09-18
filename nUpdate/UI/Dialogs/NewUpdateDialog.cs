// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        ///     Sets the name of the language file in the resources to use, if no own file is used.
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        ///     Sets the path of the file which contains the specific language content a user added on its own.
        /// </summary>
        public string LanguageFilePath { get; set; }

        /// <summary>
        ///     Gets or sets the package configurations.
        /// </summary>
        public IEnumerable<UpdateConfiguration> PackageConfigurations { get; set; }

        /// <summary>
        ///     Sets the current version.
        /// </summary>
        public UpdateVersion CurrentVersion { get; set; }

        /// <summary>
        ///     Gets or sets the size of all packages.
        /// </summary>
        public double PackageSize { get; set; }

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

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LanguageFilePath))
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
            else if (string.IsNullOrEmpty(LanguageFilePath) && LanguageName != "en")
            {
                string resourceName = $"nUpdate.Core.Localization.{LanguageName}.json";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (string.IsNullOrEmpty(LanguageFilePath) && LanguageName == "en")
            {
                _lp = new LocalizationProperties();
            }

            headerLabel.Text =
                string.Format(
                    PackageConfigurations.Count() > 1
                        ? _lp.NewUpdateDialogMultipleUpdatesHeader
                        : _lp.NewUpdateDialogSingleUpdateHeader, PackageConfigurations.Count());
            infoLabel.Text = string.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);
            var availableVersions =
                PackageConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)).ToArray();
            newestVersionLabel.Text = string.Format(_lp.NewUpdateDialogAvailableVersionsText,
                PackageConfigurations.Count() <= 2
                    ? string.Join(", ", availableVersions.Select(item => item.FullText))
                    : $"{UpdateVersion.GetLowestUpdateVersion(availableVersions).FullText} - {UpdateVersion.GetHighestUpdateVersion(availableVersions).FullText}");
            currentVersionLabel.Text = string.Format(_lp.NewUpdateDialogCurrentVersionText, CurrentVersion.FullText);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;

            var size = SizeHelper.ConvertSize(PackageSize);
            updateSizeLabel.Text = $"{string.Format(_lp.NewUpdateDialogSizeText, size.Item1)} {size.Item2}";

            Icon = _appIcon;
            Text = Application.ProductName;
            iconPictureBox.Image = _appIcon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            foreach (var updateConfiguration in PackageConfigurations)
            {
                var versionText = new UpdateVersion(updateConfiguration.LiteralVersion).FullText;
                var changelogText = updateConfiguration.Changelog.ContainsKey(new CultureInfo(LanguageName))
                    ? updateConfiguration.Changelog.First(item => item.Key.Name == LanguageName).Value
                    : updateConfiguration.Changelog.First(item => item.Key.Name == "en").Value;

                if (PackageSize > GB)
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

        public void CloseDialog(object state)
        {
            Close();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            double necessarySpaceToFree;
            if (!SizeHelper.HasEnoughSpace(PackageSize, out necessarySpaceToFree))
            {
                var packageSizeData = SizeHelper.ConvertSize(PackageSize);
                var spaceToFreeData = SizeHelper.ConvertSize(necessarySpaceToFree);
                Popup.ShowPopup(this, SystemIcons.Warning, "Not enough disk space.",
                    $"You don't have enough disk space left on your drive and nUpdate is not able to download and install the available updates ({packageSizeData.Item1} {packageSizeData.Item2}). Please free a minimum of {spaceToFreeData.Item1} {spaceToFreeData.Item2} to make sure the updates can be downloaded and installed without any problems.",
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

        private void changelogTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}