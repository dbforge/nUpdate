using nUpdate.Administration.Models;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class UpdateProviderSelectionPageViewModel : WizardPageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private UpdateProviderType _updateProviderType;

        public UpdateProviderSelectionPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            CanGoBack = true;
            CanGoForward = true;

            UpdateProviderType = UpdateProviderType.ServerOverHttp;
        }

        public UpdateProviderType UpdateProviderType
        {
            get => _updateProviderType;
            set
            {
                SetProperty(value, ref _updateProviderType, nameof(UpdateProviderType));
                _newProjectViewModel.ProjectCreationData.UpdateProviderType = value;
            }
        }
    }
}
