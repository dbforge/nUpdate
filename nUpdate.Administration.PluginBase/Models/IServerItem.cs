// IServerItem.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Administration.PluginBase.Models
{
    public interface IServerItem
    {
        /// <summary>
        ///     Gets the item type.
        /// </summary>
        ServerItemType ItemType { get; }

        /// <summary>
        ///     Gets the last modification date and time. Adjusted to from UTC (GMT) to local time.
        /// </summary>
        DateTime? Modified { get; }

        /// <summary>
        ///     Gets the name of the item.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the size of the file or directory.
        /// </summary>
        long Size { get; }
    }
}