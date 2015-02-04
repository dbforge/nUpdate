// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.UI.Popups;

namespace nUpdate.UpdateInstaller.UI.Dialogs
{
    public partial class MainForm : Form, IProgressReporter
    {
        private bool _allowCancel;
        private bool _dataSet;

        public MainForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            Icon = Icon.ExtractAssociatedIcon(Program.ApplicationExecutablePath);
            Text = Program.AppName;
            copyingLabel.Text = Program.ExtractFilesText;
            ShowDialog(); // We currently only have an instance, so we show the form as a modal dialog now.
        }

        public void ReportUnpackingProgress(float percentage, string currentFile)
        {
            Invoke(new Action(() =>
            {
                if (!_dataSet)
                {
                    extractProgressBar.Style = ProgressBarStyle.Blocks;
                    percentageLabel.Visible = true;
                    _dataSet = true;
                }

                extractProgressBar.Value = (int) percentage;
                copyingLabel.Text = String.Format(Program.CopyingText, currentFile);
                    // Hardcoded string because it serves as a plcaholder until the localiazation appear.
                percentageLabel.Text = String.Format("{0}%", Math.Round(percentage, 1));
            }));
        }

        public void ReportOperationProgress(float percentage, string currentOperation)
        {
            Invoke(new Action(() =>
            {
                extractProgressBar.Value += 1;
                copyingLabel.Text = String.Format("{0}", currentOperation);
                percentageLabel.Text = String.Format("{0}%", Math.Round(percentage, 1));
            }));
        }

        public bool Fail(Exception ex)
        {
            var result = DialogResult.None;
            Invoke(
                new Action(
                    () =>
                        result =
                            Popup.ShowPopup(this, SystemIcons.Error, Program.UpdatingErrorCaption,
                                String.Format(Program.UpdatingErrorText, ex),
                                PopupButtons.YesNo)));
            return result == DialogResult.Yes;
        }

        public void InitializingFail(Exception ex)
        {
            Popup.ShowPopup(this, SystemIcons.Error, Program.InitializingErrorCaption, ex,
                PopupButtons.Ok);
        }

        public void Terminate()
        {
            _allowCancel = true;
            Invoke(new Action(Close));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }
    }
}