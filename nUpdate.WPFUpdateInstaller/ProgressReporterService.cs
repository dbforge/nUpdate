// ProgressReporterService.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using nUpdate.UpdateInstaller.UIBase;

namespace nUpdate.WPFUpdateInstaller
{
    public class ProgressReporterService : IProgressReporter
    {
        private readonly MainWindow _mainWindow;

        public ProgressReporterService()
        {
            _mainWindow = new MainWindow();
        }

        public void Fail(Exception ex)
        {
            _mainWindow.Fail(ex);
        }

        public void Initialize()
        {
            _mainWindow.Initialize();
        }

        public void InitializingFail(Exception ex)
        {
            _mainWindow.InitializingFail(ex);
        }

        public void ReportOperationProgress(float progress, string currentOperation)
        {
            _mainWindow.ReportOperationProgress(progress, currentOperation);
        }

        public void ReportUnpackingProgress(float progress, string currentFile)
        {
            _mainWindow.ReportUnpackingProgress(progress, currentFile);
        }

        public void Terminate()
        {
            _mainWindow.Terminate();
        }
    }
}