// SshServerUpdateProvider.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Threading.Tasks;
using nUpdate.Administration.PluginBase.BusinessLogic;

namespace nUpdate.Administration.BusinessLogic
{
    public class SshServerUpdateProvider : IUpdateProvider
    {
        public Task UploadPackage()
        {
            throw new NotImplementedException();
        }
    }
}