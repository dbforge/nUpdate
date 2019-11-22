using System;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.Models.Logging
{
    [Serializable]
    public class PackageActionLogData : Model
    {
        public PackageActionLogData(PackageActionType type, string packageName)
        {
            EntryDateTime = DateTime.Now;
            PackageActionType = type;
            PackageName = packageName;
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.DateTime"/> object that represents when this <see cref="PackageActionLogData"/> has been created.
        /// </summary>
        public DateTime EntryDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the type of action that has been performed on a package.
        /// </summary>
        public PackageActionType PackageActionType { get; set; }

        /// <summary>
        ///     Gets or sets the name of the package.
        /// </summary>
        public string PackageName { get; set; }
    }
}