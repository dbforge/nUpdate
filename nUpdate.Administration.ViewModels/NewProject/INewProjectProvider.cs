// INewProjectProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using nUpdate.Administration.Models.Ftp;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public interface INewProjectProvider : IFinishProvider
    {
        string GetFtpDirectory(FtpData data);
        string GetLocationDirectory(string initialDirectory);
        string GetUpdateDirectory();
    }
}