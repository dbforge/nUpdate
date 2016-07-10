// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UserInterface.Dialogs;

//using nUpdate.Administration.UI.Dialogs;

namespace nUpdate.Administration
{
    public static class Program
    {
        private static Mutex _mutex;

        public static string VersionString => "nUpdate Administration 4.0.0";

        public static string AesKeyPassword => "VZh7mLRPNI";

        public static string AesIvPassword => "cOijH2vgwR";

        public static CultureInfo Language => Settings.Default.Language;

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            // Check if the OS is supported...
            if (Environment.OSVersion.Version.Major < 6)
            {
                var dr = MessageBox.Show("Your operating system is not supported.", string.Empty,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                    Application.Exit();
            }

            // ... and if there is no other instance running.
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
            //ExceptionlessClient.Default.Register();
            
            // A path is handled over to the application when double-clicking a ".nupdproj"-file...
            if (args.Length == 1)
            {
                var file = new FileInfo(args[0]);
                if (file.Exists)
                {
                    Application.Run(new MainDialog(file.FullName));
                    return;
                }
            }

            Application.Run(new MainDialog(string.Empty));
        }

        private static void Exit(object sender, EventArgs e)
        {
            _mutex?.Dispose();
        }
    }
}