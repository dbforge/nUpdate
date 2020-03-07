using System.Threading.Tasks;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GenerateKeyPairPageViewModel : WizardPageBase
    {
        private readonly NewProjectBase _newProjectBase;

        public GenerateKeyPairPageViewModel(NewProjectBase @base)
        {
            _newProjectBase = @base;
            
            NeedsUserInteraction = false;
        }

        public override async void OnNavigated(WizardPageBase fromPage, WizardBase window)
        {
            base.OnNavigated(fromPage, window);

            // Generate the key pair.
            await GenerateKeyPair();
            
            CanGoForward = true;
            CanBeShown = false;

            // Request going forward to the next page automatically.
            _newProjectBase.RequestGoForward();
        }

        private Task GenerateKeyPair()
        {
            return Task.Run(() =>
            {
                var rsa = new RsaManager();
                _newProjectBase.ProjectCreationData.Project.PublicKey = rsa.PublicKey;
                _newProjectBase.ProjectCreationData.PrivateKey = rsa.PrivateKey;
            });
        }
    }
}