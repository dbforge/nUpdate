using System;
using System.Windows;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Ftp;
using nUpdate.Administration.Common.Providers;
using nUpdate.Administration.Views;
using nUpdate.Administration.Views.Dialogs;
using WPFFolderBrowser;

namespace nUpdate.Administration.ViewModels.Providers
{
    public class NewProjectProvider : Singleton<NewProjectProvider>, INewProjectProvider
    {
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

        public void SetFinishAction(out Action finishAction)
        {
            finishAction = () => Application.Current.Dispatcher.Invoke(() => WindowManager.GetCurrentWindow().RequestClose());
        }
    }
}
