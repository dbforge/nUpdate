// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;

namespace nUpdate.Administration.BusinessLogic
{
    public static class PathProvider
    {
        public static string SettingsDirectoryFilePath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "nUpdate.Administration.GlobalSettings");
        public static string SettingsFilePath =
            System.IO.Path.Combine(SettingsDirectoryFilePath, "settings.dat");
        public static string DefaultProjectDirectory => SettingsManager.Instance["DefaultProjectPath"].ToString();
        public static string KeyDatabaseFilePath => System.IO.Path.Combine(Path, "keys.db");
        public static string LocalesDirectory => System.IO.Path.Combine(Path, "Locales");
        public static string Path => SettingsManager.Instance["ApplicationDataPath"].ToString();
        public static string ProjectsConfigFilePath => System.IO.Path.Combine(Path, "projconf.json");
        public static string StatisticServersFilePath => System.IO.Path.Combine(Path, "statservers.json");
    }
}