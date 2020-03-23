using System;

namespace nUpdate.Administration.PluginBase
{
    public class PluginLoadingException : Exception
    {
        public PluginLoadingException(Exception e) : base("Error while loading available plugins.", e) {}
    }
}
