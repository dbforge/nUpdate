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

        public void Initialize()
        {
            _installerDialog.Initialize();
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