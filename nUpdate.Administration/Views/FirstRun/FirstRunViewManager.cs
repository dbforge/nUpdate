// FirstRunViewManager.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using nUpdate.Administration.ViewModels.FirstRun;

namespace nUpdate.Administration.Views.FirstRun
{
    public class FirstRunViewManager : ViewManager
    {
        public FirstRunViewManager() : base(
            new Dictionary<Type, Type>
            {
                {typeof(WelcomePageViewModel), typeof(WelcomePage)},
                {typeof(KeyDatabaseSetupPageViewModel), typeof(KeyDatabaseSetupPage)},
                {typeof(PathSetupPageViewModel), typeof(PathSetupPage)}
            })
        {
        }
    }
}