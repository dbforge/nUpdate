using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.UpdateInstaller;
using System.Diagnostics;
using nUpdate.Client.GuiInterface;

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
            extractProgressBar.Value = progress;
            unpackingLabel.Text = String.Format("{0} {1}... {2}", "Unpacking...", currentFile, progress);
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
