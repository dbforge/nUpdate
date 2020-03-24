// FtpData.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Security.Authentication;
using FluentFTP;
using Newtonsoft.Json;
using nUpdate.Administration.PluginBase.Models;

namespace nUpdate.Administration.Models.Ftp
{
    // ReSharper disable once InconsistentNaming
    public class FtpData : ITransferData
    {
        /// <summary>
        ///     Gets or sets a value indicating whetehr the correct connection settings should be auto-detected, or not.
        /// </summary>
        public bool AutoConnect { get; set; }

        /// <summary>
        ///     Gets or sets the directory.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        ///     Gets or sets the encryption mode.
        /// </summary>
        public FtpEncryptionMode EncryptionMode { get; set; }

        /// <summary>
        ///     Gets or sets the host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Gets the identifier for the key database.
        /// </summary>
        [JsonIgnore]
        public Uri Identifier => new Uri($"ftp://{Uri.EscapeDataString(Username)}@{Host}:{Port}/");

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Gets or sets the SslProtocol to use.
        /// </summary>
        public SslProtocols SslProtocols { get; set; }

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        public string Username { get; set; }
    }
}