// ProgressReporterService.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using nUpdate.UI.WinForms.Dialogs;
using nUpdate.UpdateInstaller.UserInterface;

namespace nUpdate.UI.WinForms
{
    public class ProgressReporterService : IProgressReporter
    {
        private readonly InstallerDialog _installerDialog;

        public ProgressReporterService()
        {
            _installerDialog = new InstallerDialog();
        }

        public void Fail(Exception ex)
        {
            _installerDialog.Fail(ex);
        }

        public void Initialize(string appExecutablePath, string appName)
        {
            _installerDialog.Initialize(appExecutablePath, appName);
        }

        public void InitializingFail(Exception ex)
        {
            _installerDialog.InitializingFail(ex);
        }

        public void ReportOperationProgress(float progress, string currentOperation)
        {
            _installerDialog.ReportOperationProgress(progress, currentOperation);
        }

        public void ReportUnpackingProgress(float progress, string currentFile)
        {
            _installerDialog.ReportUnpackingProgress(progress, currentFile);
        }

        public void Terminate()
        {
            _installerDialog.Terminate();
        }
    }
}