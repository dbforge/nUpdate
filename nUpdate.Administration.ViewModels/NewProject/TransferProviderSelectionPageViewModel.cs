using nUpdate.Administration.Common;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class TransferProviderSelectionPageViewModel : WizardPageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private TransferProviderType _transferProviderType;

        public TransferProviderSelectionPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            CanGoBack = true;
            CanGoForward = true;

            TransferProviderType = TransferProviderType.Http;
        }

        public TransferProviderType TransferProviderType
        {
            get => _transferProviderType;
            set
            {
                SetProperty(value, ref _transferProviderType, nameof(TransferProviderType));
                _newProjectViewModel.ProjectCreationData.TransferProviderType = value;
            }
        }
    }
}
