using System;
using System.Diagnostics;
using System.Globalization;
using System.Security;

namespace nUpdate.UpdateInstaller
{
    public static class WindowsEventLog
    {
        private const string DefaultSource = "nUpdate";

        // https://stackoverflow.com/questions/25725151/write-to-windows-application-event-log
        // Note: The actual limit is higher than this, but different Microsoft operating systems actually have different limits. So just use 30,000 to be safe.
        private const int MaxEventLogEntryLength = 30000;
        private static string _source;
        
        public static string Source
        {
            get
            {
                if (string.IsNullOrEmpty(_source))
                {
                    _source = DefaultSource;
                    CreateEventSource();
                }

                return _source;
            }
            set
            {
                _source = value;
                if (string.IsNullOrEmpty(_source)) _source = DefaultSource;

                CreateEventSource();
            }
        }
        
        private static void CreateEventSource()
        {
            try
            {
                // searching the source throws a security exception ONLY if not exists!
                if (!EventLog.SourceExists(_source))
                    EventLog.CreateEventSource(_source, "Application");
            }
            catch (SecurityException)
            {
                _source = "Application";
            }
        }

        public static void LogInformation(string message)
        {
            Log(message, EventLogEntryType.Information);
        }
        
        public static void LogWarning(string message)
        {
            Log(message, EventLogEntryType.Warning);
        }
        
        public static void LogError(string message)
        {
            Log(message, EventLogEntryType.Error);
        }

        public static void LogException(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            if (Environment.UserInteractive)
                Console.WriteLine(ex.ToString());

            Log(ex.ToString(), EventLogEntryType.Error);
        }

        private static void Log(string message, EventLogEntryType entryType)
        {
            var possiblyTruncatedMessage = EnsureLogMessageLimit(message);
            EventLog.WriteEntry(Source, possiblyTruncatedMessage, entryType);
        }

        private static string EnsureLogMessageLimit(string logMessage)
        {
            if (logMessage.Length <= MaxEventLogEntryLength)
                return logMessage;
            var truncateWarningText = string.Format(CultureInfo.CurrentCulture,
                "... | Log Message Truncated [ Limit: {0} ]", MaxEventLogEntryLength);

            logMessage = logMessage.Substring(0, MaxEventLogEntryLength - truncateWarningText.Length);
            logMessage = string.Format(CultureInfo.CurrentCulture, "{0}{1}", logMessage, truncateWarningText);

            return logMessage;
        }
    }
}