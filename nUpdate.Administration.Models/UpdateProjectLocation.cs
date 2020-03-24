// UpdateProjectLocation.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using Newtonsoft.Json;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.Models
{
    [Serializable]
    public class UpdateProjectLocation : NotifyPropertyChangedBase
    {
        private Guid _guid;

        private string _lastSeenPath;

        [JsonConstructor]
        public UpdateProjectLocation(Guid guid, string lastSeenPath)
        {
            Guid = guid;
            LastSeenPath = lastSeenPath;
        }

        [JsonProperty]
        public Guid Guid
        {
            get => _guid;
            set
            {
                _guid = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty]
        public string LastSeenPath
        {
            get => _lastSeenPath;
            set
            {
                _lastSeenPath = value;
                OnPropertyChanged();
            }
        }
    }
}