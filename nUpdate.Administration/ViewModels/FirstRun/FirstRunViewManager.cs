using System;
using System.Collections.Generic;
using nUpdate.Administration.Views.FirstRun;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class FirstRunViewManager : ViewManager
    {
        public FirstRunViewManager() : base(
            new Dictionary<Type, Type>
            {
                {typeof(WelcomePageViewModel), typeof(WelcomePage) },
                {typeof(KeyDatabaseSetupPageViewModel), typeof(KeyDatabaseSetupPage) }
            })
        { }
    }
}
