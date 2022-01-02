// NewProjectProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Windows;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Models.Ftp;
using nUpdate.Administration.ViewModels.NewProject;
using nUpdate.Administration.Views.Dialogs;
using WPFFolderBrowser;

namespace nUpdate.Administration.Views.NewProject
{
    public class NewProjectProvider : Singleton<NewProjectProvider>, INewProjectProvider
    {
        public void SetFinishAction(out Action finishAction)
        {
            finishAction = () =>
            {
                if (Application.Current.Dispatcher != null)
                    Application.Current.Dispatcher.Invoke(() => WindowManager.GetCurrentWindow().RequestClose());
            };
        }

        public string GetFtpDirectory(FtpData data)
        {
            var ftpBrowseDialog = new FtpBrowseDialog(data);
            var result = WindowManager.ShowModalWindow(ftpBrowseDialog);
            if (result.HasValue && result.Value)
                return ftpBrowseDialog.Directory;
            return null;
        }

        public string GetLocationDirectory(string initialDirectory)
        {
            var browseDialog = new WPFFolderBrowserDialog
            {
                Title = "Select the project location...",
                InitialDirectory = initialDirectory
            };

            var result = browseDialog.ShowDialog();
            if (result.HasValue && result.Value)
                return browseDialog.FileName;
            return null;
        }

        public string GetUpdateDirectory()
        {
            var browseDialog = new WPFFolderBrowserDialog
            {
                Title = "Select the local update directory..."
            };

            var result = browseDialog.ShowDialog();
            if (result.HasValue && result.Value)
                return browseDialog.FileName;
            return null;
        }
    }
}