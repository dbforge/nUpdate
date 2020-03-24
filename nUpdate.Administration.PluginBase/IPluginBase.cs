// IPluginBase.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Administration.PluginBase
{
    public interface IPluginBase
    {
        string Author { get; }
        string Description { get; }
        Guid Identifier { get; }
        string Name { get; }
        (Version, Version) SupportedVersionRange { get; }
        string Url { get; }
        Version Version { get; }
    }
}