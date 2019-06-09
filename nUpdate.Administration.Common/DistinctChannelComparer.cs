using System.Collections.Generic;

namespace nUpdate.Administration.Common
{
    internal class DistinctChannelComparer : IEqualityComparer<DefaultUpdatePackage>
    {
        public bool Equals(DefaultUpdatePackage left, DefaultUpdatePackage right)
        {
            return left.ChannelName == right.ChannelName;
        }

        public int GetHashCode(DefaultUpdatePackage obj)
        {
            return obj.ChannelName.GetHashCode();
        }
    }
}