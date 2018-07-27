using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace nUpdate.UpdateInstaller
{
    public static class WindowsEventLog
    {
        private const string DefaultSource = "nUpdate";
        private static string _Source;

        /// <summary>
        /// Gets or sets the source/caller.
        /// </summary>
        public static string Source
        {
            get
            {
                if (string.IsNullOrEmpty(_Source))
                {
                    _Source = DefaultSource;
                    CreateEventSourceIfnotExsists();
                }

                return _Source;
            }
            set
            {
                _Source = value;
                if (string.IsNullOrEmpty(_Source))
                {
                    _Source = DefaultSource;
                }

                CreateEventSourceIfnotExsists();
            }
        }
        // https://stackoverflow.com/questions/25725151/write-to-windows-application-event-log
        // Note: The actual limit is higher than this, but different Microsoft operating systems actually have different limits. So just use 30,000 to be safe.
        private const int MaxEventLogEntryLength = 30000;


        private static void CreateEventSourceIfnotExsists()
        {

            try
            {
                // searching the source throws a security exception ONLY if not exists!
                if (!EventLog.SourceExists(_Source))
                {   // no exception until yet means the user as admin privilege
                    EventLog.CreateEventSource(_Source, "Application");
                }
            }
            catch (SecurityException)
            {
                _Source = "Application";
            }
        }


        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogInformation(string message)
        {
            Log(message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogWarning(string message)
        {
            Log(message, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogError(string message)
        {
            Log(message, EventLogEntryType.Error);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="source">The name of the app/process calling the logging method. If not provided,
        /// an attempt will be made to get the name of the calling process.</param>
        public static void LogException(Exception ex)
        {
            if (ex == null) { throw new ArgumentNullException("ex"); }

            if (Environment.UserInteractive)
            {
                Console.WriteLine(ex.ToString());
            }

            Log(ex.ToString(), EventLogEntryType.Error);
        }


        private static void Log(string message, EventLogEntryType entryType)
        {
            string possiblyTruncatedMessage = EnsureLogMessageLimit(message);
            EventLog.WriteEntry(Source, possiblyTruncatedMessage, entryType);
        }

        private static string EnsureLogMessageLimit(string logMessage)
        {
            if (logMessage.Length > MaxEventLogEntryLength)
            {
                string truncateWarningText = string.Format(CultureInfo.CurrentCulture, "... | Log Message Truncated [ Limit: {0} ]", MaxEventLogEntryLength);

                logMessage = logMessage.Substring(0, MaxEventLogEntryLength - truncateWarningText.Length);
                logMessage = string.Format(CultureInfo.CurrentCulture, "{0}{1}", logMessage, truncateWarningText);
            }

            return logMessage;
        }
    }
}
