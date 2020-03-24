// FtpDataPageViewModel.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Reflection;
using System.Security.Authentication;
using FluentFTP;
using nUpdate.Administration.BusinessLogic;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Models.Ftp;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class FtpDataPageViewModel : UpdateProviderWizardPageViewModelBase, IFirstUpdateProviderBase
    {
        private readonly INewProjectProvider _newProjectProvider;
        private readonly FtpData _transferData;
        private bool _autoConnect;
        private string _directory;
        private RelayCommand _directoryButtonCommand;
        private FtpEncryptionMode _encryptionMode;
        private string _host;
        private string _password;
        private int _port = 21;
        private SslProtocols _sslProtocols;
        private string _username;

        public FtpDataPageViewModel(WizardViewModelBase wizardViewModelBase, ProjectCreationData projectCreationData)
            : base(wizardViewModelBase, projectCreationData)
        {
            var serviceProvider = ServiceProviderHelper.CreateServiceProvider(Assembly.GetEntryAssembly());
            _newProjectProvider = (INewProjectProvider) serviceProvider.GetService(typeof(INewProjectProvider));
            _directoryButtonCommand = new RelayCommand(o => OnDirectoryButtonClick());
            _directory = "/";
            _transferData = new FtpData();

            PropertyChanged += (sender, args) => RefreshNavigation();
            CanGoBack = true;
        }

        public bool AutoConnect
        {
            get => _autoConnect;
            set => SetProperty(value, ref _autoConnect, nameof(AutoConnect));
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

        public FtpEncryptionMode EncryptionMode
        {
            get => _encryptionMode;
            set => SetProperty(value, ref _encryptionMode, nameof(EncryptionMode));
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

            ProjectCreationData.Project.TransferData = _transferData;
        }
    }
}