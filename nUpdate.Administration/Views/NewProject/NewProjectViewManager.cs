using System;
using System.Collections.Generic;
using nUpdate.Administration.ViewModels.NewProject;

namespace nUpdate.Administration.Views.NewProject
{
    public class NewProjectViewManager : ViewManager
    {
        public NewProjectViewManager() : base(
            new Dictionary<Type, Type>
            {
                {typeof(GenerateKeyPairPageViewModel), typeof(GenerateKeyPairPage)},
                {typeof(GeneralDataPageViewModel), typeof(GeneralDataPage)},
                {typeof(UpdateProviderSelectionPageViewModel), typeof(UpdateProviderSelectionPage)},
                {typeof(HttpBackendSelectionPageViewModel), typeof(HttpBackendSelectionPage)},
                {typeof(HttpDataPageViewModel), typeof(HttpDataPage)},
                {typeof(FtpDataPageViewModel), typeof(FtpDataPage)}
            })
        { }
    }
}
