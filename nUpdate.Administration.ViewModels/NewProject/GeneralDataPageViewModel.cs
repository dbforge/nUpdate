// GeneralDataPageViewModel.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.IO;
using System.Windows.Input;
using nUpdate.Administration.BusinessLogic;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GeneralDataPageViewModel : WizardPageViewModelBase
    {
        private readonly NewProjectBase _newProjectBase;
        private string _location;
        private ICommand _locationSelectCommand;
        private string _name;
        private string _updateDirectory;
        private ICommand _updateDirectorySelectCommand;

        public GeneralDataPageViewModel(NewProjectBase @base, INewProjectProvider newProjectProvider)
        {
            _newProjectBase = @base;

            _location = PathProvider.DefaultProjectDirectory;
            _locationSelectCommand = new RelayCommand(o =>
                Location = newProjectProvider.GetLocationDirectory(_location));
            _updateDirectorySelectCommand = new RelayCommand(o =>
                UpdateDirectory = newProjectProvider.GetUpdateDirectory());

            PropertyChanged += (sender, args) => RefreshNavigation();
        }

        public string Location
        {
            get => _location;
            set => SetProperty(value, ref _location, nameof(Location));
        }

        public ICommand LocationSelectCommand
        {
            get => _locationSelectCommand;
            set => SetProperty(value, ref _locationSelectCommand, nameof(LocationSelectCommand));
        }

        public string Name
        {
            get => _name;
            set => SetProperty(value, ref _name, nameof(Name));
        }

        public string UpdateDirectory
        {
            get => _updateDirectory;
            set => SetProperty(value, ref _updateDirectory, nameof(UpdateDirectory));
        }

        public ICommand UpdateDirectorySelectCommand
        {
            get => _updateDirectorySelectCommand;
            set => SetProperty(value, ref _updateDirectorySelectCommand, nameof(UpdateDirectorySelectCommand));
        }

        public override void OnNavigated(WizardPageViewModelBase fromPage, WizardViewModelBase window)
        {
            if (!Directory.Exists(PathProvider.DefaultProjectDirectory))
                Directory.CreateDirectory(PathProvider.DefaultProjectDirectory);

            var nameAvailable = false;
            var targetDirectory = default(DirectoryInfo);
            var i = 0;
            while (!nameAvailable)
            {
                targetDirectory = new DirectoryInfo(Path.Combine(PathProvider.DefaultProjectDirectory,
                    $"NewProject{(i > 0 ? i.ToString() : string.Empty)}"));
                if (!(nameAvailable = !targetDirectory.Exists))
                    ++i;
            }

            Name = targetDirectory.Name;
        }

        private void RefreshNavigation()
        {
            CanGoForward = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Location) &&
                           Uri.TryCreate(UpdateDirectory, UriKind.Absolute, out _);

            // If the data is okay, we will set it directly.
            if (CanGoForward)
                RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            _newProjectBase.ProjectCreationData.Project.Name = Name;
            _newProjectBase.ProjectCreationData.Project.UpdateDirectory = new Uri(UpdateDirectory);
            _newProjectBase.ProjectCreationData.Location = Location;
        }
    }
}