using nUpdate.Administration.Properties;

namespace nUpdate.Administration
{
    public static class FilePathProvider
    {
        public static string Path => Settings.Default.ProgramPath;
        public static string LanguagesDirectory => System.IO.Path.Combine(Path, "Localization");
        public static string ProjectsConfigFilePath => System.IO.Path.Combine(Path, "projconf.json");
        public static string StatisticServersFilePath => System.IO.Path.Combine(Path, "statservers.json");
    }
}