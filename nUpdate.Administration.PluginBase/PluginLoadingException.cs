// PluginLoadingException.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Administration.PluginBase
{
    public class PluginLoadingException : Exception
    {
        public PluginLoadingException(Exception e) : base("Error while loading available plugins.", e)
        {
        }
    }
}