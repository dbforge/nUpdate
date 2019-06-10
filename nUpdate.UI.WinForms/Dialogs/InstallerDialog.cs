// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.UI.WinForms.Popups;
using nUpdate.UpdateInstaller.UserInterface;

namespace nUpdate.UI.WinForms.Dialogs
{
    public partial class InstallerDialog : Form, IProgressReporter
    {
        private bool _allowCancel;
        private bool _dataSet;

        public InstallerDialog()
        {
            InitializeComponent();
        }

        public void Initialize(string appExecutablePath, string appName)
        {
            Icon = Icon.ExtractAssociatedIcon(appExecutablePath);
            Text = appName;
            copyingLabel.Text = Properties.strings.InstallerExtractingFilesText;
            ShowDialog();
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
                copyingLabel.Text = string.Format(Properties.strings.InstallerCopyingText, currentFile);
                percentageLabel.Text = $@"{Math.Round(percentage, 1)}%";
            }));
        }

        public void ReportOperationProgress(float percentage, string currentOperation)
        {
            Invoke(new Action(() =>
            {
                extractProgressBar.Value = (int) percentage;
                copyingLabel.Text = $@"{currentOperation}";
                percentageLabel.Text = $@"{Math.Round(percentage, 1)}%";
            }));
        }

        public void Fail(Exception ex)
        {
            Invoke(
                new Action(
                    () =>
                        Popup.ShowPopup(this, SystemIcons.Error, Properties.strings.InstallerUpdatingErrorCaption,
                            ex, PopupButtons.Ok)));
        }

        public void InitializingFail(Exception ex)
        {
            Popup.ShowPopup(this, SystemIcons.Error, Properties.strings.InstallerInitializingErrorCaption, ex,
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