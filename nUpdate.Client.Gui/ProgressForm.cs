using System;
using System.Windows.Forms;
using nUpdate.Client.GuiInterface;

namespace nUpdate.Client.GUI
{
    public partial class ProgressForm : Form, IProgressReporter
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void Fail(string infoMessage, string errorMessage)
        {
            MessageBox.Show("Failed");
        }

        public void Initialize()
        {
            if (progressBar1.InvokeRequired)
                progressBar1.Invoke(new Action(() => { Initialize(); }));
            Show();
        }

        public void Terminate()
        {
            if (progressBar1.InvokeRequired)
                progressBar1.Invoke(new Action(() => { Close(); }));
            MessageBox.Show("Finished");
            Close();
        }


        public void ReportProgress(int progress, string currentFile)
        {
            throw new NotImplementedException();
        }

        public void ReportProgress(int progress, int maximum)
        {
            if (progressBar1.InvokeRequired)
                progressBar1.Invoke(new Action(() => { ReportProgress(progress, maximum); }));
            progressBar1.Maximum = maximum;
            progressBar1.Value = progress;
        }
    }
}