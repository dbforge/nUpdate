// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Core;
using nUpdate.UpdateInstaller.Core.Operations;
using nUpdate.UpdateInstaller.UI.Popups;

namespace nUpdate.UpdateInstaller
{
    internal static class Program
    {
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
        ///     Gets or sets the arguments to handle over to the application.
        /// </summary>
        public static List<UpdateArgument> Arguments { get; set; }

        /// <summary>
        ///     The text of the "Copying..."-label.
        /// </summary>
        public static string CopyingText { get; set; }

        /// <summary>
        ///     The path of the external GUI assembly.
        /// </summary>
        public static string ExternalGuiAssemblyPath { get; set; }

        /// <summary>
        ///     The text of the "Extracting files..."-label.
        /// </summary>
        public static string ExtractFilesText { get; set; }

        /// <summary>
        ///     The text of the file delete information.
        /// </summary>
        public static string FileDeletingOperationText { get; set; }

        /// <summary>
        ///     The text of the error that a file is currently being used by another program.
        /// </summary>
        public static string FileInUseError { get; set; }

        /// <summary>
        ///     The text of the file rename information.
        /// </summary>
        public static string FileRenamingOperationText { get; set; }

        /// <summary>
        ///     The caption of the initializing error message.
        /// </summary>
        public static string InitializingErrorCaption { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the host application has been closed, or not.
        /// </summary>
        public static bool IsHostApplicationClosed { get; set; }

        /// <summary>
        ///     The operations to perform.
        /// </summary>
        public static Dictionary<string, IEnumerable<Operation>> Operations { get; set; }

        /// <summary>
        ///     The paths of the package files.
        /// </summary>
        public static string[] PackageFilePaths { get; set; }

        /// <summary>
        ///     The text of the process start information.
        /// </summary>
        public static string ProcessStartOperationText { get; set; }

        /// <summary>
        ///     The text of the process stop information.
        /// </summary>
        public static string ProcessStopOperationText { get; set; }

        /// <summary>
        ///     The text of the registry name-value-pair value deleting information.
        /// </summary>
        public static string RegistryNameValuePairDeleteValueOperationText { get; set; }

        /// <summary>
        ///     The text of the registry name-value-pair value setting information.
        /// </summary>
        public static string RegistryNameValuePairSetValueOperationText { get; set; }

        /// <summary>
        ///     The text of the registry sub key creation information.
        /// </summary>
        public static string RegistrySubKeyCreateOperationText { get; set; }

        /// <summary>
        ///     The text of the registry sub key deletion information.
        /// </summary>
        public static string RegistrySubKeyDeleteOperationText { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the host application should be restarted, or not.
        /// </summary>
        public static bool RestartHostApplication { get; set; }

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

        private static void HandlerMethod(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is ThreadAbortException)
                return;
            var exception = e.ExceptionObject as Exception;
            if (exception != null) MessageBox.Show(exception.InnerException?.ToString() ?? exception.ToString());
            Application.Exit();
        }

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += HandlerMethod;

            if (args.Length != 1)
            {
                Popup.ShowPopup(SystemIcons.Error, "Updating the application has failed.",
                    $"Invalid arguments count ({args.Length}) where 1 argument was expected.",
                    PopupButtons.Ok);
                return;
            }

            var appArguments = args[0].Split('|');

            try
            {
                PackageFilePaths = appArguments[0].Split('%');
                AimFolder = appArguments[1];
                ApplicationExecutablePath = appArguments[2];
                AppName = appArguments[3];
                Operations =
                    Serializer.Deserialize<Dictionary<string, IEnumerable<Operation>>>(
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
                Arguments = Serializer.Deserialize<List<UpdateArgument>>(
                    Encoding.UTF8.GetString(Convert.FromBase64String(appArguments[20])));
                // Arguments-property can't be "null" as UpdateManager creates an instance of a List<UpdateArgument> and handles that over
                IsHostApplicationClosed = Convert.ToBoolean(appArguments[21]);
                RestartHostApplication = Convert.ToBoolean(appArguments[22]);
                FileInUseError = appArguments[23];
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