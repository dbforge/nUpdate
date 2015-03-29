// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Core;
using nUpdate.UpdateInstaller.Core.Operations;
using nUpdate.UpdateInstaller.UI.Popups;

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
        ///     The text of the "Extracting files..."-label.
        /// </summary>
        public static string ExtractFilesText { get; set; }

        /// <summary>
        ///     The text of the "Copying..."-label.
        /// </summary>
        public static string CopyingText { get; set; }

        /// <summary>
        ///     The text of the file rename information.
        /// </summary>
        public static string FileRenamingOperationText { get; set; }

        /// <summary>
        ///     The text of the file delete information.
        /// </summary>
        public static string FileDeletingOperationText { get; set; }

        /// <summary>
        ///     The text of the registry sub key creation information.
        /// </summary>
        public static string RegistrySubKeyCreateOperationText { get; set; }

        /// <summary>
        ///     The text of the registry name-value-pair value setting information.
        /// </summary>
        public static string RegistryNameValuePairSetValueOperationText { get; set; }

        /// <summary>
        ///     The text of the registry name-value-pair value deleting information.
        /// </summary>
        public static string RegistryNameValuePairDeleteValueOperationText { get; set; }

        /// <summary>
        ///     The text of the registry sub key deletion information.
        /// </summary>
        public static string RegistrySubKeyDeleteOperationText { get; set; }

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
        ///     The caption of the updating error message.
        /// </summary>
        public static string UpdatingErrorCaption { get; set; }

        /// <summary>
        ///     The caption of the initializing error message.
        /// </summary>
        public static string InitializingErrorCaption { get; set; }

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length != 1)
            {
                Popup.ShowPopup(SystemIcons.Error, "Updating the application has failed.",
                    String.Format("Invalid arguments count ({0}) where 1 argument was expected.", args.Length),
                    PopupButtons.Ok);
                return;
            }

            var appArguments = args[0].Split('|');

            try
            {
                PackageFile = appArguments[0];
                AimFolder = appArguments[1];
                ApplicationExecutablePath = appArguments[2];
                AppName = appArguments[3];
                Operations =
                    Serializer.Deserialize<IEnumerable<Operation>>(
                        Encoding.UTF8.GetString(Convert.FromBase64String(appArguments[4])));
                ExternalGuiAssemblyPath = appArguments[5];
                ExtractFilesText = appArguments[6];
                CopyingText = appArguments[7];
                FileDeletingOperationText = appArguments[8];
                FileRenamingOperationText = appArguments[9];
                RegistrySubKeyCreateOperationText = appArguments[10];
                RegistrySubKeyDeleteOperationText = appArguments[11];
                RegistryNameValuePairDeleteValueOperationText = appArguments[12];
                RegistryNameValuePairSetValueOperationText = appArguments[13];
                ProcessStartOperationText = appArguments[14];
                ProcessStopOperationText = appArguments[15];
                ServiceStartOperationText = appArguments[16];
                ServiceStopOperationText = appArguments[17];
                UpdatingErrorCaption = appArguments[18];
                InitializingErrorCaption = appArguments[19];
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(SystemIcons.Error, "Updating the application has failed.", ex, PopupButtons.Ok);
                return;
            }

            new Updater().RunUpdate();
        }
    }
}