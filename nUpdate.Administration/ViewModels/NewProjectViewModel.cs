using System.Collections.Generic;
using nUpdate.Administration.ViewModels.NewProject;

namespace nUpdate.Administration.ViewModels
{
    public class NewProjectViewModel : PagedWindowViewModel
    {
        public ProjectCreationData ProjectCreationData { get; } = new ProjectCreationData();

        public NewProjectViewModel()
        {
            InitializePages(new List<PageViewModel>
            {
                new GenerateKeyPairPageViewModel(this),
                new GeneralDataPageViewModel(this)
            });
        }
    }
}
