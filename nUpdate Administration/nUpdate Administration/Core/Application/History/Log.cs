using System;
using System.Collections.Generic;

namespace nUpdate.Administration.Core.Application.History
{
    internal class Log
    {
        /// <summary>
        /// The name of the project that contains the log file.
        /// </summary>
        public UpdateProject Project { get; set; }

        /// <summary>
        /// The time when the entry was made.
        /// </summary>
        public string EntryTime { get; set; }

        /// <summary>
        /// The entry that was made.
        /// </summary>
        public LogEntry Entry { get; set; }

        /// <summary>
        /// The version of the package that was given in the entry.
        /// </summary>
        public string PackageVersion { get; set; }

        /// <summary>
        /// Writes an entry to the log.
        /// </summary>
        /// <param name="entry">The entry to set.</param>
        /// <param name="packageVersionString">The package version for the entry.</param>
        public void Write(LogEntry entry, string packageVersionString)
        {
            Log log = new Log();
            log.EntryTime = DateTime.Now.ToString();
            log.Entry = entry;
            log.PackageVersion = packageVersionString;

            if (this.Project.Log == null)
            {
                this.Project.Log = new List<Log>();
            }
            this.Project.Log.Add(log);
        }
    }
}
