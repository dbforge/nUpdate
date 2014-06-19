using nUpdate.Administration.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.Administration
{
    static class Program 
    {
        /// <summary>
        /// The properties for our program to use.
        /// </summary>
        public static string Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "nUpdate Administration");
        public static string LanguageSerializerFilePath { get; set; }

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Type t = Type.GetType("Mono.Runtime");
            if (t != null)
                throw new Exception("Mono is not supported.");

            bool firstInstance = false;
            Mutex mtx = new Mutex(true, "MainForm", out firstInstance);

            if (firstInstance)
            {
                foreach (string arg in args)
                    MessageBox.Show(arg);

                Application.EnableVisualStyles();
                //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
                HandleCmdLineArgs();
        }

        public static void HandleCmdLineArgs()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                switch (Environment.GetCommandLineArgs()[1])
                {
                    case "-1":
                        WindowsMessageHelper.SendMessage("MainForm", WindowsMessageHelper.NewProjectArg, IntPtr.Zero, IntPtr.Zero);
                        break;
                }
            }
        }
    }
}
