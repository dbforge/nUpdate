// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using nUpdate.Administration.UserInterface.Controls;
using nUpdate.Administration.Win32;
// ReSharper disable InconsistentNaming
// ReSharper disable once IdentifierTypo

namespace nUpdate.Administration
{
    internal static class Extensions
    {
        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        internal static void DoubleBuffer(this Control control)
        {
            if (SystemInformation.TerminalServerSession)
                return;
            var dbProp = typeof (Control).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance);
            dbProp.SetValue(control, true, null);
        }

        internal static void HideCheckBox(this TreeNode node)
        {
            var tvi = new ExplorerTreeNode
            {
                HItem = node.Handle,
                Mask = TVIF_STATE,
                StateMask = TVIS_STATEIMAGEMASK,
                State = 0
            };
            NativeMethods.SendMessage(node.TreeView.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
        }

        internal static T[] RemoveAt<T>(this T[] source, int index)
        {
            var destination = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, destination, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, destination, index, source.Length - index - 1);

            return destination;
        }

        internal static T Remove<T>(this Stack<T> stack, T element)
        {
            var obj = stack.Pop();
            if (obj.Equals(element))
                return obj;
            T toReturn = stack.Remove(element);
            stack.Push(obj);
            return toReturn;
        }

        internal static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

        internal static string ConvertToInsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException(nameof(securePassword));

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        internal static void MoveUp(this TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = node.Index;
                if (index <= 0)
                    return;
                parent.Nodes.RemoveAt(index);
                parent.Nodes.Insert(index - 1, node);
            }
            else if (node.TreeView.Nodes.Contains(node))
            {
                int index = view.Nodes.IndexOf(node);
                if (index <= 0)
                    return;
                view.Nodes.RemoveAt(index);
                view.Nodes.Insert(index - 1, node);
            }
        }

        internal static void MoveDown(this TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = node.Index;
                if (index >= parent.Nodes.Count - 1)
                    return;
                parent.Nodes.RemoveAt(index);
                parent.Nodes.Insert(index + 1, node);
            }
            else if (view != null && view.Nodes.Contains(node))
            {
                int index = view.Nodes.IndexOf(node);
                if (index >= view.Nodes.Count - 1)
                    return;
                view.Nodes.RemoveAt(index);
                view.Nodes.Insert(index + 1, node);
            }
        }

        internal static bool IsValidPath(this string path)
        {
            var driveCheckRegEx = new Regex(@"^[a-zA-Z]:\\$");
            if (!driveCheckRegEx.IsMatch(path.Substring(0, 3)))
                return false;

            string invalidPathChars = new string(Path.GetInvalidPathChars());
            invalidPathChars += @":/?*" + "\"";
            var containsBadCharacterRegEx = new Regex("[" + Regex.Escape(invalidPathChars) + "]");
            if (containsBadCharacterRegEx.IsMatch(path.Substring(3, path.Length - 3)))
                return false;

            var directory = new DirectoryInfo(Path.GetFullPath(path));
            if (!directory.Exists)
                directory.Create();
            return true;
        }
    }
}