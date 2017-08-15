using System.Threading.Tasks;
namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GenerateKeyPairPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;

        public GenerateKeyPairPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            
            NeedsUserInteraction = false;
        }

        public override async void OnNavigated(PageViewModel fromPage, PagedWindowViewModel window)
        {
            base.OnNavigated(fromPage, window);

            // Generate the key pair.
            await GenerateKeyPair();
            
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