// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.Core.Operations;
using nUpdate.Core.Win32;
using nUpdate.Internal;

namespace nUpdate.UI.Dialogs
{
    public partial class NewUpdateDialog : BaseDialog
    {
        private const float GB = 1073741824;
        private const int MB = 1048576;
        private const int KB = 1024;
        private bool _allowCancel;
        private LocalizationProperties _lp;
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

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
        ///     Sets the available version.
        /// </summary>
        public UpdateVersion UpdateVersion { get; set; }

        /// <summary>
        ///     Sets the current version.
        /// </summary>
        public UpdateVersion CurrentVersion { get; set; }

        /// <summary>
        ///     Sets the size of the package.
        /// </summary>
        public double PackageSize { get; set; }

        /// <summary>
        ///     Sets the changelog.
        /// </summary>
        public string Changelog { get; set; }

        /// <summary>
        ///     Sets if this update must be installed.
        /// </summary>
        public bool MustUpdate { get; set; }

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
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json", LanguageName);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }
            else if (String.IsNullOrEmpty(LanguageFilePath) && LanguageName == "en")
            {
                _lp = new LocalizationProperties();
            }

            headerLabel.Text = _lp.NewUpdateDialogHeader;
            infoLabel.Text = String.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);
            newestVersionLabel.Text = String.Format(_lp.NewUpdateDialogNewestVersionText, UpdateVersion.FullText);
            currentVersionLabel.Text = String.Format(_lp.NewUpdateDialogCurrentVersionText, CurrentVersion);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;

            if (PackageSize >= 107374182.4)
            {
                double packageSizeInMb = Math.Round((PackageSize/GB), 1);
                updateSizeLabel.Text = String.Format("{0} {1}",
                    String.Format(_lp.NewUpdateDialogSizeText, packageSizeInMb), "GB");
            }
            else if (PackageSize >= 104857.6)
            {
                double packageSizeInMb = Math.Round((PackageSize/MB), 1);
                updateSizeLabel.Text = String.Format("{0} {1}",
                    String.Format(_lp.NewUpdateDialogSizeText, packageSizeInMb), "MB");
            }
            else if (PackageSize >= 102.4)
            {
                double packageSizeInKb = Math.Round((PackageSize/KB), 1);
                updateSizeLabel.Text = String.Format("{0} {1}",
                    String.Format(_lp.NewUpdateDialogSizeText, packageSizeInKb), "KB");
            }
            else if (PackageSize >= 1)
            {
                updateSizeLabel.Text = String.Format("{0} {1}",
                    String.Format(_lp.NewUpdateDialogSizeText, PackageSize), "B");
            }

            Icon = _appIcon;
            Text = Application.ProductName;
            iconPictureBox.Image = _appIcon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            changelogTextBox.Text = Changelog;
            AddShieldToButton(installButton);

            if (MustUpdate)
                cancelButton.Enabled = false;
            else
                _allowCancel = true;

            if (OperationAreas == null || OperationAreas.Count == 0)
            {
                accessLabel.Text = String.Format("{0} -", _lp.NewUpdateDialogAccessText);
                return;
            }

            accessLabel.Text = String.Format("{0} {1}", _lp.NewUpdateDialogAccessText,
                String.Join(", ",
                    LocalizationHelper.GetLocalizedEnumerationValues(_lp, OperationAreas.Cast<object>().ToArray())));
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