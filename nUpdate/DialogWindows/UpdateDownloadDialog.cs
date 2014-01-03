using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace nUpdate.DialogWindows
{
    public partial class UpdateDownloadDialog : Form
    {
        public UpdateDownloadDialog()
        {
            InitializeComponent();
        }

        // A webclient for the update downloader
        private WebClient updateDownloader = new WebClient();

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        public Uri UpdatePackageUrl;

        private void UpdateDownloadDialog_Load(object sender, EventArgs e)
        {
            btn_further.Enabled = false;

            this.Text = Application.ProductName;
            this.Icon = AppIcon;

            pictureBox1.Image = AppIcon.ToBitmap();
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            btn_cancel.Focus();

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            updateDownloader.CancelAsync();
            DialogResult = DialogResult.Cancel;
        }


        private void UpdateDownloadDialog_Shown(object sender, EventArgs e)
        {
            new System.Threading.Thread(DoWork).Start();
        }

        private void DoWork()
        {
            try
            {
                updateDownloader.DownloadFile(UpdatePackageUrl, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\nUpdate\" + Application.ProductName + "Update.zip");
              
                Invoke(new Action(() =>
                {
                    btn_further.Enabled = true;
                    lbl_search.Text = "Die Updates wurden geladen...";
                    lbl_info.Text = "Sie können das Paket nun installieren";
                    lbl_status.Text = "Das Updatepaket wurde heruntergeladen.";
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = 100;
                }));
            }

            catch
            {
                return;
            }
        }

        private void btn_further_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
