using System;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Localization;

namespace nUpdate.UpdateInstaller
{
    internal static class Program
    {
        public static string PackageFile { get; set; }
        public static string AimFolder { get; set; }
        public static string ApplicationExecutablePath { get; set; }
        public static string AppName { get; set; }
        public static Language Language { get; set; }

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                MessageBox.Show("Invalid arguments.", "Startup failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (args.Length != 2)
            {
                MessageBox.Show("Invalid arguments.", "Startup failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] appArguments = args[1].Split(new char[] { '|' });

            PackageFile = appArguments[0];
            AimFolder = appArguments[1];
            ApplicationExecutablePath = appArguments[2];
            AppName = appArguments[3];
            Language = (Language)Enum.Parse(typeof(Language), appArguments[4]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Updater().RunUpdate();
        }
    }
}