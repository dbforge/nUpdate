using System.Collections.Generic;

namespace nUpdate.Administration.Core
{
    public class ListingItem
    {
        public ListingItem(string text)
        {
            Text = text;
        }

        /// <summary>
        ///     Gets or sets the text of the current item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Represents the sub items of the current item.
        /// </summary>
        public IEnumerable<ListingItem> SubItems { get; set; } 
    }
}
