// UpdateFactoryPackage.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

namespace nUpdate.Administration.Models
{
    internal struct UpdateFactoryPackage
    {
        public UpdateFactoryPackage(string archivePath, UpdatePackage packageData)
        {
            ArchivePath = archivePath;
            PackageData = packageData;
        }

        internal string ArchivePath { get; set; }
        internal UpdatePackage PackageData { get; set; }
    }
}