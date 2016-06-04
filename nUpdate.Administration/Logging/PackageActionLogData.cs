using System;

namespace nUpdate.Administration.Logging
{
    [Serializable]
    public struct PackageActionLogData
    {
        public PackageActionLogData(PackageActionType type, IUpdateVersion packageVersion)
        {
            EntryDateTime = DateTime.Now;
            PackageActionType = type;
            PackageVersion = packageVersion;
        }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime"/> object that represents when this <see cref="PackageActionLogData"/> has been created.
        /// </summary>
        public DateTime EntryDateTime { get; set; }

        /// <summary>
        ///     Gets or sets the type of action that has been performed on a package.
        /// </summary>
        public PackageActionType PackageActionType { get; set; }

        /// <summary>
        ///     Gets or sets the version of the package.
        /// </summary>
        public IUpdateVersion PackageVersion { get; set; }
    }
}