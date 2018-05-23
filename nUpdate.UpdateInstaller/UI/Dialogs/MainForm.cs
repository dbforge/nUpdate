// Copyright © Dominic Beger 2018

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.Core;
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

        public void Fail(Exception ex)
        {
            Invoke(
                new Action(
                    () =>
                        Popup.ShowPopup(this, SystemIcons.Error, Program.UpdatingErrorCaption,
                            ex, PopupButtons.Ok)));
        }

        public void Initialize()
        {
            Icon = IconHelper.ExtractAssociatedIcon(Program.ApplicationExecutablePath);
            Text = Program.AppName;
            copyingLabel.Text = Program.ExtractFilesText;
            ShowDialog(); // We currently only have an instance, so we show the form as a modal dialog now.
        }

        public void InitializingFail(Exception ex)
        {
            Popup.ShowPopup(this, SystemIcons.Error, Program.InitializingErrorCaption, ex,
                PopupButtons.Ok);
        }

        public void ReportOperationProgress(float percentage, string currentOperation)
        {
            Invoke(new Action(() =>
            {
                extractProgressBar.Value = (int) percentage;
                copyingLabel.Text = $"{currentOperation}";
                percentageLabel.Text = $"{Math.Round(percentage, 1)}%";
            }));
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
                copyingLabel.Text = string.Format(Program.CopyingText, currentFile);
                percentageLabel.Text = $"{Math.Round(percentage, 1)}%";
            }));
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