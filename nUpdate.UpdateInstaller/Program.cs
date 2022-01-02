// Program.cs, 01.08.2018
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.UpdateInstaller
{
    internal static class Program
    {
        /// <summary>
        ///     The paths of the package files.
        /// </summary>
        public static string[] PackageFilePaths { get; set; }

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

        public static string RegistryNameValuePairDeleteValueOperationText { get; set; }

        /// <summary>
        ///     The paths of the package files.
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
        ///     Gets or sets the arguments to handle over to the application.
        /// </summary>
        public static List<UpdateArgument> Arguments { get; set; }

        /// <summary>
        ///     Gets or sets the host application options after the update installation.
        /// </summary>
        public static HostApplicationOptions HostApplicationOptions { get; set; }

        // Deprecated, there for compatiblity
        public static Dictionary<string, IEnumerable<Operation>> Operations { get; set; }

        public static int HostProcessId { get; set; }

        /// <summary>
        ///     The text of the error that a file is currently being used by another program.
        /// </summary>
        public static string FileInUseError { get; set; }

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += ApplicationOnThreadException;
            AppDomain.CurrentDomain.UnhandledException += HandlerMethod;

            if (args.Length != 1)
            {
                MessageBox.Show($"Invalid arguments count ({args.Length}) where 1 argument was expected.",
                    "Updating the application has failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var appArguments = args[0].Split('|');

            try
            {
                PackageFilePaths = appArguments[0].Split('%');
                AimFolder = appArguments[1];
                ApplicationExecutablePath = appArguments[2];
                AppName = appArguments[3];
                // Argument 4 became deprecated, but for compatibility reasons we need to have this
                Operations = appArguments[4].Equals(string.Empty)
                    ? null
                    : Serializer.Deserialize<Dictionary<string, IEnumerable<Operation>>>(Encoding.UTF8.GetString(Convert.FromBase64String(appArguments[4])));
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
                HostApplicationOptions =
                    (HostApplicationOptions) Enum.Parse(typeof(HostApplicationOptions), appArguments[21]);
                FileInUseError = appArguments[22];
                HostProcessId = int.Parse(appArguments[23]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, "Error while updating the application.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            new Updater().RunUpdate();
        }

        private static void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + "\n" + e.Exception.StackTrace, "Error while updating the application.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void HandlerMethod(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error while updating the application.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}