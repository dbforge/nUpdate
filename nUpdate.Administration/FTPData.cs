using nUpdate.Administration.Ftp;

namespace nUpdate.Administration
{
    // ReSharper disable once InconsistentNaming
    internal struct FTPData : ITransferData
    {
        /// <summary>
        ///     Gets or sets the specific FTP protocol to use.
        /// </summary>
        public FtpSecurityProtocol FtpSpecificProtocol { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether passive move should be used, or not.
        /// </summary>
        public bool UsePassiveMode { get; set; }

        /// <summary>
        ///     Gets or sets the host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Gets or sets the directory.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the password. (Decrypted and Base64 encoded)
        /// </summary>
        public string Password { get; set; }
    }
}