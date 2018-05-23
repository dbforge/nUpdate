// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.UI.Controls;
using Starksoft.Aspen.Ftps;
using TransferProgressEventArgs = nUpdate.Administration.TransferInterface.TransferProgressEventArgs;

namespace nUpdate.Administration.Core
{
    public static class Extensions
    {
        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        public static string ConvertToInsecureString(this SecureString securePassword)
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

        public static void DoubleBuffer(this Control control)
        {
            if (SystemInformation.TerminalServerSession)
                return;
            var dbProp = typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance);
            dbProp.SetValue(control, true, null);
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie) action(i);
        }

        public static void HideCheckBox(this TreeNode node)
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

        public static void MoveDown(this TreeNode node)
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

        public static void MoveUp(this TreeNode node)
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

        public static T Remove<T>(this Stack<T> stack, T element)
        {
            var obj = stack.Pop();
            if (obj.Equals(element))
                return obj;
            T toReturn = stack.Remove(element);
            stack.Push(obj);
            return toReturn;
        }

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            var dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static ServerItem ToServerItem(this FtpsItem ftpsItem)
        {
            var serverItemType = ServerItemType.Other;
            switch (ftpsItem.ItemType)
            {
                case FtpItemType.Directory:
                    serverItemType = ServerItemType.Directory;
                    break;
                case FtpItemType.File:
                    serverItemType = ServerItemType.File;
                    break;
            }

            return new ServerItem(ftpsItem.Name, ftpsItem.FullPath, ftpsItem.Size, ftpsItem.Modified, serverItemType);
        }

        public static TransferProgressEventArgs ToTransferInterfaceProgressEventArgs(
            this Starksoft.Aspen.Ftps.TransferProgressEventArgs progressEventArgs)
        {
            return new TransferProgressEventArgs(progressEventArgs.BytesTransferred,
                progressEventArgs.TotalBytesTransferred, progressEventArgs.BytesPerSecond,
                progressEventArgs.KilobytesPerSecond, progressEventArgs.MegabytesPerSecond,
                progressEventArgs.GigabytesPerSecond, progressEventArgs.ElapsedTime, progressEventArgs.PercentComplete,
                progressEventArgs.TransferSize, progressEventArgs.BytesRemaining, progressEventArgs.TimeRemaining);
        }
    }
}