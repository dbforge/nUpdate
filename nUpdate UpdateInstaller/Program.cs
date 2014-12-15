using System;
using System.Collections.Generic;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Core;
using nUpdate.UpdateInstaller.Core.Operations;

namespace nUpdate.UpdateInstaller
{
    internal static class Program
    {
        /// <summary>
        ///     The path of the package file where the update files are stored.
        /// </summary>
        public static string PackageFile { get; set; }

        /// <summary>
        ///     The program folder where the updated files should be copied to.
        /// </summary>
        public static string AimFolder { get; set; }

        /// <summary>
        ///     The path of the program's executable file.
        /// </summary>
        public static string ApplicationExecutablePath { get; set; }

        /// <summary>
        ///     The name of the program.
        /// </summary>
        public static string AppName { get; set; }

        /// <summary>
        ///     The operations to perform.
        /// </summary>
        public static IEnumerable<Operation> Operations { get; set; }

        /// <summary>
        ///     The path of the external GUI assembly.
        /// </summary>
        public static string ExternalGuiAssemblyPath { get; set; }

        /// <summary>
        ///     The text of the "Updating..."-label.
        /// </summary>
        public static string UpdatingText { get; set; }

        /// <summary>
        ///     The text of the file rename information.
        /// </summary>
        public static string FileRenamingOperationText { get; set; }

        /// <summary>
        ///     The text of the file delete information.
        /// </summary>
        public static string FileDeletingOperationText { get; set; }

        /// <summary>
        ///     The text of the registry key creation information.
        /// </summary>
        public static string RegistryKeyCreateOperationText { get; set; }

        /// <summary>
        ///     The text of the registry key value setting information.
        /// </summary>
        public static string RegistryKeyValueSetOperationText { get; set; }

        /// <summary>
        ///     The text of the registry key deletion information.
        /// </summary>
        public static string RegistryKeyDeleteOperationText { get; set; }

        /// <summary>
        ///     The text of the process start information.
        /// </summary>
        public static string ProcessStartOperationText { get; set; }

        /// <summary>
        ///     The text of the process stop information.
        /// </summary>
        public static string ProcessStopOperationText { get; set; }

        /// <summary>
        ///     The text of the service start information.
        /// </summary>
        public static string ServiceStartOperationText { get; set; }

        /// <summary>
        ///     The text of the service stop information.
        /// </summary>
        public static string ServiceStopOperationText { get; set; }

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args == null || args.Length == 0)
            {
                MessageBox.Show("Invalid arguments.", "Startup failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (args.Length != 8)
            {
                MessageBox.Show("Invalid arguments.", "Startup failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] appArguments = args[1].Split(new[] { '|' });

            PackageFile = appArguments[0];
            AimFolder = appArguments[1];
            ApplicationExecutablePath = appArguments[2];
            AppName = appArguments[3];
            Operations = Serializer.Deserialize<IEnumerable<Operation>>(appArguments[4]);

            new Updater().RunUpdate();
        }
    }
}