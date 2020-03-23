using System;
using System.Collections.Generic;
using nUpdate.Administration.PluginBase;

namespace nUpdate.Administration.BusinessLogic
{
    public static class GlobalSession
    {
        static GlobalSession()
        {
            try
            {
                var updateProviderPluginLoader = new PluginLoader<IUpdateProviderPlugin>(PathProvider.PluginDirectory);
                updateProviderPluginLoader.Load();
                UpdateProviderPlugins = updateProviderPluginLoader.Plugins;

            }
            catch (Exception e)
            {
                throw new PluginLoadingException(e);
            }
        }

        public static string MasterPassword { get; set; }
        public static IEnumerable<Lazy<IUpdateProviderPlugin>> UpdateProviderPlugins { get; set; }
    }
}
