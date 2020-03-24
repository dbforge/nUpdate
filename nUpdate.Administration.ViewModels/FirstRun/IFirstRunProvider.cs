// IFirstRunProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using nUpdate.Administration.Models;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public interface IFirstRunProvider : IFinishProvider
    {
        bool Finish(FirstSetupData firstSetupData);
        void GetApplicationDataDirectoryCommandAction(ref string applicationDataDirectory);
        void GetDefaultProjectDirectoryCommandAction(ref string defaultProjectDirectory);
    }
}