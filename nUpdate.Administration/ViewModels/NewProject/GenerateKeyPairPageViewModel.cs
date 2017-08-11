using System.Threading.Tasks;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GenerateKeyPairPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private ICommand _loadCommand;

        public GenerateKeyPairPageViewModel(NewProjectViewModel viewModel)
        {
            // This page does not need any actions performed by the user. It will do everything on its own.
            NeedsUserInteraction = false;

            _newProjectViewModel = viewModel;
            LoadCommand = new RelayCommand(async () =>
            {
                // Generate the key pair.
                await GenerateKeyPair();

                // Allow the base class to go forward, but do not allow to show this page again.
                CanGoForward = true;
                CanBeShown = false;
                // Request going forward to the next page automatically.
                _newProjectViewModel.RequestGoForward();
            });
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand; }
            set
            {
                _loadCommand = value;
                OnPropertyChanged();
            }
        }

        private Task GenerateKeyPair()
        {
            return Task.Run(() =>
            {
                var rsa = new RsaManager();
                _newProjectViewModel.ProjectCreationData.Project.PublicKey = rsa.PublicKey;
                _newProjectViewModel.ProjectCreationData.PrivateKey = rsa.PrivateKey;
            });
        }
    }
}