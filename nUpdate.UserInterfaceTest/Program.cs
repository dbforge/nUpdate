// Program.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Windows.Forms;

namespace nUpdate.UserInterfaceTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainDialog());
        }
    }
}