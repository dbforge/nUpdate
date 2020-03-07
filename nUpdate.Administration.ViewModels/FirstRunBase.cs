using System.Collections.Generic;
using System.Threading.Tasks;
using nUpdate.Administration.Models;
using nUpdate.Administration.ViewModels.FirstRun;

namespace nUpdate.Administration.ViewModels
{
    public class FirstRunBase : WizardBase
    {
        private readonly IFirstRunProvider _firstRunProvider;
        public FirstSetupData FirstSetupData { get; } = new FirstSetupData();

        public FirstRunBase(IFirstRunProvider firstRunProvider)
        {
            _firstRunProvider = firstRunProvider;
            _firstRunProvider.SetFinishAction(out var f);
            FinishingAction = f;

            InitializePages(new List<WizardPageBase>
            {
                new WelcomePageViewModel(),
                new KeyDatabaseSetupPageViewModel(this),
                new PathSetupPageViewModel(this, _firstRunProvider)
            });
        }

        protected override Task<bool> Finish()
        {
            return Task.Run(() =>
            {
                if (!_firstRunProvider.Finish(FirstSetupData))
                    return false;

                FinishingAction?.Invoke();
                return true;
            });
        }
    }
}
