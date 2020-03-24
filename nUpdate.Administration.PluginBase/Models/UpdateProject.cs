// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using nUpdate.Administration.Infrastructure;

// ReSharper disable InconsistentNaming

namespace nUpdate.Administration.PluginBase.Models
{
    /// <summary>
    ///     Represents a local update project.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UpdateProject : NotifyPropertyChangedBase
    {
        private Guid _guid;
        private string _name;
        private List<UpdatePackage> _packages;
        private string _publicKey;
        private ITransferData _transferData;
        private Uri _updateDirectoryUri;
        private Guid _updateProviderIdentifier;

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
        public Guid UpdateProviderIdentifier
        {
            get => _updateProviderIdentifier;
            set
            {
                _updateProviderIdentifier = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="Uri" /> of the local or remote update directory of the project.
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