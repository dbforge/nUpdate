// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using nUpdate.UI.WinForms.Win32;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class NewUpdateDialog : BaseDialog
    {
        private bool _allowCancel;

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

        internal UpdateResult UpdateResult { get; set; }

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = UpdateProvider.LanguageCulture;

            headerLabel.Text =
                string.Format(
                    UpdateResult.Packages.Count() > 1
                        ? Properties.strings.NewUpdateDialogMultipleUpdatesHeader
                        : Properties.strings.NewUpdateDialogSingleUpdateHeader, UpdateResult.Packages.Count());
            infoLabel.Text = string.Format(Properties.strings.NewUpdateDialogInfoText, Application.ProductName);

            var availablePackages =
                UpdateResult.Packages.ToArray();
            newestVersionLabel.Text = string.Format(Properties.strings.NewUpdateDialogAvailableVersionsText,
                UpdateResult.Packages.Count() <= 2
                    ? string.Join(", ", availablePackages.Select(x => x.Version))
                    : $"{availablePackages.First().Version} - {availablePackages.Last().Version}");
            currentVersionLabel.Text = string.Format(Properties.strings.NewUpdateDialogCurrentVersionText, UpdateProvider.ApplicationVersion);
            changelogLabel.Text = Properties.strings.NewUpdateDialogChangelogText;
            cancelButton.Text = Properties.strings.CancelButtonText;
            installButton.Text = Properties.strings.InstallButtonText;
            updateSizeLabel.Text = string.Format(Properties.strings.NewUpdateDialogSizeText, UpdateResult.Packages.Sum(x => x.Size).ToAdequateSizeString());
            
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
            if (Icon != null)
                iconPictureBox.Image = Icon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;
            AddShieldToButton(installButton);

            foreach (var updatePackage in UpdateResult.Packages)
            {
                var changelogText = updatePackage.Changelog.ContainsKey(UpdateProvider.LanguageCulture)
                    ? updatePackage.Changelog.First(item => Equals(item.Key, UpdateProvider.LanguageCulture)).Value
                    : updatePackage.Changelog.First(item => Equals(item.Key, new CultureInfo("en"))).Value;

                changelogTextBox.Text +=
                    string.Format(string.IsNullOrEmpty(changelogTextBox.Text) ? "{0}:\n{1}" : "\n\n{0}:\n{1}",
                        updatePackage.Version, changelogText);
            }
            
            _allowCancel = true;
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

        private static string GetVersionDescription(DefaultUpdatePackage package)
        {
            var versionStringBuilder = new StringBuilder(package.Version.ToString());
            if (package.ChannelName.ToLowerInvariant() != "release")
                versionStringBuilder.Append($" {package.ChannelName}");
            return versionStringBuilder.ToString();
        }
    }
}