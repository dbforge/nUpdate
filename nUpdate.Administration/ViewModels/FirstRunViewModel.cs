using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using nUpdate.Administration.Properties;
using nUpdate.Administration.ViewModels.FirstRun;

namespace nUpdate.Administration.ViewModels
{
    public class FirstRunViewModel : PagedWindowViewModel
    {
        public FirstSetupData FirstSetupData { get; } = new FirstSetupData();

        public FirstRunViewModel()
        {
            InitializePages(new List<PageViewModel>
            {
                new WelcomePageViewModel(),
                new KeyDatabaseSetupPageViewModel(this),
                new PathSetupPageViewModel(this)
            });
        }

        protected override Task<bool> Finish()
        {
            return Task.Run(() =>
            {
                // Create all the folders
                if (!Directory.Exists(FirstSetupData.ApplicationDataLocation))
                    Directory.CreateDirectory(FirstSetupData.ApplicationDataLocation);
                if (!Directory.Exists(FirstSetupData.DefaultProjectDirectory))
                    Directory.CreateDirectory(FirstSetupData.DefaultProjectDirectory);

                // Save the encryption settings
                var encrypt =
                    FirstSetupData.EncryptKeyDatabase;
                Settings.Default.UseEncryptedKeyDatabase = encrypt;

                // Set the master password for this session, if encryption should be used
                if (encrypt)
                    GlobalSession.MasterPassword = FirstSetupData.MasterPassword;

                // Save the key database to set its password for the first time and load it afterwards
                KeyManager.Instance.Save();
                KeyManager.Instance.Initialize(GlobalSession.MasterPassword);

                // Save the application specific data
                Settings.Default.ApplicationDataPath = FirstSetupData.ApplicationDataLocation;
                Settings.Default.DefaultProjectPath = FirstSetupData.DefaultProjectDirectory;
                Settings.Default.FirstRun = false;
                Settings.Default.Save();

                return true;
            });
        }
    }
}
