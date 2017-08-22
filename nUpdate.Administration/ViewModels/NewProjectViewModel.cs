using System.Collections.Generic;
using System.Threading.Tasks;
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
                new GeneralDataPageViewModel(this),
                new ProtocolSelectionPageViewModel(this),
                new FtpDataPageViewModel(this),
                new HttpDataPageViewModel(this)
            });
        }

        protected override Task<bool> Finish()
        {
            throw new System.NotImplementedException();
        }
    }
}
