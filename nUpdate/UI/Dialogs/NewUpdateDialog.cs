using nUpdate.Core;
using nUpdate.Core.Language;
using nUpdate.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Dialogs
{
    public partial class NewUpdateDialog : BaseForm
    {
        private bool allowCancel = false;
        public Language Language { get; set; }
        public string LanguageFilePath { get; set; }

        public NewUpdateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the available version.
        /// </summary>
        public UpdateVersion UpdateVersion { get; set; }

        /// <summary>
        /// Sets the current version.
        /// </summary>
        public UpdateVersion CurrentVersion { get; set; }

        /// <summary>
        /// Sets the size of the package.
        /// </summary>
        public double PackageSize { get; set; }

        /// <summary>
        /// Sets the changelog.
        /// </summary>
        public string Changelog { get; set; }

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

            if (this.Language != Language.Custom)
            {
                switch (this.Language)
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
                    case Language.Russian:
                        resourceName += "ru.xml";
                        break;
                }
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    lang = LanguageSerializer.ReadXml(stream);
                }
            }
            else
            {
                if (File.Exists(this.LanguageFilePath))
                {
                    lang = LanguageSerializer.ReadXml(this.LanguageFilePath);
                }
                else
                {
                    this.infoLabel.Text = String.Format(this.infoLabel.Text, Application.ProductName);
                    this.newestVersionLabel.Text = String.Format(this.newestVersionLabel.Text, this.UpdateVersion.FullText);
                    this.currentVersionLabel.Text = String.Format(this.currentVersionLabel.Text, this.CurrentVersion);
                }
            }

            this.headerLabel.Text = lang.NewUpdateDialogHeader;
            this.infoLabel.Text = String.Format(lang.NewUpdateDialogInfoText, Application.ProductName);
            this.newestVersionLabel.Text = String.Format(this.newestVersionLabel.Text, this.UpdateVersion.FullText);
            this.currentVersionLabel.Text = String.Format(lang.NewUpdateDialogCurrentVersionText, this.CurrentVersion);
            this.changelogLabel.Text = lang.NewUpdateDialogChangelogText;
            this.cancelButton.Text = lang.CancelButtonText;
            this.installButton.Text = lang.InstallButtonText;

            const int Mb = 1048576;
            const int Kb = 1024;

            if (this.PackageSize == -1)
            {
                this.updateSizeLabel.Text = String.Format(this.updateSizeLabel.Text, "N/A");
            }
            else if (this.PackageSize >= 104857.6)
            {
                double PackageSizeInMb = Math.Round((this.PackageSize / Mb), 1);
                this.updateSizeLabel.Text = String.Format("{0} {1}", String.Format(lang.NewUpdateDialogSizeText, PackageSizeInMb), "MB");
            }
            else
            {
                double PackageSizeInKb = Math.Round((this.PackageSize / Kb), 1);
                this.updateSizeLabel.Text = String.Format("{0} {1}", String.Format(lang.NewUpdateDialogSizeText, PackageSizeInKb), "KB");
            }

            this.Icon = this.AppIcon;
            this.Text = Application.ProductName;
            this.iconPictureBox.Image = this.AppIcon.ToBitmap();
            this.iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            this.changelogTextBox.Text = this.Changelog;
            AddShieldToButton(installButton);

            if (this.MustUpdate)
            {
                cancelButton.Enabled = false;
            }
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            allowCancel = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void NewUpdateDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.MustUpdate && !allowCancel)
            {
                e.Cancel = true;
            }
        }
    }
}
