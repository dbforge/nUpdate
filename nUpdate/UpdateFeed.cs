using System;

namespace nUpdate
{
    [Serializable]
    public class UpdateFeed
    {
        public string Name { get; set; }

        public Uri Feed { get; set; }

        public Version LatestVersion { get; set; }
    }
}