using System.Threading.Tasks;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GenerateKeyPairPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;

        public GenerateKeyPairPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
        }

        public Task GenerateKeyPair()
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