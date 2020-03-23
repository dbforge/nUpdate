// Author: Dominic Beger (Trade/ProgTrade) 2017

using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class KeyDatabaseSetupPageViewModel : WizardPageViewModelBase
    {
        private readonly FirstRunBase _firstRunBase;
        private bool _encryptKeyDatabase = true;
        private string _masterPassword = string.Empty;
        private string _masterPasswordVerify = string.Empty;

        public KeyDatabaseSetupPageViewModel(FirstRunBase firstRunBase)
        {
            _firstRunBase = firstRunBase;
            CanGoBack = true;
        }

        public bool EncryptKeyDatabase
        {
            get => _encryptKeyDatabase;
            set
            {
                SetProperty(value, ref _encryptKeyDatabase, nameof(EncryptKeyDatabase));
                RefreshNavigation();
            }
        }

        public string MasterPassword
        {
            get => _masterPassword;
            set
            {
                SetProperty(value, ref _masterPassword, nameof(MasterPassword));
                RefreshNavigation();
            }
        }

        public string MasterPasswordVerify
        {
            get => _masterPasswordVerify;
            set
            {
                SetProperty(value, ref _masterPasswordVerify, nameof(MasterPasswordVerify));
                RefreshNavigation();
            }
        }
        private void RefreshFirstRunData()
        {
            _firstRunBase.FirstSetupData.EncryptKeyDatabase = EncryptKeyDatabase;
            _firstRunBase.FirstSetupData.MasterPassword = MasterPassword;
        }

        private void RefreshNavigation()
        {
            CanGoForward = !EncryptKeyDatabase || EncryptKeyDatabase && !string.IsNullOrEmpty(MasterPassword) &&
                           MasterPassword.Equals(MasterPasswordVerify);

            if (CanGoForward)
                RefreshFirstRunData();
        }
    }
}