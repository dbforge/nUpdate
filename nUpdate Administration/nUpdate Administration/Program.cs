// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;

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
                ((Exception) e.ExceptionObject), PopupButtons.Ok);
            Application.Exit();
        }

        private static void UnhandledThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Popup.ShowPopup(SystemIcons.Error, "nUpdate has just noticed an unhandled error.",
                e.Exception, PopupButtons.Ok);
            Application.Exit();
        }

        /// <summary>
        ///     The root path of nUpdate Administration.
        /// </summary>
        public static string Path =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nUpdate Administration");

        /// <summary>
        ///     The path of the config file for all projects.
        /// </summary>
        public static string ProjectsConfigFilePath = System.IO.Path.Combine(Path, "projconf.json");

        /// <summary>
        ///     The path of the statistic server file.
        /// </summary>
        public static string StatisticServersFilePath = System.IO.Path.Combine(Path, "statservers.json");
    }
}