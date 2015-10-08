using System;
using System.Collections.Generic;

namespace nUpdate.Administration
{
    [Serializable]
    public class PackageItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PackageItem" /> class.
        /// </summary>
        public PackageItem()
        {
            IsRoot = true;
            Children = new List<PackageItem>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PackageItem" /> class.
        /// </summary>
        /// <param name="hash">The hash of the <see cref="PackageItem" />.</param>
        /// <param name="name">The name of the <see cref="PackageItem" />.</param>
        /// <param name="parentGuid">The unique identifier of the parent of this <see cref="PackageItem" />.</param>
        /// <param name="isDirectory">If set to <c>true</c> the <see cref="PackageItem" /> is a directory.</param>
        public PackageItem(byte[] hash, string name, Guid parentGuid, bool isDirectory)
            : this(BitConverter.ToString(hash).Replace("-", string.Empty), name, parentGuid, isDirectory)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PackageItem" /> class.
        /// </summary>
        /// <param name="hash">The hash of the <see cref="PackageItem" />.</param>
        /// <param name="name">The name of the <see cref="PackageItem" />.</param>
        /// <param name="parentGuid">The unique identifier of the parent of this <see cref="PackageItem" />.</param>
        /// <param name="isDirectory">If set to <c>true</c> the <see cref="PackageItem" /> is a directory.</param>
        public PackageItem(string hash, string name, Guid parentGuid, bool isDirectory)
        {
            Hash = hash;
            Name = name;
            IsDirectory = isDirectory;
            Guid = Guid.NewGuid();
            ParentGuid = parentGuid;
            Children = new List<PackageItem>();
        }

        /// <summary>
        ///     Gets or sets the hash of this <see cref="PackageItem" /> whose bytes are represented as <see cref="String" />.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        ///     Gets or sets the name of this <see cref="PackageItem" /> including the extension.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier of the parent of this <see cref="PackageItem" />.
        /// </summary>
        public Guid ParentGuid { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="PackageItem" /> is directory, or not.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is directory; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirectory { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier of this <see cref="PackageItem" />.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="PackageItem" /> is a root-item, or not.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is root; otherwise, <c>false</c>.
        /// </value>
        public bool IsRoot { get; set; }

        /// <summary>
        ///     Gets or sets the children of the current <see cref="PackageItem" />.
        /// </summary>
        public List<PackageItem> Children { get; set; }
    }
}