using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Client.GuiInterface;

namespace nUpdate.UpdateInstaller.Dialogs
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
                Show(); // We currently only have an instace, so we show the form now.
            }));
        }

        public void ReportUnpackingProgress(int progress, string currentFile)
        {
            Invoke(new Action(() =>
            {
                extractProgressBar.Value = progress;
                updateLabel.Text = String.Format("{0} {1}... {2}%", "Unpacking...", currentFile, progress);
            }));
        }

        public void ReportOperationProgress(int progress, string currentOperation)
        {
            Invoke(new Action(() =>
            {
                extractProgressBar.Value = progress;
                updateLabel.Text = String.Format("{0}... {1}%", currentOperation, progress);
            }));
        }

        public bool Fail(Exception ex)
        {
            return false;
        }

        public void Terminate()
        {
            Invoke(new Action(Close));
        }
    }
}