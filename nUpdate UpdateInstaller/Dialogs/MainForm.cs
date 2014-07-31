using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Client.GuiInterface;
using nUpdate.UpdateInstaller;

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
            Invoke(new Action(() =>
            {
                Icon = Icon.ExtractAssociatedIcon(Program.ApplicationExecutablePath);
                Show();
            }));
        }

        public void ReportProgress(int progress, string currentFile)
        {
            Invoke(new Action(() =>
            {
                extractProgressBar.Value = progress;
                unpackingLabel.Text = String.Format("{0} {1}... {2}", "Unpacking...", currentFile, progress);
            }));
        }

        public void Fail(string infoMessage, string errorMessage)
        {
            Invoke(new Action(() =>
            {
                var errorDialog = new ErrorDialog();
                errorDialog.InfoMessage = infoMessage;
                errorDialog.ErrorMessage = errorMessage;
            }));
        }

        public void Terminate()
        {
            Invoke(new Action(() => Close()));
        }
    }
}