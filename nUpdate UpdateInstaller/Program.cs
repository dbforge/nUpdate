using nUpdate.UpdateInstaller.LanguageManagement;
using System;
using System.Windows.Forms;

namespace nUpdate.UpdateInstaller
{
    static class Program
    {
        public static string PackageFile { get; set; }
        public static string AimFolder { get; set; }
        public static string ApplicationExecutablePath { get; set; }
        public static string AppName { get; set; }
        public static LanguageManagement.Language Language { get; set; } 

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            PackageFile = args[0];
            AimFolder = args[1];
            ApplicationExecutablePath = args[2];
            AppName = args[3];
            Language = (Language)Enum.Parse(typeof(Language), args[4]);

            new Updater().RunUpdate();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
        }
    }
}
