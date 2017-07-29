using System;
using System.IO;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GeneralDataPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private string _name;
        private Uri _updateDirectory;
        private string _location;
        private ICommand _loadCommand;

        public GeneralDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            CanGoBack = true;
            LoadCommand = new RelayCommand(OnLoad);
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(value, ref _name); }
        }

        public Uri UpdateDirectory
        {
            get { return _updateDirectory; }
            set { SetProperty(value, ref _updateDirectory); }
        }

        public string Location
        {
            get { return _location; }
            set { SetProperty(value, ref _location); }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand; }
            set { SetProperty(value, ref _loadCommand); }
        }

        private void OnLoad()
        {
            if (!Directory.Exists(FilePathProvider.DefaultProjectsDirectory))
                Directory.CreateDirectory(FilePathProvider.DefaultProjectsDirectory);

            // Initialize the default text values
            var nameAvailable = false;
            var targetDirectory = default(DirectoryInfo);
            while (!nameAvailable)
            {
                var i = 0;
                targetDirectory = new DirectoryInfo(Path.Combine(FilePathProvider.DefaultProjectsDirectory, $"NewProject{(i > 0 ? i.ToString() : string.Empty)}"));
                if (!(nameAvailable = !targetDirectory.Exists))
                    ++i;
            }

            Name = targetDirectory.Name;
            Location = targetDirectory.FullName;
        }
    }
}