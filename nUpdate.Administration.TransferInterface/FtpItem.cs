/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2007-2009 Starksoft, LLC (http://www.starksoft.com) 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

using System;

namespace nUpdate.Administration.TransferInterface
{
    /// <summary>
    ///     The available types for the <see cref="FtpItem" />.
    /// </summary>
    public enum FtpItemType
    {
        /// <summary>
        ///     Directory item.
        /// </summary>
        Directory,

        /// <summary>
        ///     File item.
        /// </summary>
        File,

        /// <summary>
        ///     Symbolic link item.
        /// </summary>
        SymbolicLink,

        /// <summary>
        ///     Block special file item.
        /// </summary>
        BlockSpecialFile,

        /// <summary>
        ///     Character special file item.
        /// </summary>
        CharacterSpecialFile,

        /// <summary>
        ///     Name socket item.
        /// </summary>
        NamedSocket,

        /// <summary>
        ///     Domain socket item.
        /// </summary>
        DomainSocket,

        /// <summary>
        ///     Unknown item.  The system was unable to determine the itemType of item.
        /// </summary>
        Unknown
    }


    /// <summary>
    ///     Represents a file or directory that is created when listing items on the server.
    /// </summary>
    public class FtpItem
    {
        private readonly string _attributes;
        private readonly FtpItemType _itemType;
        private readonly DateTime _modified;
        private readonly string _name;
        private readonly string _rawText;
        private readonly long _size;
        private readonly string _symbolicLink;
        private string _parentPath;

        /// <summary>
        ///     Initializes a new instace of the <see cref="FtpItem" />-class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="modified">The modified date and/or time of the item.</param>
        /// <param name="size">The size of the item.</param>
        /// <param name="symbolicLink">The symbolic link name.</param>
        /// <param name="attributes">The permission text for item.</param>
        /// <param name="itemType">The type of the item.</param>
        /// <param name="rawText">The raw text of the item.</param>
        public FtpItem(string name, DateTime modified, long size, string symbolicLink, string attributes,
            FtpItemType itemType, string rawText)
        {
            _name = name;
            _modified = modified;
            _size = size;
            _symbolicLink = symbolicLink;
            _attributes = attributes;
            _itemType = itemType;
            _rawText = rawText;
        }

        /// <summary>
        ///     The name of the item. All servers should report a name value for the <see cref="FtpItem" />.
        /// </summary>
        public string Name => _name;

        /// <summary>
        ///     The permissions text for the item. Some servers will report file permission information.
        /// </summary>
        public string Attributes => _attributes;

        /// <summary>
        ///     The modified date and possibly the time for the <see cref="FtpItem" />.
        /// </summary>
        public DateTime Modified => _modified;

        /// <summary>
        ///     The size of the <see cref="FtpItem" /> as reported by the server.
        /// </summary>
        public long Size => _size;

        /// <summary>
        ///     The symbolic link name if the <see cref="FtpItem" /> is a symbolic link.
        /// </summary>
        public string SymbolicLink => _symbolicLink;

        /// <summary>
        ///     The type of the <see cref="FtpItem" />.
        /// </summary>
        public FtpItemType ItemType => _itemType;

        /// <summary>
        ///     The raw textual line information as reported by the server. This can be useful for examining exotic FTP formats and
        ///     for debugging a custom ftp item parser.
        /// </summary>
        public string RawText => _rawText;

        /// <summary>
        ///     the path to the parent directory.
        /// </summary>
        public string ParentPath
        {
            get { return _parentPath; }
            set { _parentPath = value; }
        }

        /// <summary>
        ///     The full path of the <see cref="FtpItem" />.
        /// </summary>
        public string FullPath => _parentPath == "/" || _parentPath == "//"
            ? $"{_parentPath}{_name}"
            : $"{_parentPath}/{_name}";
    }
}