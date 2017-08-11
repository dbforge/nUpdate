// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.IO;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GeneralDataPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private ICommand _loadCommand;
        private string _location;
        private string _name;
        private string _updateDirectory;

        public GeneralDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            CanGoBack = true;

            Location = PathProvider.DefaultProjectDirectory;
            LoadCommand = new RelayCommand(OnLoad);
        }

        public ICommand LoadCommand
        {
            get => _loadCommand;
            set => SetProperty(value, ref _loadCommand, nameof(LoadCommand));
        }

        public string Location
        {
            get => _location;
            set
            {
                SetProperty(value, ref _location, nameof(Location));
                RefreshNavigation();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(value, ref _name, nameof(Name));
                RefreshNavigation();
            }
        }

        public string UpdateDirectory
        {
            get => _updateDirectory;
            set
            {
                SetProperty(value, ref _updateDirectory, nameof(UpdateDirectory));
                RefreshNavigation();
            }
        }

        private void OnLoad()
        {
            if (!Directory.Exists(PathProvider.DefaultProjectDirectory))
                Directory.CreateDirectory(PathProvider.DefaultProjectDirectory);
            
            var nameAvailable = false;
            var targetDirectory = default(DirectoryInfo);
            while (!nameAvailable)
            {
                var i = 0;
                targetDirectory = new DirectoryInfo(Path.Combine(PathProvider.DefaultProjectDirectory,
                    $"NewProject{(i > 0 ? i.ToString() : string.Empty)}"));
                if (!(nameAvailable = !targetDirectory.Exists))
                    ++i;
            }

            Name = targetDirectory.Name;
        }

        private void RefreshNavigation()
        {
            Uri updateDirectoryUri;
            CanGoForward = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Location) &&
                           Uri.TryCreate(UpdateDirectory, UriKind.Absolute, out updateDirectoryUri);

            // If the data is okay, we will set it directly.
            if (CanGoForward)
                RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            _newProjectViewModel.ProjectCreationData.Project.Name = Name;
            _newProjectViewModel.ProjectCreationData.Project.UpdateDirectory = new Uri(UpdateDirectory);
            _newProjectViewModel.ProjectCreationData.Location = Location;
        }
    }
}