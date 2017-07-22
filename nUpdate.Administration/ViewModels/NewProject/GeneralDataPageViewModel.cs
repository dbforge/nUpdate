namespace nUpdate.Administration.ViewModels.NewProject
{
    public class GeneralDataPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;

        public GeneralDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            CanGoBack = true;
        }
    }
}
