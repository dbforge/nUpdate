// UpdateActionAppPathProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using nUpdate.Actions;

namespace nUpdate.UpdateInstaller
{
    internal class UpdateActionAppPathProvider : IUpdateActionAppPathProvider
    {
        public string GetAppPath()
        {
            return Program.AppDirectory;
        }
    }
}