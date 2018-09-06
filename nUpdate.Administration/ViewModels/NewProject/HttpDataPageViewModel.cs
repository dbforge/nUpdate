using System.ComponentModel;

namespace nUpdate.Administration.ViewModels.NewProject
{
    [Description("HttpDataPageViewModel")]
    public class HttpDataPageViewModel : PageViewModel, IProtocolPageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;

        public HttpDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
        }
    }
}
