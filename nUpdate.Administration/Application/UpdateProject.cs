// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using nUpdate.Administration.History;
using nUpdate.Updating;
using Newtonsoft.Json;

namespace nUpdate.Administration.Application
{
    /// <summary>
    ///     Represents a local update project.
    /// </summary>
    [Serializable]
    public class UpdateProject : IDisposable
    {
        private bool _disposed;
        public string ConfigVersion => "4";

        /// <summary>
        ///     The path of the project.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     The name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The GUID of the project.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        ///     The application ID of the project for the MySQL-connections.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     The online update directory of the project.
        /// </summary>
        public string UpdateUrl { get; set; }

        /// <summary>
        ///     The private key of the project.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        ///     The public key of the project.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        ///     The path of the file containing an assembly for loading the version.
        /// </summary>
        public string AssemblyVersionPath { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the credentials should be saved or not.
        /// </summary>
        public bool SaveCredentials { get; set; }

        /// <summary>
        ///     The proxy to use, if wished.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     The username of the proxy to use, if necessary.
        /// </summary>
        public string ProxyUsername { get; set; }

        /// <summary>
        ///     The password for the proxy to use, if necessary. (Base64-encoded).
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        ///     Gets or sets the decrypted password for the proxy.
        /// </summary>
        [JsonIgnore]
        public SecureString RuntimeProxyPassword { get; set; }

        /// <summary>
        ///     Gets or sets the path of the file containing an assembly that implements custom transfer handlers for FTP.
        /// </summary>
        public string FtpTransferAssemblyFilePath { get; set; }

        /// <summary>
        ///     The FTP-host of the project.
        /// </summary>
        public string FtpHost { get; set; }

        /// <summary>
        ///     The FTP-port of the project.
        /// </summary>
        public int FtpPort { get; set; }

        /// <summary>
        ///     The FTP-username of the project.
        /// </summary>
        public string FtpUsername { get; set; }

        /// <summary>
        ///     Gets or sets the password of the FTP-server.
        /// </summary>
        public string FtpPassword { get; set; }

        /// <summary>
        ///     Gets or sets the decrypted password of the FTP-server.
        /// </summary>
        [JsonIgnore]
        public SecureString RuntimeFtpPassword { get; set; }

        /// <summary>
        ///     The FTP-protocol of the project.
        /// </summary>
        public int FtpProtocol { get; set; }

        /// <summary>
        ///     The option if passive mode should be used.
        /// </summary>
        public bool FtpUsePassiveMode { get; set; }

        /// <summary>
        ///     The FTP-directory of the project.
        /// </summary>
        public string FtpDirectory { get; set; }

        /// <summary>
        ///     The log of the project.
        /// </summary>
        public List<Log> Log { get; set; }

        /// <summary>
        ///     The created update packages.
        /// </summary>
        public List<UpdatePackage> Packages { get; set; }

        /// <summary>
        ///     Sets if a statistics server should be used.
        /// </summary>
        public bool UseStatistics { get; set; }

        /// <summary>
        ///     The url of the SQL-connection.
        /// </summary>
        public string SqlWebUrl { get; set; }

        /// <summary>
        ///     The name of the SQL-database to use.
        /// </summary>
        public string SqlDatabaseName { get; set; }

        /// <summary>
        ///     The username for the SQL-login.
        /// </summary>
        public string SqlUsername { get; set; }

        /// <summary>
        ///     Gets or sets the encrypted password of the SQL-server. (Base64-encoded)
        /// </summary>
        public string SqlPassword { get; set; }

        /// <summary>
        ///     Gets or sets the decrypted password of the SQL-server.
        /// </summary>
        [JsonIgnore]
        public SecureString RuntimeSqlPassword { get; set; }

        /// <summary>
        ///     Loads an update project.
        /// </summary>
        /// <param name="path">The path of the project file.</param>
        /// <returns>Returns the read update project.</returns>
        public static UpdateProject LoadProject(string path)
        {
            return Serializer.Deserialize<UpdateProject>(File.ReadAllText(path));
        }

        /// <summary>
        ///     Saves an update project.
        /// </summary>
        /// <param name="path">The path of the project file.</param>
        /// <param name="project">The project to save.</param>
        public static void SaveProject(string path, UpdateProject project)
        {
            var serializedContent = Serializer.Serialize(project);
            File.WriteAllText(path, serializedContent);
        }

        /// <summary>
        ///     Releases all managed and unmanaged resources used by the current <see cref="UpdateManager" />-instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            RuntimeSqlPassword.Dispose();
            _disposed = true;
        }
    }

    [Serializable]
    public class OldUpdatePackage
    {
        /// <summary>
        ///     The version of the package.
        /// </summary>
        public UpdateVersion Version { get; set; }

        /// <summary>
        ///     The description of the package.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The option if the package is released.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        ///     The local path of the package.
        /// </summary>
        public string LocalPackagePath { get; set; }
    }
}