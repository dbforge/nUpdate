using nUpdate.Client.GuiInterface;
using System;
using System.Windows.Forms;

namespace nUpdate.Client.GUI
{
    public partial class ProgressForm : Form, IProgressReporter
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void ReportProgress(int progress, int maximum)
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke(new Action(() =>
                {
                    this.ReportProgress(progress, maximum);
                }));
            }
            this.progressBar1.Maximum = maximum;
            this.progressBar1.Value = progress;
        }

        public void Fail(string infoMessage, string errorMessage)
        {
            MessageBox.Show("Failed");
        }

        public void Initialize()
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke(new Action(() =>
                {
                    this.Initialize();
                }));
            }
            Show();
        }

        public void Terminate()
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke(new Action(() =>
                {
                    this.Close();
                }));
            }
            MessageBox.Show("Fertig");
            this.Close();
        }


        public void ReportProgress(int progress, string currentFile)
        {
            throw new NotImplementedException();
        }
    }
}
