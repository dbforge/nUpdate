using System;
using nUpdate.UpdateInstaller.Client.GuiInterface;

namespace nUpdate.UpdateInstaller.Core
{
    public class ProgressReporterServiceEventLog : IProgressReporter
    {
        private bool shouldRun = true;

 
        public void Fail(Exception ex)
        {
            WindowsEventLog.LogException(ex);
        }

        public void Initialize()
        {
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
            shouldRun = false;
        }
    }
}