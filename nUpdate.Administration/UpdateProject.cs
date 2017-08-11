// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Logging;
using nUpdate.Administration.Sql;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace nUpdate.Administration
{
    /// <summary>
    ///     Represents a local update project.
    /// </summary>
    [Serializable]
    public class UpdateProject : Model
    {
        private string _assemblyVersionPath;
        private List<UpdateChannel> _channels;
        private Guid _guid;
        private List<PackageActionLogData> _logData;
        private string _name;
        private List<UpdatePackage> _packages;
        private string _privateKey;
        private ProxyData _proxyData;
        private string _publicKey;
        private SqlData _sqlData;
        private string _transferAssemblyFilePath;
        private ITransferData _transferData;
        private TransferProtocol _transferProtocol;
        private Uri _updateDirectoryUri;
        private bool _useProxy;
        private bool _useStatistics;

        /// <summary>
        ///     Gets or sets the path of the file containing the <see cref="System.Reflection.Assembly" /> of the .NET project that
        ///     should be updated.
        /// </summary>
        public string AssemblyVersionPath
        {
            get => _assemblyVersionPath;
            set
            {
                _assemblyVersionPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the version of the project file.
        /// </summary>
        public Version Version => new Version(4, 0);

        /// <summary>
        ///     Gets or sets the <see cref="System.Guid" /> of the project.
        /// </summary>
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
        ///     Gets or sets the master channel of the project.
        /// </summary>
        public List<UpdateChannel> MasterChannel
        {
            get => _channels;
            set
            {
                _channels = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the name of the project.
        /// </summary>
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
        ///     Gets or sets the <see cref="Administration.ProxyData" /> that carries the necessary information for proxies.
        /// </summary>
        public ProxyData ProxyData
        {
            get => _proxyData;
            set
            {
                _proxyData = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the public key of the project.
        /// </summary>
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
        ///     Gets or sets the <see cref="Sql.SqlData" /> that carries the necessary information for statistics entries.
        /// </summary>
        public SqlData SqlData
        {
            get => _sqlData;
            set
            {
                _sqlData = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the path of the file containing an assembly that implements a custom transfer protocol.
        /// </summary>
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
        ///     Gets or sets the <see cref="Administration.TransferProtocol" /> that should be used for data transfers.
        /// </summary>
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
        ///     Gets or sets the <see cref="Uri" /> of the local or remote update directory of the project.
        /// </summary>
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
        ///     Gets or sets a value indicating whether a proxy should be used for data transfers, or not.
        /// </summary>
        public bool UseProxy
        {
            get => _useProxy;
            set
            {
                _useProxy = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether a statistics server should be used for the project, or not.
        /// </summary>
        public bool UseStatistics
        {
            get => _useStatistics;
            set
            {
                _useStatistics = value;
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
            {
                ProjectSession.AvailableLocations.Add(new UpdateProjectLocation(updateProject.Guid, path));
            }
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
        }

        /// <summary>
        ///     Saves the current <see cref="UpdateProject" /> at the specified path using its name as file identifier.
        /// </summary>
        public void Save(string path)
        {
            string filename = Name + ".nupdproj";
            File.WriteAllText(Path.Combine(path, filename), JsonSerializer.Serialize(this));
        }
    }
}