using System;
using Newtonsoft.Json;

namespace nUpdate.Administration.Application
{
    [Serializable]
    internal class UpdateProjectLocation : PropertyChangedBase
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
            get { return _guid; }
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
            get { return _lastSeenPath; }
            set
            {
                _lastSeenPath = value;
                OnPropertyChanged();
            }
        }
    }
}