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

            if (args.Length != 5)
            {
                MessageBox.Show("Invalid arguments.", "Startup failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PackageFile = args[0];
            AimFolder = args[1];
            ApplicationExecutablePath = args[2];
            AppName = args[3];
            Language = (Language) Enum.Parse(typeof (Language), args[4]);


            new Updater().RunUpdate();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
        }
    }
}