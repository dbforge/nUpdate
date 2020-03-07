using nUpdate.Administration.Models;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class UpdateProviderSelectionPageViewModel : WizardPageBase
    {
        private readonly NewProjectBase _newProjectBase;
        private UpdateProviderType _updateProviderType;

        public UpdateProviderSelectionPageViewModel(NewProjectBase @base)
        {
            _newProjectBase = @base;
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
                _newProjectBase.ProjectCreationData.UpdateProviderType = value;
            }
        }
    }
}
