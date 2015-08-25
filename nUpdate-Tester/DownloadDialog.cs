using System;
using System.Windows.Forms;
using nUpdate.UpdateEventArgs;

namespace nUpdate.Tester
{
    public partial class DownloadDialog : Form
    {
        public DownloadDialog()
        {
            InitializeComponent();
        }

        public void ProgressChanged(object sender, UpdateDownloadProgressChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                progressBar1.Value = (int) e.Percentage;
                label3.Text = e.Percentage + "%";
            }));
        }

        public void Finish(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public void Fail(object sender, FailedEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }

        private void DownloadDialog_Load(object sender, EventArgs e)
        {
        }
    }
}