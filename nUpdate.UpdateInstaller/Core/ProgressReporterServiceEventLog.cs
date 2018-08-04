using System;
using System.Threading;
using nUpdate.UpdateInstaller.Client.GuiInterface;

namespace nUpdate.UpdateInstaller.Core
{
    public class ProgressReporterServiceEventLog : IProgressReporter
    {
        private bool _shouldRun = true;

        public void Fail(Exception ex)
        {
            WindowsEventLog.LogException(ex);
        }

        public void Initialize()
        {
            if (!WindowsServiceHelper.IsRunningInServiceContext) return;
            while (_shouldRun)
                Thread.Sleep(2000);
        }

        public void InitializingFail(Exception ex)
        {
            WindowsEventLog.LogException(ex);
        }

        public void ReportOperationProgress(float progress, string currentOperation)
        {
        }

        public void ReportUnpackingProgress(float progress, string currentFile)
        {
        }

        public void Terminate()
        {
            _shouldRun = false;
        }
    }
}