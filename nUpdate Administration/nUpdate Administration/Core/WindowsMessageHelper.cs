using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core
{
    internal class WindowsMessageHelper
    {
        public static int NewProjectArg;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int RegisterWindowMessage(string msgName);

        static WindowsMessageHelper()
        {
            NewProjectArg = WindowsMessageHelper.RegisterWindowMessage("NewProject");
        }

        public static int RegisterWndMsg(string Name)
        {
            return RegisterWindowMessage(Name);
        }

        public static void SendrWndMsg(string Title, int Identification)
        {
            SendMessage(Title, Identification, IntPtr.Zero, IntPtr.Zero);
        }

        public static bool SendMessage(string windowTitle, int msgId, IntPtr wParam, IntPtr lParam)
        {
            IntPtr WindowToFind = FindWindow(null, windowTitle);
            if (WindowToFind == IntPtr.Zero)
            {
                return false;
            }

            long result = SendMessage(WindowToFind, msgId, wParam, lParam);

            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
