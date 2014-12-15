using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using nUpdate.Core;
using nUpdate.Core.Localization;
using nUpdate.Core.Operations;
using nUpdate.Dialogs;
using nUpdate.Internal;

namespace nUpdate.UI.Dialogs
{
    public partial class NewUpdateDialog : BaseForm
    {
        private readonly Icon _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        private bool _allowCancel;
        private LocalizationProperties _lp; 

        public NewUpdateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets the name of the _lpuage file in the resources to use, if no own file is used.
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        ///     Sets the path of the file which contains the specific _lpuage content a user added on its own.
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

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        internal static void AddShieldToButton(Button btn)
        {
            const Int32 bcmSetshield = 0x160C;

            btn.FlatStyle = FlatStyle.System;
            SendMessage(btn.Handle, bcmSetshield, 0, 1);
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
                    /*string resourceName = "nUpdate.Core.Localization.en.json";
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                    {
                        _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                    }*/

                    _lp = new LocalizationProperties();
                }
            }
            else
            {
                string resourceName = String.Format("nUpdate.Core.Localization.{0}.json", LanguageName);
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            headerLabel.Text = _lp.NewUpdateDialogHeader;
            infoLabel.Text = String.Format(_lp.NewUpdateDialogInfoText, Application.ProductName);
            newestVersionLabel.Text = String.Format(newestVersionLabel.Text, UpdateVersion.FullText);
            currentVersionLabel.Text = String.Format(_lp.NewUpdateDialogCurrentVersionText, CurrentVersion);
            changelogLabel.Text = _lp.NewUpdateDialogChangelogText;
            cancelButton.Text = _lp.CancelButtonText;
            installButton.Text = _lp.InstallButtonText;

            const int mb = 1048576;
            const int kb = 1024;

            if (PackageSize == -1)
                updateSizeLabel.Text = String.Format(updateSizeLabel.Text, "N/A");
            else if (PackageSize >= 104857.6)
            {
                double packageSizeInMb = Math.Round((PackageSize/mb), 1);
                updateSizeLabel.Text = String.Format("{0} {1}",
                    String.Format(_lp.NewUpdateDialogSizeText, packageSizeInMb), "MB");
            }
            else
            {
                double packageSizeInKb = Math.Round((PackageSize/kb), 1);
                updateSizeLabel.Text = String.Format("{0} {1}",
                    String.Format(_lp.NewUpdateDialogSizeText, packageSizeInKb), "KB");
            }

            Icon = _appIcon;
            Text = Application.ProductName;
            iconPictureBox.Image = _appIcon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            changelogTextBox.Text = Changelog;
            AddShieldToButton(installButton);

            if (MustUpdate)
                cancelButton.Enabled = false;

            var accessStrings = new List<string>();

            if (OperationAreas == null || OperationAreas.Count == 0)
            {
                accessLabel.Text += " -";
                return;
            }

            if (OperationAreas.Any(item => item == OperationArea.Files))
            {
                accessStrings.Add(_lp.NewUpdateDialogDemandsFilesAccessText);
            }

            if (OperationAreas.Any(item => item == OperationArea.Registry))
            {
                accessStrings.Add(_lp.NewUpdateDialogDemandsRegistryAccessText);
            }

            if (OperationAreas.Any(item => item == OperationArea.Processes))
            {
                accessStrings.Add(_lp.NewUpdateDialogDemandsProcessesAccessText);
            }

            if (OperationAreas.Any(item => item == OperationArea.Files))
            {
                accessStrings.Add(_lp.NewUpdateDialogDemandsServicesAccessText);
            }
            
            accessLabel.Text += String.Format("{0} {1}", " ", String.Join(", ", accessStrings));
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            _allowCancel = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void NewUpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MustUpdate && !_allowCancel)
                e.Cancel = true;
        }
    }
}