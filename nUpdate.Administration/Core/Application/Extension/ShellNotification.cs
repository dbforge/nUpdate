// Copyright © Dominic Beger 2018

using System;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.Core.Application.Extension
{
    public class ShellNotification
    {
        /// <summary>
        ///     Notify shell of change of file associations.
        /// </summary>
        public static void NotifyOfChange()
        {
            NativeMethods.SHChangeNotify((uint) ShellChangeNotificationEvents.ShcneAssocchanged,
                (uint) (ShellChangeNotificationFlags.ShcnfIdlist | ShellChangeNotificationFlags.ShcnfFlushnowait),
                IntPtr.Zero, IntPtr.Zero);
        }

        [Flags]
        private enum ShellChangeNotificationEvents : uint
        {
            /// <summary>
            ///     The name of a nonfolder item has changed. SHCNF_IDLIST or  SHCNF_PATH must be specified in uFlags. dwItem1 contains
            ///     the  previous PIDL or name of the item. dwItem2 contains the new PIDL or name of the item.
            /// </summary>
            ShcneRenameitem = 0x00000001,

            /// <summary>
            ///     A nonfolder item has been created. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1 contains the
            ///     item that was created. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneCreate = 0x00000002,

            /// <summary>
            ///     A nonfolder item has been deleted. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1 contains the
            ///     item that was deleted. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneDelete = 0x00000004,

            /// <summary>
            ///     A folder has been created. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1 contains the folder that
            ///     was created. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneMkdir = 0x00000008,

            /// <summary>
            ///     A folder has been removed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1 contains the folder that
            ///     was removed. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneRmdir = 0x00000010,

            /// <summary>
            ///     Storage media has been inserted into a drive. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1
            ///     contains the root of the drive that contains the new media. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneMediainserted = 0x00000020,

            /// <summary>
            ///     Storage media has been removed from a drive. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1
            ///     contains the root of the drive from which the media was removed. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneMediaremoved = 0x00000040,

            /// <summary>
            ///     A drive has been removed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1 contains the root of the
            ///     drive that was removed. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneDriveremoved = 0x00000080,

            /// <summary>
            ///     A drive has been added. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1 contains the root of the
            ///     drive that was added. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneDriveadd = 0x00000100,

            /// <summary>
            ///     A folder on the local computer is being shared via the network. SHCNF_IDLIST or SHCNF_PATH must be specified in
            ///     uFlags. dwItem1 contains the folder that is being shared. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneNetshare = 0x00000200,

            /// <summary>
            ///     A folder on the local computer is no longer being shared via the network. SHCNF_IDLIST or SHCNF_PATH must be
            ///     specified in uFlags. dwItem1 contains the folder that is no longer being shared. dwItem2 is not used and should be
            ///     NULL.
            /// </summary>
            ShcneNetunshare = 0x00000400,

            /// <summary>
            ///     The attributes of an item or folder have changed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1
            ///     contains the item or folder that has changed. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneAttributes = 0x00000800,

            /// <summary>
            ///     The contents of an existing folder have changed, but the folder still exists and has not been renamed. SHCNF_IDLIST
            ///     or SHCNF_PATH must be specified in uFlags. dwItem1 contains the folder that has changed. dwItem2 is not used and
            ///     should be NULL. If a folder has been created, deleted, or renamed, use SHCNE_MKDIR, SHCNE_RMDIR, or
            ///     SHCNE_RENAMEFOLDER, respectively, instead.
            /// </summary>
            ShcneUpdatedir = 0x00001000,

            /// <summary>
            ///     An existing nonfolder item has changed, but the item still exists and has not been renamed. SHCNF_IDLIST or
            ///     SHCNF_PATH must be specified in uFlags. dwItem1 contains the item that has changed. dwItem2 is not used and should
            ///     be NULL. If a nonfolder item has been created, deleted, or renamed, use SHCNE_CREATE, SHCNE_DELETE, or
            ///     SHCNE_RENAMEITEM, respectively, instead.
            /// </summary>
            ShcneUpdateitem = 0x00002000,

            /// <summary>
            ///     The computer has disconnected from a server. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1
            ///     contains the server from which the computer was disconnected. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneServerdisconnect = 0x00004000,

            /// <summary>
            ///     An image in the system image list has changed. SHCNF_DWORD must be specified in uFlags. dwItem1 contains the index
            ///     in the system image list that has changed. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneUpdateimage = 0x00008000,

            /// <summary>
            ///     A drive has been added and the Shell should create a new window for the drive. SHCNF_IDLIST or SHCNF_PATH must be
            ///     specified in uFlags. dwItem1 contains the root of the drive that was added. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneDriveaddgui = 0x00010000,

            /// <summary>
            ///     The name of a folder has changed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1 contains the
            ///     previous pointer to an item identifier list (PIDL) or name of the folder. dwItem2 contains the new PIDL or name of
            ///     the folder.
            /// </summary>
            ShcneRenamefolder = 0x00020000,

            /// <summary>
            ///     The amount of free space on a drive has changed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1
            ///     contains the root of the drive on which the free space changed. dwItem2 is not used and should be NULL.
            /// </summary>
            ShcneFreespace = 0x00040000,

            /// <summary>
            ///     Not currently used.
            /// </summary>
            ShcneExtendedEvent = 0x04000000,

            /// <summary>
            ///     A file type association has changed. SHCNF_IDLIST must be specified in the uFlags parameter. dwItem1 and dwItem2
            ///     are not used and must be NULL.
            /// </summary>
            ShcneAssocchanged = 0x08000000,

            /// <summary>
            ///     Specifies a combination of all of the disk event identifiers.
            /// </summary>
            ShcneDiskevents = 0x0002381F,

            /// <summary>
            ///     Specifies a combination of all of the global event identifiers.
            /// </summary>
            ShcneGlobalevents = 0x0C0581E0,

            /// <summary>
            ///     All events have occurred.
            /// </summary>
            ShcneAllevents = 0x7FFFFFFF,

            /// <summary>
            ///     The specified event occurred as a result of a system interrupt. As this value modifies other event values, it
            ///     cannot be used alone.
            /// </summary>
            ShcneInterrupt = 0x80000000
        }

        [Flags]
        private enum ShellChangeNotificationFlags
        {
            /// <summary>
            ///     dwItem1 and dwItem2 are the addresses of ITEMIDLIST structures that represent the item(s) affected by the change.
            ///     Each ITEMIDLIST must be relative to the desktop folder.
            /// </summary>
            ShcnfIdlist = 0x0000,

            /// <summary>
            ///     dwItem1 and dwItem2 are the addresses of null-terminated strings of maximum length MAX_PATH that contain the full
            ///     path names of the items affected by the change.
            /// </summary>
            ShcnfPatha = 0x0001,

            /// <summary>
            ///     dwItem1 and dwItem2 are the addresses of null-terminated strings that represent the friendly names of the
            ///     printer(s) affected by the change.
            /// </summary>
            ShcnfPrintera = 0x0002,

            /// <summary>
            ///     The dwItem1 and dwItem2 parameters are DWORD values.
            /// </summary>
            ShcnfDword = 0x0003,

            /// <summary>
            ///     like SHCNF_PATHA but unicode string
            /// </summary>
            ShcnfPathw = 0x0005,

            /// <summary>
            ///     like SHCNF_PRINTERA but unicode string
            /// </summary>
            ShcnfPrinterw = 0x0006,

            /// <summary>
            /// </summary>
            ShcnfType = 0x00FF,

            /// <summary>
            ///     The function should not return until the notification has been delivered to all affected components. As this flag
            ///     modifies other data-type flags, it cannot by used by itself.
            /// </summary>
            ShcnfFlush = 0x1000,

            /// <summary>
            ///     The function should begin delivering notifications to all affected components but should return as soon as the
            ///     notification process has begun. As this flag modifies other data-type flags, it cannot by used  by itself.
            /// </summary>
            ShcnfFlushnowait = 0x2000
        }
    }
}