// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Dialogs;

namespace nUpdate.Administration
{
    public static class Program
    {
        private static Mutex _mutex;

        /// <summary>
        ///     The path of the languages directory.
        /// </summary>
        public static string LanguagesDirectory { get; set; }

        //private static void HandleException(Exception ex)
        //{
        //    Popup.ShowPopup(SystemIcons.Error, "nUpdate has just noticed an unhandled error.",
        //        (ex), PopupButtons.Ok);
        //    var eventBuilder = (ex).ToExceptionless();
        //    var nUpdateVersion = Assembly.GetExecutingAssembly()
        //        .GetCustomAttributes(false)
        //        .OfType<nUpdateVersionAttribute>()
        //        .SingleOrDefault();
        //    if (nUpdateVersion != null)
        //        eventBuilder.SetVersion(
        //            nUpdateVersion
        //                .VersionString);
        //    eventBuilder.MarkAsCritical().Submit();
        //    ExceptionlessClient.Default.ProcessQueue();
        //    Application.Exit();
        //}

        /// <summary>
        ///     The root path of the locally stored data.
        /// </summary>
        public static string Path => Settings.Default.ProgramPath;

        /// <summary>
        ///     The path of the configuration file for all projects.
        /// </summary>
        public static string ProjectsConfigFilePath => System.IO.Path.Combine(Path, "projconf.json");

        /// <summary>
        ///     The path of the statistic server file.
        /// </summary>
        public static string StatisticServersFilePath => System.IO.Path.Combine(Path, "statservers.json");

        /// <summary>
        ///     The version string shown in all dialog titles.
        /// </summary>
        public static string VersionString => "nUpdate Administration v3.1.2";

        public static string AesKeyPassword => "VZh7mLRPNI";
        public static string AesIvPassword => "cOijH2vgwR";

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            bool firstInstance;
            _mutex = new Mutex(true, "MainForm", out firstInstance);

            if (!firstInstance)
                return;

            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledException;
            Application.ThreadException += UnhandledThreadException;
            Application.ApplicationExit += Exit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            var dialog = new MainDialog();
            if (args.Length == 1)
            {
                var file = new FileInfo(args[0]);
                if (file.Exists)
                    dialog.ProjectPath = file.FullName;
            }

            Application.Run(dialog);
        }

        private static void Exit(object sender, EventArgs e)
        {
            if (_mutex != null)
                _mutex.Dispose();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            MessageBox.Show(
                $"nUpdate Administration has encountered an unhandled error and needs to exit:\n{exception.Message}\n{exception.StackTrace}",
                "Unhandled Exception in nUpdate Administration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        private static void UnhandledThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var exception = e.Exception;
            MessageBox.Show(
                $"nUpdate Administration has encountered an unhandled error and needs to exit:\n{exception.Message}\n{exception.StackTrace}",
                "Unhandled Exception in nUpdate Administration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }
}