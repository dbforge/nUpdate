using nUpdate.Client.GuiInterface;
using nUpdate.UpdateInstaller;
using System;
using System.Windows.Forms;

namespace nUpdate
{
    public partial class MainForm : Form, IProgressReporter
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            Show();
        }

        public void ReportProgress(int progress, string currentFile)
        {
            this.extractProgressBar.Value = progress;
            this.unpackingLabel.Text = String.Format("{0} {1}... {2}", "Unpacking...", currentFile, progress);
        }

        public void Fail(string infoMessage, string errorMessage)
        {
            var errorDialog = new ErrorDialog();
            errorDialog.InfoMessage = infoMessage;
            errorDialog.ErrorMessage = errorMessage;
        }

        public void Terminate()
        {
            Close();
        }
    }
}
