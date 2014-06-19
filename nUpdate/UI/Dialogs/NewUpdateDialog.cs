using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using nUpdate.Core.Language;
using nUpdate.UI.Controls;

namespace nUpdate.Dialogs
{
    public partial class NewUpdateDialog : BaseForm
    {
        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

        public NewUpdateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the available version.
        /// </summary>
        public Version UpdateVersion { get; set; }

        /// <summary>
        /// Sets the current version.
        /// </summary>
        public Version CurrentVersion { get; set; }

        /// <summary>
        /// Sets the size of the package.
        /// </summary>
        public double PackageSize { get; set; }

        /// <summary>
        /// Sets the changelog.
        /// </summary>
        public string ChangelogText { get; set; }

        /// <summary>
        /// Sets if this update must be installed.
        /// </summary>
        public bool MustUpdate { get; set; }

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        const Int32 BCM_SETSHIELD = 0x160C;

        internal static void AddShieldToButton(Button btn)
        {
            const Int32 BCM_SETSHIELD = 0x160C;

            btn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            SendMessage(btn.Handle, BCM_SETSHIELD, 0, 1);
        }

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            string resourceName = "nUpdate.Core.Language.";
            LanguageSerializer lang = null;

            if (Language != Language.Custom)
            {
                switch (Language)
                {
                    case Language.English:
                        resourceName += "en.xml";
                        break;
                    case Language.German:
                        resourceName += "de.xml";
                        break;
                    case Language.French:
                        resourceName += "fr.xml";
                        break;
                    case Language.Spanish:
                        resourceName += "es.xml";
                        break;
                }
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    lang = LanguageSerializer.ReadXml(stream);
                }
            }
            else
            {
                if (File.Exists(LanguageFilePath))
                {
                    lang = LanguageSerializer.ReadXml(LanguageFilePath);
                }
                else
                {
                    infoLabel.Text = String.Format(infoLabel.Text, Application.ProductName);
                    newestVersionLabel.Text = String.Format(newestVersionLabel.Text, UpdateVersion);
                    currentVersionLabel.Text = String.Format(currentVersionLabel.Text, CurrentVersion);
                }
            }

            headerLabel.Text = lang.NewUpdateDialogHeader;
            infoLabel.Text = String.Format(lang.NewUpdateDialogInfoText, Application.ProductName);
            newestVersionLabel.Text = String.Format(lang.NewUpdateDialogNewestVersionText, UpdateVersion);
            currentVersionLabel.Text = String.Format(lang.NewUpdateDialogCurrentVersionText, CurrentVersion);
            changelogLabel.Text = lang.NewUpdateDialogChangelogText;
            cancelButton.Text = lang.CancelButtonText;
            installButton.Text = lang.InstallButtonText;

            const int MB = 1048576;
            const int KB = 1024;

            if (PackageSize == -1)
            {
                updateSizeLabel.Text = String.Format(updateSizeLabel.Text, "N/A");
            }
            else if (PackageSize >= 104857.6)
            {
                double PackageSizeInMB = Math.Round((PackageSize / MB), 1);
                updateSizeLabel.Text = String.Format("{0} {1}", String.Format(lang.NewUpdateDialogSizeText, PackageSizeInMB), "MB");
            }
            else
            {
                double PackageSizeInKB = Math.Round((PackageSize / KB), 1);
                updateSizeLabel.Text = String.Format("{0} {1}", String.Format(lang.NewUpdateDialogSizeText, PackageSizeInKB), "KB");
            }

            this.Icon = AppIcon;
            this.Text = Application.ProductName;
            iconPictureBox.Image = AppIcon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            changelogTextBox.Text = ChangelogText;
            AddShieldToButton(installButton);

            if (MustUpdate)
                cancelButton.Enabled = false;
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void NewUpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MustUpdate)
                e.Cancel = true;
        }
    }
}
