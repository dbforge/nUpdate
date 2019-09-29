using System;
using nUpdate.Administration.Common.Http;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class HttpDataPageViewModel : WizardPageViewModel, IUpdateProviderPageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private readonly HttpData _transferData;
        private string _scriptUri;

        public HttpDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            _transferData = new HttpData();

            PropertyChanged += (sender, args) => RefreshNavigation();
            CanGoBack = true;
        }

        public string ScriptUri
        {
            get => _scriptUri;
            set => SetProperty(value, ref _scriptUri, nameof(ScriptUri));
        }

        private void RefreshNavigation()
        {
            CanGoForward = !string.IsNullOrEmpty(ScriptUri) && Uri.TryCreate(ScriptUri, UriKind.Absolute, out Uri _);
           
            // If the data is okay, we will set it directly.
            if (CanGoForward)
                RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            _transferData.ScriptUri = new Uri(ScriptUri);

            _newProjectViewModel.ProjectCreationData.Project.TransferData = _transferData;
        }
    }
}
