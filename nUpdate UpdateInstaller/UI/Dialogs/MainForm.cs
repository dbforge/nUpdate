using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.UI.Popups;

namespace nUpdate.UpdateInstaller.UI.Dialogs
{
    public partial class MainForm : Form, IProgressReporter
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            Icon = Icon.ExtractAssociatedIcon(Program.ApplicationExecutablePath);
            Show(); // We currently only have an instance, so we show the form now.
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
            var result = DialogResult.None;
            Invoke(
                new Action(
                    () =>
                        result =
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while updating the application.", String.Format("{0}. Should the updating be cancelled?", ex),
                                PopupButtons.YesNo)));
            return result == DialogResult.Yes;
        }

        public void Terminate()
        {
            Invoke(new Action(Close));
        }
    }
}