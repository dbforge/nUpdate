// Author: Dominic Beger (Trade/ProgTrade)

using System;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.Models
{
    [Serializable]
    public class StatisticsServer : Model
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