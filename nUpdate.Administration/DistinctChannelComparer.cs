using System.Collections.Generic;

namespace nUpdate.Administration
{
    internal class DistinctChannelComparer : IEqualityComparer<UpdatePackage>
    {
        public bool Equals(UpdatePackage left, UpdatePackage right)
        {
            return left.ChannelName == right.ChannelName;
        }

        public int GetHashCode(UpdatePackage obj)
        {
            return obj.ChannelName.GetHashCode();
        }
    }
}