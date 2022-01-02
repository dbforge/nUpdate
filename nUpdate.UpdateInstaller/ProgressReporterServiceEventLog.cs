// ProgressReporterServiceEventLog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Threading;
using nUpdate.UpdateInstaller.UIBase;

namespace nUpdate.UpdateInstaller
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