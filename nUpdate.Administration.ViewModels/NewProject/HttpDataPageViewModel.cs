using System;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Http;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class HttpDataPageViewModel : WizardPageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private readonly HttpData _transferData;
        private string _username;
        private string _password;
        private string _confirmationPassword;
        private string _scriptName;
        private bool _scriptNameEditable;

        public HttpDataPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            _transferData = new HttpData();

            PropertyChanged += (sender, args) => RefreshNavigation();
            CanGoBack = true;
        }

        public string Username
        {
            get => _username;
            set => SetProperty(value, ref _username);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(value, ref _password);
        }

        public string ConfirmationPassword
        {
            get => _confirmationPassword;
            set => SetProperty(value, ref _confirmationPassword);
        }

        public string ScriptName
        {
            get => _scriptName;
            set => SetProperty(value, ref _scriptName);
        }

        public bool ScriptNameEditable
        {
            get => _scriptNameEditable;
            set => SetProperty(value, ref _scriptNameEditable);
        }

        public override void OnNavigated(WizardPageViewModel fromPage, WizardViewModel window)
        {
            base.OnNavigated(fromPage, window);
            var backendType = _newProjectViewModel.ProjectCreationData.HttpBackendType;
            ScriptName = backendType != HttpBackendType.Custom
                ? EnumDescriptionHelper.GetEnumDescription(backendType)
                : string.Empty;
            ScriptNameEditable = backendType == HttpBackendType.Custom;
        }

        private void RefreshNavigation()
        {
            CanGoForward = !string.IsNullOrEmpty(Username) && !string.IsNullOrWhiteSpace(Password) && Password.Equals(ConfirmationPassword);
           
            // If the data is okay, we will set it directly.
            if (CanGoForward)
                RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            _transferData.ScriptUri =
                new Uri(_newProjectViewModel.ProjectCreationData.Project.UpdateDirectory, ScriptName);
            _transferData.Username = Username;
            _transferData.Password = Password;
            _newProjectViewModel.ProjectCreationData.Project.TransferData = _transferData;
        }
    }
}
