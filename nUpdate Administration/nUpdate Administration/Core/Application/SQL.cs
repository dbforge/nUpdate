// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 20:13
namespace nUpdate.Administration.Core.Application
{
    internal class Sql
    {
        /// <summary>
        ///     The url of the SQL-connection.
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        ///     The name of the database to use.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     The username for the SQL-login.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The password for the SQL-login. (Base64 encoded)
        /// </summary>
        public string Password { get; set; }
    }
}