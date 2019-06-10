// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.UpdateInstaller
{
    internal static class Program
    {
        public static string PackageDirectory { get; set; }
        public static object NewUpdatePackages { get; internal set; }

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
            // ...
        }

        private static void HandlerMethod(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is ThreadAbortException)
                return;
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                MessageBox.Show(exception.InnerException?.ToString() ?? exception.ToString());
            }
            Application.Exit();
        }
    }
}