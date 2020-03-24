// UpdateChannelFilter.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Collections.Generic;
using System.Linq;

namespace nUpdate
{
    public class UpdateChannelFilter
    {
        public enum ChannelFilterMode
        {
            SearchOnlyInSpecified,
            ExcludeSpecifiedFromSearch
        }

        public UpdateChannelFilter(ChannelFilterMode mode, params string[] channel)
        {
            Mode = mode;
            Channels = channel;
        }

        public UpdateChannelFilter(ChannelFilterMode mode, IEnumerable<string> channels)
        {
            Mode = mode;
            Channels = channels;
        }

        public IEnumerable<string> Channels { get; set; }

        public ChannelFilterMode Mode { get; set; }

        public static UpdateChannelFilter None =>
            new UpdateChannelFilter(ChannelFilterMode.ExcludeSpecifiedFromSearch, Enumerable.Empty<string>());
    }
}