// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Exceptionless;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using nUpdate.Core;

namespace nUpdate.Administration
{
    public static class Program
    {
        private static Mutex _mutex;

        /// <summary>
        ///     The path of the languages directory.
        /// </summary>
        public static string LanguagesDirectory { get; set; }

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

            //AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.UnhandledException += UnhandledException;
            //Application.ThreadException += UnhandledThreadException;
            Application.ApplicationExit += Exit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            ExceptionlessClient.Default.Register();

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

        //private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    HandleException((Exception)e.ExceptionObject);
        //}

        //private static void UnhandledThreadException(object sender, ThreadExceptionEventArgs e)
        //{
        //    HandleException(e.Exception);
        //}

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
        public static string Path
        {
            get { return Properties.Settings.Default.ProgramPath; }
        }

        /// <summary>
        ///     The path of the configuration file for all projects.
        /// </summary>
        public static string ProjectsConfigFilePath
        {
            get { return System.IO.Path.Combine(Path, "projconf.json"); }
        }

        /// <summary>
        ///     The path of the statistic server file.
        /// </summary>
        public static string StatisticServersFilePath
        {
            get { return System.IO.Path.Combine(Path, "statservers.json"); }
        }

        /// <summary>
        ///     The version string shown in all dialog titles.
        /// </summary>
        public static string VersionString
        {
            get { return "nUpdate Administration 2.0.0.0 Beta 4"; }
        }

        public static string AesKeyPassword
        {
            get { return "VZh7mLRPNI"; }
        }

        public static string AesIvPassword
        {
            get { return "cOijH2vgwR"; }
        }
    }
}