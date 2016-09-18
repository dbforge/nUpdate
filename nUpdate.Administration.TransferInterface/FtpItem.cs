// Author: Dominic Beger (Trade/ProgTrade) 2016

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
            Name = name;
            Modified = modified;
            Size = size;
            SymbolicLink = symbolicLink;
            Attributes = attributes;
            ItemType = itemType;
            RawText = rawText;
        }

        /// <summary>
        ///     The name of the item. All servers should report a name value for the <see cref="FtpItem" />.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The permissions text for the item. Some servers will report file permission information.
        /// </summary>
        public string Attributes { get; }

        /// <summary>
        ///     The modified date and possibly the time for the <see cref="FtpItem" />.
        /// </summary>
        public DateTime Modified { get; }

        /// <summary>
        ///     The size of the <see cref="FtpItem" /> as reported by the server.
        /// </summary>
        public long Size { get; }

        /// <summary>
        ///     The symbolic link name if the <see cref="FtpItem" /> is a symbolic link.
        /// </summary>
        public string SymbolicLink { get; }

        /// <summary>
        ///     The type of the <see cref="FtpItem" />.
        /// </summary>
        public FtpItemType ItemType { get; }

        /// <summary>
        ///     The raw textual line information as reported by the server. This can be useful for examining exotic FTP formats and
        ///     for debugging a custom ftp item parser.
        /// </summary>
        public string RawText { get; }

        /// <summary>
        ///     the path to the parent directory.
        /// </summary>
        public string ParentPath { get; set; }

        /// <summary>
        ///     The full path of the <see cref="FtpItem" />.
        /// </summary>
        public string FullPath
            =>
                ParentPath == "/" || ParentPath == "//"
                    ? $"{ParentPath}{Name}"
                    : $"{ParentPath}/{Name}";
    }
}