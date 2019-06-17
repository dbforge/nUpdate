// ServerItem.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate.Administration.TransferInterface
{
    public class ServerItem
    {
        public ServerItem(string name, string fullPath, long size, DateTime? modified, ServerItemType itemType)
        {
            Name = name;
            FullPath = fullPath;
            Size = size;
            Modified = modified;
            ItemType = itemType;
        }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the full path.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        ///     Gets or sets the size of the file or directory.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        ///     Gets or sets the last modification date and time. Adjusted to from UTC (GMT) to local time.
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        ///     Gets or sets the item type.
        /// </summary>
        public ServerItemType ItemType { get; set; }
    }
}