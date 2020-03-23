using System;

namespace nUpdate.Administration.PluginBase
{
    public interface IPluginBase
    {
        Guid Identifier { get; }
        string Name { get; }
        string Author { get; }
        Version Version { get; }
        string Url { get; }
        string Description { get; }
        (Version, Version) SupportedVersionRange { get; }
    }
}
