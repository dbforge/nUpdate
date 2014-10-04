// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration
{
    internal static class Program
    {
        /// <summary>
        ///     The root path of nUpdate Administration.
        /// </summary>
        public static string Path =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nUpdate Administration");

        /// <summary>
        ///     The path of the config file for all projects.
        /// </summary>
        public static string ProjectsConfigFilePath = System.IO.Path.Combine(Path, "projconf.txt");

        /// <summary>
        ///     The path of the statistic server file.
        /// </summary>
        public static string StatisticServersFilePath = System.IO.Path.Combine(Path, "statservers.txt");

        /// <summary>
        ///     The currently existing projects.
        /// </summary>
        public static Dictionary<string, string> ExisitingProjects = new Dictionary<string, string>();

        /// <summary>
        ///     The FTP-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public static SecureString FtpPassword = new SecureString();

        /// <summary>
        ///     The MySQL-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public static SecureString SqlPassword = new SecureString();

        /// <summary>
        ///     The proxy-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public static SecureString ProxyPassword = new SecureString();

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

            if (firstInstance)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += UnhandledException;
                Application.ThreadException += UnahndledThreadException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ApplicationExit += OnExit;
                Application.Run(new MainDialog());
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Popup.ShowPopup(SystemIcons.Error, "nUpdate has just noticed an unhandled error.",
                ((Exception) e.ExceptionObject), PopupButtons.Ok);
            Application.Exit();
        }

        private static void UnahndledThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Popup.ShowPopup(SystemIcons.Error, "nUpdate has just noticed an unhandled error.",
                    e.Exception, PopupButtons.Ok);
            Application.Exit();
        }

        private static void OnExit(object sender, EventArgs e)
        {
            FtpPassword.Dispose();
            ProxyPassword.Dispose();
            SqlPassword.Dispose();
            Application.Exit();
        }
    }
}