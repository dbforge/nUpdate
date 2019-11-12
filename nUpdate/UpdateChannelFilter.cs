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

        public static UpdateChannelFilter None =>
            new UpdateChannelFilter(ChannelFilterMode.ExcludeSpecifiedFromSearch, Enumerable.Empty<string>());

        public IEnumerable<string> Channels { get; set; }

        public ChannelFilterMode Mode { get; set; }

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
    }
}
