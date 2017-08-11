// Author: Dominic Beger (Trade/ProgTrade) 2017

using nUpdate.Administration.Properties;

namespace nUpdate.Administration
{
    public static class PathProvider
    {
        public static string DefaultProjectDirectory => Settings.Default.DefaultProjectPath;
        public static string KeyDatabaseFilePath => System.IO.Path.Combine(Path, "keys.db");
        public static string LocalesDirectory => System.IO.Path.Combine(Path, "Locales");
        public static string Path => Settings.Default.ApplicationDataPath;
        public static string ProjectsConfigFilePath => System.IO.Path.Combine(Path, "projconf.json");
        public static string StatisticServersFilePath => System.IO.Path.Combine(Path, "statservers.json");
    }
}