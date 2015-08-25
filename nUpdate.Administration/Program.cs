// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Exceptionless;
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
        public static string VersionString => "nUpdate Administration 4.0.0";

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

            //AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.UnhandledException += UnhandledException;
            //Application.ThreadException += UnhandledThreadException;
            System.Windows.Forms.Application.ApplicationExit += Exit;
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            ExceptionlessClient.Default.Register();

            var dialog = new MainDialog();
            if (args.Length == 1)
            {
                var file = new FileInfo(args[0]);
                if (file.Exists)
                    dialog.ProjectPath = file.FullName;
            }

            System.Windows.Forms.Application.Run(dialog);
        }

        private static void Exit(object sender, EventArgs e)
        {
            if (_mutex != null)
                _mutex.Dispose();
        }
    }
}