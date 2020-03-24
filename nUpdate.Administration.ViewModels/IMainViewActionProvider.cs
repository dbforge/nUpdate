// IMainViewActionProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

namespace nUpdate.Administration.ViewModels
{
    public interface IMainViewActionProvider : ILoadActionProvider
    {
        bool CanEditMasterPassword();
        void CreateNewProject();
    }
}