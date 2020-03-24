// UpdateProviderPluginManager.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.PluginBase.BusinessLogic;

namespace nUpdate.Administration.BusinessLogic
{
    public class UpdateProviderPluginManager : Singleton<UpdateProviderPluginManager>
    {
        [ImportMany(typeof(IUpdateProvider))] public IUpdateProvider[] UpdateProviders { get; set; }

        public void LoadPlugins()
        {
            var directoryCatalog = new DirectoryCatalog(PathProvider.PluginDirectory);
            var container = new CompositionContainer(directoryCatalog);
            container.ComposeParts(this);
        }
    }
}