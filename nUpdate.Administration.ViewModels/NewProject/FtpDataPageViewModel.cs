// Author: Dominic Beger (Trade/ProgTrade) 2017

using System.Security.Authentication;
using FluentFTP;
using nUpdate.Administration.BusinessLogic;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Models.Ftp;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class FtpDataPageViewModel : WizardPageViewModel, IFirstUpdateProviderSubWizardPageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private readonly INewProjectProvider _newProjectProvider;
        private readonly FtpData _transferData;
        private RelayCommand _directoryButtonCommand;
        private string _directory;
        private string _host;
        private string _password;
        private int _port = 21;
        private string _username;
        private bool _autoConnect;
        private FtpEncryptionMode _encryptionMode;
        private SslProtocols _sslProtocols;

        public FtpDataPageViewModel(NewProjectViewModel viewModel, INewProjectProvider newProjectProvider)
        {
            _newProjectViewModel = viewModel;
            _newProjectProvider = newProjectProvider;
            _directoryButtonCommand = new RelayCommand(OnDirectoryButtonClick);
            _directory = "/";
            _transferData = new FtpData();

            PropertyChanged += (sender, args) => RefreshNavigation();
            CanGoBack = true;
        }

        private void OnDirectoryButtonClick()
        {
            var data = new FtpData
            {
                Host = Host,
                Port = Port,
                Username = Username
            };

            // TODO: Add password in a good way
            KeyManager.Instance[data.Identifier] = Password;
            Directory = _newProjectProvider.GetFtpDirectory(data);
        }

        public string Directory
        {
            get => _directory;
            set => SetProperty(value, ref _directory, nameof(Directory));
        }

        public RelayCommand DirectoryButtonCommand
        {
            get => _directoryButtonCommand;
            set => SetProperty(value, ref _directoryButtonCommand, nameof(DirectoryButtonCommand));
        }

        public string Host
        {
            get => _host;
            set => SetProperty(value, ref _host, nameof(Host));
        }

        public string Password
        {
            get => _password;
            set => SetProperty(value, ref _password, nameof(Password));
        }

        public int Port
        {
            get => _port;
            set => SetProperty(value, ref _port, nameof(Port));
        }

        public bool AutoConnect
        {
            get => _autoConnect;
            set => SetProperty(value, ref _autoConnect, nameof(AutoConnect));
        }

        public FtpEncryptionMode EncryptionMode
        {
            get => _encryptionMode;
            set => SetProperty(value, ref _encryptionMode, nameof(EncryptionMode));
        }

        public SslProtocols SslProtocols
        {
            get => _sslProtocols;
            set => SetProperty(value, ref _sslProtocols, nameof(SslProtocols));
        }

        public string Username
        {
            get => _username;
            set => SetProperty(value, ref _username, nameof(Username));
        }

        private void RefreshNavigation()
        {
            CanGoForward = !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Username) &&
                           !string.IsNullOrEmpty(Directory);

            // If the data is okay, we will set it directly.
            if (CanGoForward)
                RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            _transferData.Host = Host;
            _transferData.Port = Port;
            _transferData.Username = Username;
            _transferData.Directory = Directory;

            _newProjectViewModel.ProjectCreationData.Project.TransferData = _transferData;
        }
    }
}