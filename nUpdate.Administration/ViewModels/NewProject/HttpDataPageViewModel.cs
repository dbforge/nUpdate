namespace nUpdate.Administration.ViewModels.NewProject
{
    public class HttpDataPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;

        public HttpDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
        }
    }
}
