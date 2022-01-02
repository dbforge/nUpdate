// Log.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.History
{
    public class Log
    {
        /// <summary>
        ///     The entry that was made.
        /// </summary>
        public LogEntry Entry { get; set; }

        /// <summary>
        ///     The time when the entry was made.
        /// </summary>
        public string EntryTime { get; set; }

        /// <summary>
        ///     The version of the package that was given in the entry.
        /// </summary>
        public string PackageVersion { get; set; }

        /// <summary>
        ///     The name of the project that contains the log file.
        /// </summary>
        public UpdateProject Project { get; set; }

        /// <summary>
        ///     Gets or Set the Username that contains in the entry.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Writes an entry to the log.
        /// </summary>
        /// <param name="entry">The entry to set.</param>
        /// <param name="packageVersionString">The package version for the entry.</param>
        public void Write(LogEntry entry, string packageVersionString)
        {
            var log = new Log();
            log.EntryTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            log.Entry = entry;
            log.PackageVersion = packageVersionString;

            string userDomainName;

            try
            {
                userDomainName = Environment.UserDomainName;
            }
            catch (NotSupportedException e1)
            {
                userDomainName = null;
                Console.WriteLine(e1);
            }
            catch (Exception ex)
            {
                userDomainName = null;
                Popup.ShowPopup(null, SystemIcons.Error, "Error while get the current domainname.", ex,
                    PopupButtons.Ok);
            }

            log.Username = userDomainName == null ? Environment.UserName : $"{userDomainName}\\{Environment.UserName}";

            if (Project.Log == null)
                Project.Log = new List<Log>();
            Project.Log.Add(log);
        }
    }
}