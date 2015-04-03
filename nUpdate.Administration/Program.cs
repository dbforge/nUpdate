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
            new Mutex(true, "MainForm", out firstInstance);

            if (!firstInstance)
                return;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledException;
            Application.ThreadException += UnhandledThreadException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            ExceptionlessClient.Current.Register();

            var dialog = new MainDialog();
            if (args.Length == 1)
            {
                var file = new FileInfo(args[0]);
                if (file.Exists)
                    dialog.ProjectPath = file.FullName;
            }

            Application.Run(dialog);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Popup.ShowPopup(SystemIcons.Error, "nUpdate has just noticed an unhandled error.",
                ((Exception)e.ExceptionObject), PopupButtons.Ok);
            var eventBuilder = ((Exception) e.ExceptionObject).ToExceptionless();
            var nUpdateVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(false)
                .OfType<nUpdateVersionAttribute>()
                .SingleOrDefault();
            if (nUpdateVersion != null)
                eventBuilder.SetVersion(
                    nUpdateVersion
                        .VersionString);
            eventBuilder.MarkAsCritical().Submit();
            ExceptionlessClient.Default.ProcessQueue();
            Application.Exit();
        }

        private static void UnhandledThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Popup.ShowPopup(SystemIcons.Error, "nUpdate has just noticed an unhandled error.",
                e.Exception, PopupButtons.Ok);
            var eventBuilder = e.Exception.ToExceptionless();
            var nUpdateVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(false)
                .OfType<nUpdateVersionAttribute>()
                .SingleOrDefault();
            if (nUpdateVersion != null)
                eventBuilder.SetVersion(
                    nUpdateVersion
                        .VersionString);
            eventBuilder.MarkAsCritical().Submit();
            ExceptionlessClient.Default.ProcessQueue();
            Application.Exit();
        }

        /// <summary>
        ///     The root path.
        /// </summary>
        public static string Path
        {
            get
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "nUpdate Administration");
            }
        }

        /// <summary>
        ///     The path of the config file for all projects.
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
            get { return "nUpdate Administration 1.0.0.0 Beta 1"; }
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