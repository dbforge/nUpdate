using System.Collections.Generic;
using nUpdate.Administration.ViewModels.FirstRun;

namespace nUpdate.Administration.ViewModels
{
    public class FirstRunViewModel : PagedWindowViewModel
    {
        public FirstRunViewModel()
        {
            InitializePages(new List<PageViewModel>
            {
                new WelcomePageViewModel(),
                new KeyDatabaseSetupPageViewModel()
            });
        }
    }
}
