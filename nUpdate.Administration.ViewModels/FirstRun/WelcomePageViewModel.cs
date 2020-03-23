using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class WelcomePageViewModel : WizardPageViewModelBase
    {
        public WelcomePageViewModel()
        {
            CanGoForward = true;
        }
    }
}
