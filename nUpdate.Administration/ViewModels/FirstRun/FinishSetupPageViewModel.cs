// Author: Dominic Beger (Trade/ProgTrade) 2017

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Properties;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class FinishSetupPageViewModel : PageViewModel
    {
        private const float _time = 3;
        private readonly FirstRunViewModel _firstRunViewModel;
        private string _currentAction = "Please wait while nUpdate Administration finishes your first time setup...";
        private ICommand _loadCommand;

        public FinishSetupPageViewModel(FirstRunViewModel firstRunViewModel)
        {
            _firstRunViewModel = firstRunViewModel;
            NeedsUserInteraction = false;

            LoadCommand = new RelayCommand(async () =>
            {
                // Finish the setup.
                await Setup();
                CanGoForward = true;

                // Start the timer for the closing countdown.
                var timeLeft = _time;
                var timer = new Timer(obj => Dispatcher.Invoke(() =>
                {
                    if (timeLeft <= 0)
                    {
                        _firstRunViewModel.RequestClose();
                        return;
                    }

                    timeLeft = timeLeft - 0.1f;
                    CurrentAction =
                        $"Setup completed. {(timeLeft >= 0 ? $"This dialog will close in {timeLeft:F1} seconds." : string.Empty)}";
                }), null, 0, 100);
            });
        }

        public string CurrentAction
        {
            get => _currentAction;
            set => SetProperty(value, ref _currentAction, nameof(CurrentAction));
        }

        public ICommand LoadCommand
        {
            get => _loadCommand;
            set => SetProperty(value, ref _loadCommand, nameof(LoadCommand));
        }

        private Task Setup()
        {
            return Task.Run(() =>
            {
                // Create all the folders
                CurrentAction = "Checking and creating directories...";

                var setupData = _firstRunViewModel.FirstSetupData;
                if (!Directory.Exists(setupData.ApplicationDataLocation))
                    Directory.CreateDirectory(setupData.ApplicationDataLocation);
                if (!Directory.Exists(setupData.DefaultProjectDirectory))
                    Directory.CreateDirectory(setupData.DefaultProjectDirectory);

                // Set the master password for this session, if encryption should be used
                CurrentAction = "Setting encryption settings...";

                var encrypt =
                    setupData.EncryptKeyDatabase;
                Settings.Default.UseEncryptedKeyDatabase = encrypt;
                if (encrypt)
                    GlobalSession.MasterPassword = setupData.MasterPassword;

                // Save the key database and setting its password
                CurrentAction = "Creating key database...";
                KeyManager.Instance.Save();

                // Save the application specific data
                CurrentAction = "Setting program data...";

                Settings.Default.ApplicationDataPath = setupData.ApplicationDataLocation;
                Settings.Default.DefaultProjectPath = setupData.DefaultProjectDirectory;
                Settings.Default.FirstRun = false;
                Settings.Default.Save();
            });
        }
    }
}