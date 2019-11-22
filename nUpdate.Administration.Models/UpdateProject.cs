// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using nUpdate.Administration.Infrastructure;
using nUpdate.Administration.Models.Logging;

// ReSharper disable InconsistentNaming

namespace nUpdate.Administration.Models
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
        private string _publicKey;
        private string _transferAssemblyFilePath;
        private ITransferData _transferData;
        private Type _customTransferProviderClassType;
        private Uri _updateDirectoryUri;
        private UpdateProviderType _updateProviderType;

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
        ///     Gets or sets the transfer provider type that should be used for data transfers.
        /// </summary>
        [JsonProperty]
        public UpdateProviderType UpdateProviderType
        {
            get => _updateProviderType;
            set
            {
                _updateProviderType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="Type"/> of the custom transfer provider in the assembly that should be used for data transfers.
        /// </summary>
        [JsonProperty]
        public Type CustomTransferProviderClassType
        {
            get => _customTransferProviderClassType;
            set
            {
                _customTransferProviderClassType = value;
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
    }
}