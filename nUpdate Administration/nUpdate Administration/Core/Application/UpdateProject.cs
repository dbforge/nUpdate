using System;
using System.Collections.Generic;
using System.Net;
using nUpdate.Administration.Core.Update.History;

namespace nUpdate.Administration.Core.Update
{
    [Serializable]
    internal class UpdateProject
    {
        /// <summary>
        ///     Sets if there are unsaved changes e.g. when interacting between dialogs.
        /// </summary>
        public bool HasUnsavedChanges { get; set; }

        /// <summary>
        ///     The path of the project.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     The name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The ID of the project.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The programming language of the project.
        /// </summary>
        public string ProgrammingLanguage { get; set; }

        /// <summary>
        ///     The update url of the project.
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
        ///     The path of the file containing an assembly for loading the version from.
        /// </summary>
        public string AssemblyVersionPath { get; set; }

        /// <summary>
        ///     The name of the SQL-server to use for the statistics, if set.
        /// </summary>
        public string SqlServer { get; set; }

        /// <summary>
        ///     The proxy to use, if wished.
        /// </summary>
        public WebProxy Proxy { get; set; }

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
        ///     The FTP-password of the project.
        /// </summary>
        public string FtpPassword { get; set; }

        /// <summary>
        ///     The FTP-protocol of the project.
        /// </summary>
        public string FtpProtocol { get; set; }

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
        ///     The amount of released packages.
        /// </summary>
        public int ReleasedPackages { get; set; }

        /// <summary>
        ///     The newest package released.
        /// </summary>
        public string NewestPackage { get; set; }

        /// <summary>
        ///     The created update packages.
        /// </summary>
        public List<UpdatePackage> Packages { get; set; }
    }
}