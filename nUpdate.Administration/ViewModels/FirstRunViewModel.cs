using System.Collections.Generic;
using nUpdate.Administration.ViewModels.FirstRun;

namespace nUpdate.Administration.ViewModels
{
    public class FirstRunViewModel : PagedWindowViewModel
    {
        public FirstSetupData FirstSetupData { get; } = new FirstSetupData();

        public FirstRunViewModel()
        {
            InitializePages(new List<PageViewModel>
            {
                new WelcomePageViewModel(),
                new KeyDatabaseSetupPageViewModel(this),
                new PathSetupPageViewModel(this),
                new FinishSetupPageViewModel(this)
            });
        }
    }
}
