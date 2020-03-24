// IUpdateProvider.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System.Threading.Tasks;

namespace nUpdate.Administration.PluginBase.BusinessLogic
{
    public interface IUpdateProvider
    {
        Task UploadPackage();
    }
}