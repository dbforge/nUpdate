using System;
using nUpdate.Administration.BusinessLogic;
using nUpdate.Administration.Models.Http;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class HttpDataPageViewModel : UpdateProviderWizardPageViewModelBase
    {
        private readonly HttpData _transferData;
        private string _username;
        private string _password;
        private string _confirmationPassword;
        private string _scriptName;
        private bool _scriptNameEditable;

        public HttpDataPageViewModel(WizardViewModelBase wizardViewModelBase, ProjectCreationData projectCreationData)
            : base(wizardViewModelBase, projectCreationData)
        {
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

        public override void OnNavigated(WizardPageViewModelBase fromPage, WizardViewModelBase window)
        {
            base.OnNavigated(fromPage, window);
            var backendType = ((HttpBackendSelectionPageViewModel) fromPage).HttpBackendType;
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
                new Uri(ProjectCreationData.Project.UpdateDirectory, ScriptName);
            _transferData.Username = Username;
            _transferData.Password = Password;
            ProjectCreationData.Project.TransferData = _transferData;
        }
    }
}
