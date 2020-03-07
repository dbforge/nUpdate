using nUpdate.Administration.Models.Http;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class HttpBackendSelectionPageViewModel : WizardPageBase, IFirstUpdateProviderSubWizardPageViewModel
    {
        private readonly NewProjectBase _newProjectBase;
        private HttpBackendType _httpBackendType;

        public HttpBackendSelectionPageViewModel(NewProjectBase newProjectBase)
        {
            _newProjectBase = newProjectBase;

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
            _newProjectBase.ProjectCreationData.HttpBackendType = _httpBackendType;
        }
    }
}
