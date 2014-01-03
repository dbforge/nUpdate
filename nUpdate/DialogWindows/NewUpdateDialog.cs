using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace nUpdate.DialogWindows
{
    public partial class NewUpdateDialog : Form
    {
        public NewUpdateDialog()
        {
            InitializeComponent();
        }

        #region "Properties"

        public string AvailableVersion { set; get; }
        public string CurrentVersion { set; get; }
        public double PackageSize { set; get; }
        public string ChangelogText { set; get; }
        public bool CancelButtonIsEnabled { set; get; }

        #endregion

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        const Int32 BCM_SETSHIELD = 0x160C;

        public static void AddShieldToButton(Button btn)
        {
            const Int32 BCM_SETSHIELD = 0x160C;

            btn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            SendMessage(btn.Handle, BCM_SETSHIELD, 0, 1);

        }


        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btn_install_Click(object sender, EventArgs e)
        {
            CancelButtonIsEnabled = false;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void NewUpdateDialog_Load(object sender, EventArgs e)
        {
            btn_install.Focus();
            lbl_newestversion.Text = "Neueste Version: " + AvailableVersion;
            lbl_actualversion.Text = "Aktuelle Version: " + CurrentVersion;
            lbl_info.Text = "Es können Updates für " + Application.ProductName + " geladen werden.";

            const int MB = 1048576;
            const int KB = 1024;

            if (PackageSize >= 104857.6)
            {
                var PackageSizeInMB = (PackageSize / MB);
                lbl_updatesize.Text = "Updategröße: " + Convert.ToString(Math.Round(PackageSizeInMB, 1)) + " MB";
            }

            else
            {

                var PackageSizeInKB = (PackageSize / KB);
                lbl_updatesize.Text = "Updategröße: " + Convert.ToString(Math.Round(PackageSizeInKB, 1)) + " KB";
            }

            lbl_changelog.Text = "Changelog: ";

            this.Icon = AppIcon;
            this.Text = Application.ProductName;
            pictureBox1.Image = AppIcon.ToBitmap();
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;

            txt_changelog.Text = ChangelogText;

            AddShieldToButton(btn_install);
        }

        private void txt_changelog_MouseEnter(object sender, EventArgs e)
        {
            btn_install.Focus();
        }

        private void txt_changelog_Enter(object sender, EventArgs e)
        {
            btn_install.Focus();
        }

    }
}
