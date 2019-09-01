// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using nUpdate.Administration.Common.Logging;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace nUpdate.Administration.Common
{
    /// <summary>
    ///     Represents a local update project.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UpdateProject : Model
    {
        private Uri _packageFileUri;
        private Guid _guid;
        private List<PackageActionLogData> _logData;
        private string _name;
        private List<UpdatePackage> _packages;
        private string _privateKey;
        private string _publicKey;
        private string _transferAssemblyFilePath;
        private ITransferData _transferData;
        private TransferProtocol _transferProtocol;
        private Uri _updateDirectoryUri;

        /// <summary>
        ///     Gets or sets the <see cref="System.Guid" /> of the project.
        /// </summary>
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

        /// <summary>
        ///     Gets the identifier for the key database.
        /// </summary>
        [JsonIgnore]
        public Uri Identifier => new Uri($"nupdproj://{Guid}");

        /// <summary>
        ///     Gets or sets the <see cref="PackageActionLogData" /> that carries information about the package history.
        /// </summary>
        [JsonProperty]
        public List<PackageActionLogData> LogData
        {
            get => _logData;
            set
            {
                _logData = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="Uri"/> of the package feed file of the project.
        /// </summary>
        [JsonProperty]
        public Uri PackageFileUri
        {
            get => _packageFileUri;
            set
            {
                _packageFileUri = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the name of the project.
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the available <see cref="UpdatePackage" />s of the project.
        /// </summary>
        public List<UpdatePackage> Packages
        {
            get => _packages;
            set
            {
                _packages = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the private key of the project for signing update packages.
        /// </summary>
        [JsonIgnore]
        public string PrivateKey
        {
            get => _privateKey;
            set
            {
                _privateKey = value;
                KeyManager.Instance[Identifier] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the public key of the project.
        /// </summary>
        [JsonProperty]
        public string PublicKey
        {
            get => _publicKey;
            set
            {
                _publicKey = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the path of the file containing an assembly that implements a custom transfer protocol.
        /// </summary>
        [JsonProperty]
        public string TransferAssemblyFilePath
        {
            get => _transferAssemblyFilePath;
            set
            {
                _transferAssemblyFilePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="ITransferData" /> that carries the necessary information for data transfers.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public ITransferData TransferData
        {
            get => _transferData;
            set
            {
                _transferData = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="Common.TransferProtocol" /> that should be used for data transfers.
        /// </summary>
        [JsonProperty]
        public TransferProtocol TransferProtocol
        {
            get => _transferProtocol;
            set
            {
                _transferProtocol = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Uri" /> of the local or remote update directory of the project.
        /// </summary>
        [JsonProperty]
        public Uri UpdateDirectory
        {
            get => _updateDirectoryUri;
            set
            {
                _updateDirectoryUri = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Loads an update project from the specified path.
        /// </summary>
        /// <param name="path">The path of the project file.</param>
        /// <returns>The loaded <see cref="UpdateProject" />.</returns>
        public static UpdateProject Load(string path)
        {
            var updateProject = JsonSerializer.Deserialize<UpdateProject>(File.ReadAllText(path));
            var currentProjectEntry =
                ProjectSession.AvailableLocations.FirstOrDefault(item => item.Guid == updateProject.Guid);
            if (currentProjectEntry == null)
                ProjectSession.AvailableLocations.Add(new UpdateProjectLocation(updateProject.Guid, path));
            else
            {
                if (currentProjectEntry.LastSeenPath != path)
                    currentProjectEntry.LastSeenPath = path;
            }

            return updateProject;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            PrivateKey = (string) KeyManager.Instance[Identifier];
        }

        /// <summary>
        ///     Saves the current <see cref="UpdateProject" /> under the last known file path.
        /// </summary>
        public void Save()
        {
            var updateProjectLocation = ProjectSession.AvailableLocations.FirstOrDefault(loc => loc.Guid == Guid);
            if (updateProjectLocation != null)
                File.WriteAllText(updateProjectLocation.LastSeenPath, JsonSerializer.Serialize(this));
            // TODO: Handle case that path is null
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(this));
        }
    }
}