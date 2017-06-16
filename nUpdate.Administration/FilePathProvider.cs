using System;
using nUpdate.Administration.Properties;

namespace nUpdate.Administration
{
    public static class FilePathProvider
    {
        public static string Path => Settings.Default.ProgramPath;
        public static string LocalesDirectory => System.IO.Path.Combine(Path, "Locales");
        public static string DefaultProjectsDirectory => System.IO.Path.Combine(Path, "Projects");
        public static string ProjectsConfigFilePath => System.IO.Path.Combine(Path, "projconf.json");
        public static string StatisticServersFilePath => System.IO.Path.Combine(Path, "statservers.json");

        public static string KeyDatabaseFilePath => System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nUpdate Administration", "keys.db");
    }
}