// StatisticsServer.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.Models
{
    [Serializable]
    public class StatisticsServer : NotifyPropertyChangedBase
    {
        public StatisticsServer(Uri webUri, string databaseName, string username)
        {
            WebUri = webUri;
            DatabaseName = databaseName;
            Username = username;
        }

        public string DatabaseName { get; set; }
        public string Username { get; set; }

        public Uri WebUri { get; set; }
    }
}