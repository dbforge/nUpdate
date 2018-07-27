// Copyright © Dominic Beger 2018

using System;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.UI.Dialogs;

namespace nUpdate.UpdateInstaller.Core
{
    public class ProgressReporterService : IProgressReporter
    {
        private readonly MainForm _mainForm;
        private bool shouldRun = true;

        public ProgressReporterService()
        {
            if (!WindowsServiceHelper.IsRunningInServiceContext)
            {
                _mainForm = new MainForm();
            }
        }

        public void Fail(Exception ex)
        {
            _mainForm?.Fail(ex);
        }

        public void Initialize()
        {
            _mainForm?.Initialize();

            if(WindowsServiceHelper.IsRunningInServiceContext)
            {
                while (shouldRun)
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }

        public void InitializingFail(Exception ex)
        {
            _mainForm?.InitializingFail(ex);
        }

        public void ReportOperationProgress(float progress, string currentOperation)
        {
            _mainForm?.ReportOperationProgress(progress, currentOperation);
        }

        public void ReportUnpackingProgress(float progress, string currentFile)
        {
            _mainForm?.ReportUnpackingProgress(progress, currentFile);
        }

        public void Terminate()
        {
            _mainForm?.Terminate();
            shouldRun = false;
        }
    }
}