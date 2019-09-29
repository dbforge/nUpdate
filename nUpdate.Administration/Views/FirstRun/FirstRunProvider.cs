using System;
using System.IO;
using System.Windows;
using nUpdate.Administration.Common;
using nUpdate.Administration.ViewModels.FirstRun;
using WPFFolderBrowser;

namespace nUpdate.Administration.Views.FirstRun
{
    internal class FirstRunProvider : Singleton<FirstRunProvider>, IFirstRunProvider
    {
        public bool Finish(FirstSetupData firstSetupData)
        {
            // Set the application specific data
            SettingsManager.Instance["ApplicationDataPath"] = firstSetupData.ApplicationDataLocation;
            SettingsManager.Instance["DefaultProjectPath"] = firstSetupData.DefaultProjectDirectory;
            SettingsManager.Instance["FirstRun"] = false;

            // Set the encryption settings
            var encrypt =
                firstSetupData.EncryptKeyDatabase;
            SettingsManager.Instance["UseEncryptedKeyDatabase"] = encrypt;

            // Save the settings that we've just set
            SettingsManager.Instance.Save();

            // Create all the folders
            if (!Directory.Exists(firstSetupData.ApplicationDataLocation))
                Directory.CreateDirectory(firstSetupData.ApplicationDataLocation);
            if (!Directory.Exists(firstSetupData.DefaultProjectDirectory))
                Directory.CreateDirectory(firstSetupData.DefaultProjectDirectory);

            // Set the master password for this session, if encryption should be used
            if (encrypt)
                GlobalSession.MasterPassword = firstSetupData.MasterPassword;

            // Save the key database to set its password for the first time and load it afterwards
            KeyManager.Instance.Save();
            KeyManager.Instance.Initialize(GlobalSession.MasterPassword);

            return true;
        }

        public void GetApplicationDataDirectoryCommandAction(ref string applicationDataDirectory)
        {
            var browseDialog = new WPFFolderBrowserDialog
            {
                Title = "Select the location for the application data..."
            };

            var result = browseDialog.ShowDialog();
            if (result.HasValue && result.Value)
                applicationDataDirectory = browseDialog.FileName;
        }

        public void GetDefaultProjectDirectoryCommandAction(ref string defaultProjectDirectory)
        {
            var browseDialog = new WPFFolderBrowserDialog
            {
                Title = "Select the default location for your projects..."
            };

            var result = browseDialog.ShowDialog();
            if (result.HasValue && result.Value)
                defaultProjectDirectory = browseDialog.FileName;
        }

        public void SetFinishAction(out Action action)
        {
            action = () => Application.Current.Dispatcher.Invoke(() => WindowManager.GetCurrentWindow().RequestClose());
        }
    }
}
