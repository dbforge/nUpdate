// UpdateActionPathProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using nUpdate.Actions;

namespace nUpdate.UpdateInstaller
{
    internal class UpdateActionPathProvider : IUpdateActionPathProvider
    {
        public string AssignPathVariables(string path)
        {
            return path.Replace(path.Split('\\')[0].Trim(), Environment.GetFolderPath(
                (Environment.SpecialFolder) Enum.Parse(typeof(Environment.SpecialFolder), path.Split('\\')[0].Trim())));
        }
    }
}