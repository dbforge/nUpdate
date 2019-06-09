// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Administration.Common
{
    [Serializable]
    public struct StatisticsServer
    {
        public StatisticsServer(Uri webUri, string databaseName, string username)
        {
            WebUri = webUri;
            DatabaseName = databaseName;
            Username = username;
        }

        public Uri WebUri { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
    }
}