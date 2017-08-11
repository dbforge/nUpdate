using System;
using nUpdate.Administration.Infrastructure;
using Newtonsoft.Json;

namespace nUpdate.Administration
{
    [Serializable]
    internal class UpdateProjectLocation : Model
    {
        [JsonConstructor]
        internal UpdateProjectLocation(Guid guid, string lastSeenPath)
        {
            Guid = guid;
            LastSeenPath = lastSeenPath;
        }

        private Guid _guid;

        [JsonProperty]
        internal Guid Guid
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
        internal string LastSeenPath
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