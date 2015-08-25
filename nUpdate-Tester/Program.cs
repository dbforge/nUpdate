// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Linq;
using System.Windows.Forms;

namespace nUpdate.Tester
{
    internal static class Program
    {
        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Any())
            {
                string[] arguments = args[0].Split('|');
                foreach (string arg in arguments)
                    MessageBox.Show(arg);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.Run(new MainForm());
        }
    }
}