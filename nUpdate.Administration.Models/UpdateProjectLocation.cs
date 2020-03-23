using System;
using Newtonsoft.Json;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.Models
{
    [Serializable]
    public class UpdateProjectLocation : NotifyPropertyChangedBase
    {
        [JsonConstructor]
        public UpdateProjectLocation(Guid guid, string lastSeenPath)
        {
            Guid = guid;
            LastSeenPath = lastSeenPath;
        }

        private Guid _guid;

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

        private string _lastSeenPath;

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