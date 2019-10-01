using nUpdate.Administration.Common;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class HttpBackendSelectionPageViewModel : WizardPageViewModel, IFirstUpdateProviderSubWizardPageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private HttpBackendType _httpBackendType;

        public HttpBackendSelectionPageViewModel(NewProjectViewModel newProjectViewModel)
        {
            _newProjectViewModel = newProjectViewModel;

            PropertyChanged += (sender, args) => RefreshProjectData();
            CanGoBack = true;
            CanGoForward = true;
        }

        public HttpBackendType HttpBackendType
        {
            get => _httpBackendType;
            set => SetProperty(value, ref _httpBackendType);
        }

        private void RefreshProjectData()
        {
            _newProjectViewModel.ProjectCreationData.HttpBackendType = _httpBackendType;
        }
    }
}
