// ListingItem.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Collections.Generic;

namespace nUpdate.Administration
{
    public class ListingItem
    {
        public ListingItem(string text, bool isDirectory)
            : this(text, isDirectory, new List<ListingItem>())
        {
        }

        public ListingItem(string text, bool isDirectory, List<ListingItem> children)
        {
            Text = text;
            Children = children;
            IsDirectory = isDirectory;
        }

        /// <summary>
        ///     The children of the current <see cref="ListingItem" />.
        /// </summary>
        public List<ListingItem> Children { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the current <see cref="ListingItem" /> is a directory or not.
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        ///     The text of the current <see cref="ListingItem" />.
        /// </summary>
        public string Text { get; set; }
    }
}