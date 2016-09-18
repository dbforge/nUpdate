// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace nUpdate.Administration.TransferInterface
{
    /// <summary>
    ///     Ftp item list.
    /// </summary>
    public class FtpItemCollection : IEnumerable<FtpItem>
    {
        private const string COL_NAME = "Name";
        private const string COL_MODIFIED = "Modified";
        private const string COL_SIZE = "Size";
        private const string COL_SYMBOLIC_LINK = "SymbolicLink";
        private const string COL_TYPE = "Type";
        private const string COL_ATTRIBUTES = "Attributes";
        private const string COL_RAW_TEXT = "RawText";
        private readonly List<FtpItem> _list = new List<FtpItem>();

        /// <summary>
        ///     Default constructor for FtpItemCollection.
        /// </summary>
        public FtpItemCollection()
        {
        }

        /// <summary>
        ///     Split a multi-line file list text response and add the parsed items to the collection.
        /// </summary>
        /// <param name="path">Path to the item on the FTP server.</param>
        /// <param name="fileList">The multi-line file list text from the FTP server.</param>
        /// <param name="itemParser">Line item parser object used to parse each line of fileList data.</param>
        public FtpItemCollection(string path, string fileList, IFtpItemParser itemParser)
        {
            Parse(path, fileList, itemParser);
        }

        /// <summary>
        ///     Gets the size, in bytes, of all files in the collection as reported by the FTP server.
        /// </summary>
        public long TotalSize { get; private set; }

        /// <summary>
        ///     Gets the number of elements actually contained in the FtpItemCollection list.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        ///     Gets an FtpItem from the FtpItemCollection based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>FtpItem</returns>
        public FtpItem this[int index] => _list[index];

        IEnumerator<FtpItem> IEnumerable<FtpItem>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        ///     Merges two FtpItemCollection together into a single collection.
        /// </summary>
        /// <param name="items">Collection to merge with.</param>
        public void Merge(FtpItemCollection items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items), "must have a value");

            foreach (FtpItem item in items)
            {
                var newItem = new FtpItem(item.Name, item.Modified, item.Size, item.SymbolicLink, item.Attributes,
                    item.ItemType, item.RawText) {ParentPath = item.ParentPath};
                Add(newItem);
            }
        }

        private void Parse(string path, string fileList, IFtpItemParser itemParser)
        {
            string[] lines = SplitFileList(fileList);

            int length = lines.Length - 1;
            for (int i = 0; i <= length; i++)
            {
                FtpItem item = itemParser.ParseLine(lines[i]);
                if (item != null && item.Name != "." & item.Name != "..")
                {
                    // set the parent path to the value passed in
                    item.ParentPath = path;
                    _list.Add(item);
                    TotalSize += item.Size;
                }
            }
        }

        private string[] SplitFileList(string response)
        {
            var crlfSplit = new char[2];
            crlfSplit[0] = '\r';
            crlfSplit[1] = '\n';
            return response.Split(crlfSplit, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        ///     Convert current FtpCollection to a DataTable object.
        /// </summary>
        /// <returns>Data table object.</returns>
        public DataTable ToDataTable()
        {
            var dataTbl = new DataTable {Locale = CultureInfo.InvariantCulture};

            CreateColumns(dataTbl);

            foreach (FtpItem item in _list)
            {
                DataRow row = dataTbl.NewRow();
                row[COL_NAME] = item.Name;
                row[COL_MODIFIED] = item.Modified;
                row[COL_SIZE] = item.Size;
                row[COL_SYMBOLIC_LINK] = item.SymbolicLink;
                row[COL_TYPE] = item.ItemType.ToString();
                row[COL_ATTRIBUTES] = item.Attributes;
                row[COL_RAW_TEXT] = item.RawText;
                dataTbl.Rows.Add(row);
            }

            return dataTbl;
        }

        private void CreateColumns(DataTable dataTbl)
        {
            dataTbl.Columns.Add(new DataColumn(COL_NAME, typeof (string)));
            dataTbl.Columns.Add(new DataColumn(COL_MODIFIED, typeof (DateTime)));
            dataTbl.Columns.Add(new DataColumn(COL_SIZE, typeof (long)));
            dataTbl.Columns.Add(new DataColumn(COL_TYPE, typeof (string)));
            dataTbl.Columns.Add(new DataColumn(COL_ATTRIBUTES, typeof (string)));
            dataTbl.Columns.Add(new DataColumn(COL_SYMBOLIC_LINK, typeof (string)));
            dataTbl.Columns.Add(new DataColumn(COL_RAW_TEXT, typeof (string)));
        }

        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the entire FtpItemCollection list.
        /// </summary>
        /// <param name="item">The FtpItem to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(FtpItem item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        ///     Adds an FtpItem to the end of the FtpItemCollection list.
        /// </summary>
        /// <param name="item">FtpItem object to add.</param>
        public void Add(FtpItem item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///     Searches for the specified object based on the 'name' parameter value
        ///     and returns true if an object with the name is found; otherwise false.
        /// </summary>
        /// <param name="name">The name of the FtpItem to locate in the collection.</param>
        /// <returns>True if the name if found; otherwise false.</returns>
        public bool ContainsName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "must have a value");

            return _list.Any(item => name == item.Name);
        }
    }
}