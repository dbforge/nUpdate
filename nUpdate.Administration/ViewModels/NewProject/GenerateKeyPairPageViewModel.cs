using System.Threading.Tasks;
using System.Windows.Input;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GenerateKeyPairPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;

        public GenerateKeyPairPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;

            // This page does not need any actions performed by the user. It will do everything on its own.
            NeedsUserInteraction = false;
        }

        public override async void OnNavigated(PageViewModel fromPage, PagedWindowViewModel window)
        {
            base.OnNavigated(fromPage, window);

            // Generate the key pair.
            await GenerateKeyPair();

            // Allow the base class to go forward, but do not allow to show this page again.
            CanGoForward = true;
            CanBeShown = false;

            // Request going forward to the next page automatically.
            _newProjectViewModel.RequestGoForward();
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