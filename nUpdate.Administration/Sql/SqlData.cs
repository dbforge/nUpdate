// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Administration.Sql
{
    public struct SqlData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlData"/> struct.
        /// </summary>
        /// <param name="webUri">The <see cref="Uri"/> for creating the SQL connections.</param>
        /// <param name="databaseName">The name of the database that should be used for the statistics.</param>
        /// <param name="username">The password for the login. (Base64-encoded)</param>
        /// <param name="password">The SQL password.</param>
        /// <param name="entryMethod">The <see cref="StatisticsEntryMethod"/> that defines, how the statistics entries are made.</param>
        public SqlData(Uri webUri, string databaseName, string username, string password, StatisticsEntryMethod entryMethod)
        {
            WebUri = webUri;
            DatabaseName = databaseName;
            Username = username;
            Password = password;
            EntryMethod = entryMethod;
        }

        /// <summary>
        ///     Gets or sets the <see cref="Uri"/> for creating the SQL connections.
        /// </summary>
        internal Uri WebUri { get; set; }

        /// <summary>
        ///     Gets or sets the name of the database that should be used for the statistics.
        /// </summary>
        internal string DatabaseName { get; set; }

        /// <summary>
        ///     Gets or sets the username for the login.
        /// </summary>
        internal string Username { get; set; }

        /// <summary>
        ///     Gets or sets the password for the login. (Base64-encoded)
        /// </summary>
        internal string Password { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="StatisticsEntryMethod"/> that defines, how the statistics entries are made.
        /// </summary>
        internal StatisticsEntryMethod EntryMethod { get; set; }
    }
}