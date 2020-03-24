// MainWindowViewModel.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Windows.Input;
using nUpdate.Administration.Infrastructure;

// ReSharper disable MemberCanBeMadeStatic.Local

namespace nUpdate.Administration.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private bool _canEditMasterPassword;
        private ICommand _loadCommand;
        private ICommand _newProjectCommand;

        public MainWindowViewModel(IMainViewActionProvider mainWindowActionProvider)
        {
            LoadCommand = new RelayCommand(o => mainWindowActionProvider.Load());
            NewProjectCommand = new RelayCommand(o => mainWindowActionProvider.CreateNewProject());
            CanEditMasterPassword = mainWindowActionProvider.CanEditMasterPassword();
        }

        public bool CanEditMasterPassword
        {
            get => _canEditMasterPassword;
            set => SetProperty(value, ref _canEditMasterPassword);
        }

        public ICommand LoadCommand
        {
            get => _loadCommand;
            set => SetProperty(value, ref _loadCommand);
        }

        public ICommand NewProjectCommand
        {
            get => _newProjectCommand;
            set => SetProperty(value, ref _newProjectCommand);
        }
    }
}