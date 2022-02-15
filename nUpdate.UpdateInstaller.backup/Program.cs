﻿// Program.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.UpdateInstaller
{
    internal static class Program
    {
        public static string AppDirectory { get; internal set; }
        public static IEnumerable<UpdatePackage> NewUpdatePackages { get; internal set; }
        public static string PackageDirectory { get; set; }

        private static void HandlerMethod(object sender, UnhandledExceptionEventArgs e)
        {
            switch (e.ExceptionObject)
            {
                case ThreadAbortException _:
                    return;
                case Exception exception:
                    MessageBox.Show(exception.InnerException?.ToString() ?? exception.ToString());
                    break;
            }

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
                return;

            var appArguments = args[0].Split('|');
            PackageDirectory = appArguments[0];
            AppDirectory = appArguments[1];
            // ...
            UpdateInstaller.Instance.InstallUpdates();
        }
    }
}