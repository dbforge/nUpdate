// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Localization;
using nUpdate.Operations;
using nUpdate.Updating;
using nUpdate.Win32;

namespace nUpdate.UI.Dialogs
{
    public partial class NewUpdateDialog : BaseDialog
    {
        private const float GB = 1073741824;
        private const int MB = 1048576;
        private const int KB = 1024;
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
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
            const Int32 bcmSetshield = 0x160C;

            btn.FlatStyle = FlatStyle.System;
            NativeMethods.SendMessage(btn.Handle, bcmSetshield, new IntPtr(0), new IntPtr(1));
        }

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(LanguageFilePath))
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
            else if (String.IsNullOrEmpty(LanguageFilePath) && LanguageName != "en")
            {
                string resourceName = $"nUpdate.Localization.{LanguageName}.json";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (String.IsNullOrEmpty(LanguageFilePath) && LanguageName == "en")
            {
                _lp = new LocalizationProperties();
            }

            headerLabel.Text =
                String.Format(
                    PackageConfigurations.Count() > 1
                        ? _lp.NewUpdateDialogMultipleUpdatesHeader
                        : _lp.NewUpdateDialogSingleUpdateHeader, PackageConfigurations.Count());
            infoLabel.Text = String.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);
            var availableVersions =
                PackageConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)).ToArray();
            newestVersionLabel.Text = String.Format(_lp.NewUpdateDialogAvailableVersionsText,
                PackageConfigurations.Count() <= 2
                    ? String.Join(", ", availableVersions.Select(item => item.FullText))
                    : $"{UpdateVersion.GetLowestUpdateVersion(availableVersions).FullText} - {UpdateVersion.GetHighestUpdateVersion(availableVersions).FullText}");
            currentVersionLabel.Text = String.Format(_lp.NewUpdateDialogCurrentVersionText, CurrentVersion.FullText);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;

            if (PackageSize >= 107374182.4)
            {
                double packageSizeInMb = Math.Round((PackageSize/GB), 1);
                updateSizeLabel.Text = $"{String.Format(_lp.NewUpdateDialogSizeText, packageSizeInMb)} {"GB"}";
            }
            else if (PackageSize >= 104857.6)
            {
                double packageSizeInMb = Math.Round((PackageSize/MB), 1);
                updateSizeLabel.Text = $"{String.Format(_lp.NewUpdateDialogSizeText, packageSizeInMb)} {"MB"}";
            }
            else if (PackageSize >= 102.4)
            {
                double packageSizeInKb = Math.Round((PackageSize/KB), 1);
                updateSizeLabel.Text = $"{String.Format(_lp.NewUpdateDialogSizeText, packageSizeInKb)} {"KB"}";
            }
            else if (PackageSize >= 1)
            {
                updateSizeLabel.Text = $"{String.Format(_lp.NewUpdateDialogSizeText, PackageSize)} {"B"}";
            }

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

                changelogTextBox.Text +=
                    String.Format(String.IsNullOrEmpty(changelogTextBox.Text) ? "{0}:\n{1}" : "\n\n{0}:\n{1}",
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
                $"{_lp.NewUpdateDialogAccessText} {String.Join(", ", LocalizationHelper.GetLocalizedEnumerationValues(_lp, OperationAreas.Cast<object>().GroupBy(item => item).Select(item => item.First()).ToArray()))}";
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
            if (!_allowCancel)
                e.Cancel = true;
        }

        private void changelogTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}